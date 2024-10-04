using System;
using UnityEngine;

public class Slot : MonoBehaviour, IActionReceiver
{
    [field:SerializeField] public PlayerCharacter Owner { get; private set; }
    [field:SerializeField] public Territory Territory { get; private set; }
    public bool IsDropEnabled { get; private set; } = true;
    public bool CanInteractWithoutOwnership => true;
    
    private Material _material;
    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    public void OnDraggingSelect()
    {
        GetComponent<MeshRenderer>().material = null;
    }

    public void OnDraggingDeselect()
    {
        GetComponent<MeshRenderer>().material = _material;
    }
}