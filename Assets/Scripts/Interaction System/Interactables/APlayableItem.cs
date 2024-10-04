using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class APlayableItem : MonoBehaviour, IInteractable
{
    [field:SerializeField] public PlayerCharacter Owner { get; protected set; } //!
    public bool IsDraggable { get; protected set; } = true;
    public bool IsPlayed { get; protected set; }
    

    public abstract bool OnlyVisibleOnOverview { get; }
    public abstract bool CanInteractWithoutOwnership { get; }

    public abstract IActionItem ActionItem { get; }

    

    [SerializeField] private bool _meshInChild;
    [SerializeField] private float _returnDuration;
    private Transform _meshTransform;
    private Vector3 _defaultMeshScale;
    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    private Transform _camTransform;
    private Collider _collider;
    private bool _isReturning;
    private float t;
    private Vector3 _returnStartPosition;
    private Quaternion _returnStartRotation;

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

    private void Update()
    {
        if (!_isReturning) return;

        transform.position = Vector3.Lerp(_returnStartPosition, _defaultPosition,  t);
        transform.rotation = Quaternion.Lerp( _returnStartRotation, _defaultRotation, t);
        t += Time.deltaTime / _returnDuration;
        if (t >= 1f)
        {
            _isReturning = false;
            _collider.enabled = true;
            transform.SetPositionAndRotation(_defaultPosition, _defaultRotation);
        }
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
        // transform.SetPositionAndRotation(_defaultPosition, _defaultRotation);
        _isReturning = true;
        t = 0f;
        _collider.enabled = false;
        _returnStartPosition = transform.position;
        _returnStartRotation = transform.rotation;
    }

    public virtual void OnDrop(IActionReceiver dropLocation)
    {
        //se snappea a la drop location
        transform.position = dropLocation.SnapTransform.position;
    }

    public void SetColliderActive(bool active) => _collider.enabled = active;


}
