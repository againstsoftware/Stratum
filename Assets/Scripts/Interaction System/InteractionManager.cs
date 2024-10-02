using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }
    public IInteractable SelectedInteractable { get; private set; }
    public IDropLocation SelectedDropLocation { get; private set; }

    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private LayerMask _dropLocationLayer;

    private InputAction _pointerPosAction, _scrollAction;
    private Camera _cam;
    private CameraMovement _cameraMovement;
    private bool _isDragging;
    private Transform _dragItemTransform;
    private Vector3 _screenPointerPosition;
    private Vector3 _offset;
    private bool _isCamDefault = true;


    #region Callbacks
    private void Awake()
    {
        if (Instance is null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _cam = Camera.main;
        _cameraMovement = _cam.GetComponent<CameraMovement>();
    }

    private void OnEnable()
    {
        _pointerPosAction = _inputActions.FindAction("PointerPosition");
        _pointerPosAction.performed += OnPointerPositionChanged;
        _scrollAction = _inputActions.FindAction("Scroll");
        _scrollAction.performed += OnScroll;
    }

    private void OnDisable()
    {
        _pointerPosAction.performed -= OnPointerPositionChanged;
        _scrollAction.performed -= OnScroll;
    }

    private void Update()
    {
        if (!_isDragging) return;
        _dragItemTransform.position = GetPointerWorldPos();// + _offset;
        _dragItemTransform.rotation = Quaternion.LookRotation(_cam.transform.forward, Vector3.up);
        CheckDropLocations();
    }
    
    private void OnPointerPositionChanged(InputAction.CallbackContext ctx)
    {
        _screenPointerPosition = ctx.ReadValue<Vector2>();
    }

    private void OnScroll(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<Vector2>().y;
        if (scroll == 0f || _isDragging) return;
        if (scroll > 0f && _isCamDefault)
        {
            _cameraMovement.ChangeToOverview();
            _isCamDefault = false;
        }
        else if (scroll < 0f && !_isCamDefault)
        {
            _cameraMovement.ChangeToDefault();
            _isCamDefault = true;
        }
    }
    

    #endregion
    
    
    public void SelectInteractable(IInteractable item)
    {
        if (_isDragging) return;
        if(item is null)
        {
            Debug.LogError("select called with null item!");
            return;
        }
        
        var old = SelectedInteractable;
        if(old is not null) old.OnDeselect();
        SelectedInteractable = item;
        item.OnSelect();
    }

    public void DeselectInteractable(IInteractable item)
    {
        if (_isDragging || SelectedInteractable != item) return;
        
        if(SelectedInteractable is not null) SelectedInteractable.OnDeselect();
        SelectedInteractable = null;
    }

    public void DragPlayableItem(PlayableItem item)
    {
        if (!item.IsDraggable) return;
        if (item != SelectedInteractable as PlayableItem)
        {
            Debug.LogError("drag called with non selected item!");
            return;
        }
        
        item.OnDrag();
        _isDragging = true;
        _dragItemTransform = item.transform;
        _offset = _dragItemTransform.position - GetPointerWorldPos();
        
        if(!item.OnlyVisibleOnOverview) _cameraMovement.ChangeToOverview();
    }

    public void DropPlayableItem(PlayableItem item)
    {
        if (!item.IsDraggable) return;
        if (item != SelectedInteractable as PlayableItem)
        {
            Debug.LogError("drop called with non selected item!");
            return;
        }
        _isDragging = false;

        var dropLocation = SelectedDropLocation;
        DeselectInteractable(SelectedInteractable);
        
        if (SelectedDropLocation is not null)
        {
            SelectedDropLocation.OnDeselect();
            SelectedDropLocation = null;
        }
        if (dropLocation is null || !GameManager.IsValidAction(item, dropLocation))
        {
            item.OnDragCancel();
            if(!item.OnlyVisibleOnOverview) _cameraMovement.ChangeToDefault();
        }
        
        
        else //es una accion valida segun el cliente
        {
            GameManager.TryPerformAction(item, dropLocation, 
                () => 
            {
                item.OnDrop(SelectedDropLocation);
                if(!item.OnlyVisibleOnOverview) _cameraMovement.ChangeToDefault();
                //asi de momento, esto hay que meterselo a un patron command que ejecute todas los pasos de la accion secuencialmente
            });
        }
    }

    private Vector3 GetPointerWorldPos()
    {
        float z = _cam.WorldToScreenPoint(_dragItemTransform.position).z;
        return _cam.ScreenToWorldPoint(_screenPointerPosition + new Vector3(0, 0, z));
    }

    private void CheckDropLocations()
    {
        Ray ray = _cam.ScreenPointToRay(_screenPointerPosition);
        if (!Physics.Raycast(ray, out var hitInfo, float.MaxValue, _dropLocationLayer) || hitInfo.collider is null)
        {
            if (SelectedDropLocation is not null)
            {
                SelectedDropLocation.OnDeselect();
                SelectedDropLocation = null;
            }
            return;
        }

        var old = SelectedDropLocation;
        SelectedDropLocation = hitInfo.collider.GetComponentInParent<IDropLocation>();
        if (old == SelectedDropLocation) return;
        if(old is not null) old.OnDeselect();
        if (!SelectedDropLocation.IsDropEnabled)
        {
            SelectedDropLocation = null;
            return;
        }
        SelectedDropLocation.OnSelect();
        
    }
}
