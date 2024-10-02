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

    private InputAction _pointerPosAction;
    private Camera _cam;
    private bool _isDragging;
    private Transform _dragItemTransform;
    private Vector3 _screenPointerPosition;
    private Vector3 _offset;


    #region Callbacks
    private void Awake()
    {
        if (Instance is null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _cam = Camera.main;
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
        if (!_isDragging) return;
        _dragItemTransform.position = GetPointerWorldPos() + _offset;
        CheckDropLocations();
    }
    
    private void OnPointerPositionChanged(InputAction.CallbackContext ctx)
    {
        _screenPointerPosition = ctx.ReadValue<Vector2>();
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

    public void DragInteractable(IInteractable item)
    {
        if (!item.IsDraggable) return;
        if (item != SelectedInteractable)
        {
            Debug.LogError("drag called with non selected item!");
            return;
        }
        
        _isDragging = true;
        _dragItemTransform = SelectedInteractable.GameObject.transform;
        _offset = _dragItemTransform.position - GetPointerWorldPos();
        item.OnDrag();
    }

    public void DropInteractable(IInteractable item)
    {
        if (!item.IsDraggable) return;
        if (item != SelectedInteractable)
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
        if (dropLocation is null || !GameManager.IsValidAction(item.Action, dropLocation.Receiver))
        {
            item.OnDragCancel();
        }
        else //es una accion valida segun el cliente
        {
            GameManager.TryPerformAction(item.Action, dropLocation.Receiver, 
                () => 
            {
                item.OnDrop(SelectedDropLocation);
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
