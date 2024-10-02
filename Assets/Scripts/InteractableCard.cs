using System;
using UnityEngine;

public class InteractableCard : MonoBehaviour, IInteractable
{
    [field:SerializeField] public PlayerCharacter Owner { get; private set; }
    public bool IsAction { get; } = true;
    public bool IsDraggable { get; } = true;

    private Transform _meshTransform;
    private Vector3 _normalMeshScale;

    private void Awake()
    {
        _meshTransform = transform.GetChild(0);
        if (!_meshTransform.TryGetComponent<MeshRenderer>(out _))
            Debug.LogError($"Card without mesh child! ({gameObject.name})");
        else
            _normalMeshScale = _meshTransform.localScale;
    }

    public void OnSelect()
    {
        _meshTransform.localScale = _normalMeshScale * 1.15f;
    }

    public void OnDeselect()
    {
        _meshTransform.localScale = _normalMeshScale;
    }

    public void OnDragStart()
    {
        
    }

    public void OnDragCancel()
    {
        
    }

    public void OnDragItemHover(IInteractable draggingItem)
    {
    }

    public void OnDragItemRelease(IInteractable draggingItem)
    {
    }

}
