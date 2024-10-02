using System;
using UnityEngine;

public class InteractableCard : MonoBehaviour, IInteractable
{
    public GameObject GameObject { get => gameObject; }
    [field:SerializeField] public PlayerCharacter Owner { get; private set; }
    public bool IsDraggable { get; private set; } = true;
    public ITurnAction Action { get; }

    private Transform _meshTransform;
    private Vector3 _defaultMeshScale;
    private Vector3 _defaultPosition;

    private void Awake()
    {
        _meshTransform = transform.GetChild(0);
        if (!_meshTransform.TryGetComponent<MeshRenderer>(out _))
            Debug.LogError($"Card without mesh child! ({gameObject.name})");
        else
            _defaultMeshScale = _meshTransform.localScale;
        _defaultPosition = transform.position;
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
        transform.position = _defaultPosition;
    }

    public void OnDrop(IDropLocation dropLocation)
    {
        //se snappea a la drop location
        Destroy(gameObject);
    }

}
