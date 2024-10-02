using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayableItem : MonoBehaviour, IInteractable
{
    [field:SerializeField] public PlayerCharacter Owner { get; private set; }
    [field:SerializeField] public bool OnlyVisibleOnOverview { get; private set; }

    public bool IsDraggable { get; private set; } = true;
    public ITurnAction Action { get; }

    [SerializeField] private bool _meshInChild;
    [SerializeField] private float _onDragOffset = 0.5f;
    private Transform _meshTransform;
    private Vector3 _defaultMeshScale;
    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    private Transform _camTransform;

    private void Awake()
    {
        _meshTransform = _meshInChild ? transform.GetChild(0) : transform;
        if (!_meshTransform.TryGetComponent<MeshRenderer>(out _))
            Debug.LogError($"Playable Item without mesh child! ({gameObject.name})");
        else
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
        transform.Translate(_camTransform.forward * _onDragOffset);
    }

    public void OnDragCancel()
    {
        //animacion para volver a su pos inicial
        transform.position = _defaultPosition;
        transform.rotation = _defaultRotation;
    }

    public void OnDrop(IDropLocation dropLocation)
    {
        //se snappea a la drop location
        Destroy(gameObject);
    }

}
