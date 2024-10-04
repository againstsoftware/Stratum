using System;
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
    public bool IsDragging { get; private set; }


    [SerializeField] private PlayerCharacter _playerOnTurn;
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private LayerMask _dropLocationLayer;
    [SerializeField] private float _itemCamOffsetOnDrag;

    private InputAction _pointerPosAction;
    private Camera _cam;
    private CameraMovement _cameraMovement;
    private Transform _dragItemTransform;
    private Vector3 _screenPointerPosition;
    // private Vector3 _screenOffsetOnDrag;
    private Rulebook _rulebook;
    private bool _isSelectedRulebookOpener;
    private APlayableItem _draggingItem;


    #region Callbacks

    private void Awake()
    {
        ServiceLocator.Register<IInteractionSystem>(this);
        ServiceLocator.Register<IRulesSystem>(new DummyRulesManager()); //de pega
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
        if (!IsDragging) return;
        var newPos = _cam.ScreenToWorldPoint(
            new Vector3(_screenPointerPosition.x, _screenPointerPosition.y, _itemCamOffsetOnDrag));//0f) + _screenOffsetOnDrag);
        newPos.y = Mathf.Max(.2f, newPos.y);
        _dragItemTransform.position = newPos;
        CheckDropLocations();
    }

    private void OnPointerPositionChanged(InputAction.CallbackContext ctx)
    {
        _screenPointerPosition = ctx.ReadValue<Vector2>();
    }

    #endregion


    public void SelectInteractable(IInteractable item)
    {
        if (IsDragging) return;
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
        if (IsDragging || SelectedInteractable != item) return;

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
        if (!item.IsDraggable) return;
        if (item.Owner != _playerOnTurn) return;
        if (item != SelectedInteractable as APlayableItem)
        {
            throw new Exception("drag called with non selected item!");
        }

        _draggingItem = item;
        item.OnDrag();
        IsDragging = true;
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
        if (!item.IsDraggable) return;
        if (item != SelectedInteractable as APlayableItem)
        {
            // throw new Exception("drop called with non selected item!");
            return;
        }

        IsDragging = false;

        var dropLocation = SelectedDropLocation;
        DeselectInteractable(SelectedInteractable);

        if (SelectedDropLocation is null)
        {
            item.OnDragCancel();
            if (!item.OnlyVisibleOnOverview) _cameraMovement.ChangeToDefault();
            return;
        }

        SelectedDropLocation.OnDraggingDeselect();
        SelectedDropLocation = null;

        if (ServiceLocator.Get<IRulesSystem>()
            .IsValidAction(item, dropLocation)) //es una accion valida segun el cliente
        {
            ServiceLocator.Get<IRulesSystem>().TryPerformAction(item, dropLocation,
                () =>
                {
                    item.OnDrop(SelectedDropLocation);
                    if (!item.OnlyVisibleOnOverview) _cameraMovement.ChangeToDefault();
                    //asi de momento, esto hay que meterselo a un patron command que ejecute todas los pasos de la accion secuencialmente
                });
        }
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