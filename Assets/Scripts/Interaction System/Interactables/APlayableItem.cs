using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class APlayableItem : MonoBehaviour, IInteractable
{
    [field:SerializeField] public PlayerCharacter Owner { get; protected set; } //!
    public bool IsDraggable { get; protected set; } = true;
    public abstract bool OnlyVisibleOnOverview { get; }
    public abstract bool CanInteractWithoutOwnership { get; }
    public abstract IActionItem ActionItem { get; }

    

    [SerializeField] private bool _meshInChild;
    private Transform _meshTransform;
    private Vector3 _defaultMeshScale;
    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    private Transform _camTransform;
    private Collider _collider;

    private void Awake()
    {
        _meshTransform = _meshInChild ? transform.GetChild(0) : transform;
        if (!_meshTransform.TryGetComponent<MeshRenderer>(out _))
            throw new Exception($"Playable Item without mesh child! ({gameObject.name})");

        _collider = _meshTransform.GetComponent<Collider>();
        _defaultMeshScale = _meshTransform.localScale;
        _defaultPosition = transform.position;
        _defaultRotation = transform.rotation;
    }

    private void Start()
    {
        _camTransform = Camera.main.transform;
    }

    public void OnSelect()
    {
        _meshTransform.localScale = _defaultMeshScale * 1.15f;
    }

    public void OnDeselect()
    {
        _meshTransform.localScale = _defaultMeshScale;
    }

    public void OnDrag()
    {
        OnDeselect();
    }

    public void OnDragCancel()
    {
        //animacion para volver a su pos inicial
        transform.SetPositionAndRotation(_defaultPosition, _defaultRotation);
    }

    public void OnDrop(IActionReceiver dropLocation)
    {
        //se snappea a la drop location
        //Destroy(gameObject);
    }

    public void SetColliderActive(bool active) => _collider.enabled = active;


}
