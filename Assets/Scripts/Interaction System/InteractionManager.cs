using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InteractionManager : MonoBehaviour, IInteractionSystem
{
    public IInteractable SelectedInteractable { get; private set; }
    public IActionReceiver SelectedDropLocation { get; private set; }
    public InputActionAsset InputActions => _inputActions;
    public IInteractionSystem.State CurrentState { get; private set; } = IInteractionSystem.State.Idle;


    [SerializeField] private PlayerCharacter _playerOnTurn;
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private LayerMask _dropLocationLayer;
    [SerializeField] private float _itemCamOffsetOnDrag;
    [SerializeField] private float _dropLocationCheckFrequency;

    private InputAction _pointerPosAction;
    private Camera _cam;
    private CameraMovement _cameraMovement;
    private Transform _dragItemTransform;
    private Vector3 _screenPointerPosition;
    // private Vector3 _screenOffsetOnDrag;
    private Rulebook _rulebook;
    private bool _isSelectedRulebookOpener;
    private APlayableItem _draggingItem;
    private float _dropLocationCheckTimer, _dropLocationCheckPeriod;
    private HashSet<IActionReceiver> _selectedReceivers = new();
    private IActionReceiver _selectedReceiver;


    #region Callbacks

    private void Awake()
    {
        ServiceLocator.Register<IInteractionSystem>(this);
        ServiceLocator.Register<IRulesSystem>(new DummyRulesManager()); //de pega

        _dropLocationCheckPeriod = 1f / _dropLocationCheckFrequency;
    }

    private void Start()
    {
        _cam = Camera.main;
        _cameraMovement = _cam.GetComponent<CameraMovement>();
        _rulebook = FindAnyObjectByType<Rulebook>();
    }

    private void OnEnable()
    {
        _pointerPosAction = _inputActions.FindAction("PointerPosition");
        _pointerPosAction.performed += OnPointerPositionChanged;
    }

    private void OnDisable()
    {
        _pointerPosAction.performed -= OnPointerPositionChanged;
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case IInteractionSystem.State.Waiting:
                break;
            
            case IInteractionSystem.State.Idle:
                break;
            
            case IInteractionSystem.State.Dragging:
                var newPos = _cam.ScreenToWorldPoint(
                    new Vector3(_screenPointerPosition.x, _screenPointerPosition.y, _itemCamOffsetOnDrag));
                newPos.y = Mathf.Max(.2f, newPos.y);
                _dragItemTransform.position = newPos;
                _dropLocationCheckTimer += Time.deltaTime;
                if (_dropLocationCheckTimer < _dropLocationCheckPeriod) return;
                _dropLocationCheckTimer = 0f;
                CheckDropLocations();
                break;
            
            case IInteractionSystem.State.Choosing:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnPointerPositionChanged(InputAction.CallbackContext ctx)
    {
        _screenPointerPosition = ctx.ReadValue<Vector2>();
    }

    #endregion


    public void SelectInteractable(IInteractable item)
    {
        if (CurrentState is IInteractionSystem.State.Dragging) return;
        if (item is null)
        {
            throw new Exception("select called with null item!");
        }

        if (!item.CanInteractWithoutOwnership && item.Owner != _playerOnTurn) return;
        var old = SelectedInteractable;
        if (old is not null) old.OnDeselect();
        SelectedInteractable = item;
        item.OnSelect();
        if (item is Component itemC && itemC.TryGetComponent<IRulebookOpener>(out var rulebookOpener))
        {
            _isSelectedRulebookOpener = true;
            var entry = rulebookOpener.RulebookEntry;
            _rulebook.ShowRulebookEntry(entry);
        }
    }

    public void DeselectInteractable(IInteractable item)
    {
        if (CurrentState is IInteractionSystem.State.Dragging || SelectedInteractable != item) return;

        if (SelectedInteractable is not null) SelectedInteractable.OnDeselect();
        SelectedInteractable = null;
        if (_isSelectedRulebookOpener)
        {
            _isSelectedRulebookOpener = false;
            _rulebook.HideRulebook();
        }
    }

    public void DragPlayableItem(APlayableItem item)
    {
        if (CurrentState is not IInteractionSystem.State.Idle) return;
        if (!item.IsDraggable) return;
        if (item.Owner != _playerOnTurn) return;
        if (item != SelectedInteractable as APlayableItem)
        {
            throw new Exception("drag called with non selected item!");
        }

        _draggingItem = item;
        item.OnDrag();
        CurrentState = IInteractionSystem.State.Dragging;
        _dragItemTransform = item.transform;
        // _screenOffsetOnDrag = _cam.WorldToScreenPoint(_dragItemTransform.position) - _screenPointerPosition;
        // _screenOffsetOnDrag.z = _itemCamOffsetOnDrag;
        _dragItemTransform.rotation = Quaternion.LookRotation(Vector3.down, _dragItemTransform.forward);
        
        if (!item.OnlyVisibleOnOverview) _cameraMovement.ChangeToOverview();

        if (_isSelectedRulebookOpener)
        {
            _isSelectedRulebookOpener = false;
            _rulebook.HideRulebook();
        }
    }

    public void DropPlayableItem(APlayableItem item)
    {
        if (CurrentState is not IInteractionSystem.State.Dragging) return;
        if (!item.IsDraggable) return;
        if (item != SelectedInteractable as APlayableItem)
        {
            // throw new Exception("drop called with non selected item!");
            return;
        }
        
        var dropLocation = SelectedDropLocation;
        DeselectInteractable(SelectedInteractable);

        if (SelectedDropLocation is null)
        {
            CurrentState = IInteractionSystem.State.Idle;
            item.OnDragCancel();
            if (!item.OnlyVisibleOnOverview) _cameraMovement.ChangeToDefault();
            return;
        }

        SelectedDropLocation.OnDraggingDeselect();
        SelectedDropLocation = null;

        switch(ActionAssembler.TryAssembleAction(item, dropLocation))
        {
            case ActionAssembler.AssemblyState.Failed:
                CurrentState = IInteractionSystem.State.Idle;
                item.OnDragCancel();
                if (!item.OnlyVisibleOnOverview) _cameraMovement.ChangeToDefault();
                break;
            
            case ActionAssembler.AssemblyState.Ongoing:
                CurrentState = IInteractionSystem.State.Choosing;
                _selectedReceivers.Clear();
                _selectedReceivers.Add(dropLocation);
                item.OnDrop(dropLocation);
                break;
            
            case ActionAssembler.AssemblyState.Completed:
                CurrentState = IInteractionSystem.State.Waiting;
                Debug.Log("accion ensamblada!!!");
                break;
        }
    }


    public void ClickReceiver(IActionReceiver receiver)
    {
        if (CurrentState is not IInteractionSystem.State.Choosing) return;
        if (!receiver.CanInteractWithoutOwnership) return;
        if (_selectedReceivers.Contains(receiver)) return;
        
        switch (ActionAssembler.AddReceiver(receiver))
        {
            case ActionAssembler.AssemblyState.Failed:
                CurrentState = IInteractionSystem.State.Idle;
                _draggingItem.OnDragCancel();
                if (!_draggingItem.OnlyVisibleOnOverview) _cameraMovement.ChangeToDefault();
                if(_selectedReceiver is not null) _selectedReceiver.OnChoosingDeselect();
                break;
            
            case ActionAssembler.AssemblyState.Ongoing:
                _selectedReceivers.Add(receiver);
                break;
            
            case ActionAssembler.AssemblyState.Completed:
                CurrentState = IInteractionSystem.State.Waiting;
                Debug.Log("accion ensamblada!!!");
                break;
        }
    }

    public void SelectReceiver(IActionReceiver receiver)
    {
        if (CurrentState is not IInteractionSystem.State.Choosing) return;
        if (!receiver.CanInteractWithoutOwnership) return;
        if (_selectedReceivers.Contains(receiver)) return;
        if(_selectedReceiver is not null) _selectedReceiver.OnChoosingDeselect();
        _selectedReceiver = receiver;
        receiver.OnChoosingSelect();
    }

    public void DeselectReceiver(IActionReceiver receiver)
    {
        if (CurrentState is not IInteractionSystem.State.Choosing) return;
        if (_selectedReceivers.Contains(receiver)) return;

        receiver.OnChoosingDeselect();
        _selectedReceiver = null;
    }

    
    

    private void CheckDropLocations()
    {
        _draggingItem.SetColliderActive(false);
        Ray ray = _cam.ScreenPointToRay(_screenPointerPosition);
        var hit = Physics.Raycast(ray, out var hitInfo, float.MaxValue, _dropLocationLayer);
        _draggingItem.SetColliderActive(true);
        if (!hit || hitInfo.collider is null)
        {
            if (SelectedDropLocation is null) return;
            SelectedDropLocation.OnDraggingDeselect();
            SelectedDropLocation = null;
            return;
        }

        var newDropLocation = hitInfo.collider.GetComponentInParent<IActionReceiver>();

        if (newDropLocation == SelectedDropLocation) return;
        if (SelectedDropLocation is not null) SelectedDropLocation.OnDraggingDeselect();

        if (!newDropLocation.IsDropEnabled ||
            (!newDropLocation.CanInteractWithoutOwnership && newDropLocation.Owner != _draggingItem.Owner))
            return;

        SelectedDropLocation = newDropLocation;

        if (!SelectedDropLocation.IsDropEnabled)
        {
            SelectedDropLocation = null;
            return;
        }

        SelectedDropLocation.OnDraggingSelect();
    }
}