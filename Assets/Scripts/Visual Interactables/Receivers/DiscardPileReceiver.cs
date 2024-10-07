using System;
using UnityEngine;

public class DiscardPileReceiver : MonoBehaviour, IActionReceiver
{
    [field:SerializeField] public PlayerCharacter Owner { get; private set; }
    [field:SerializeField] public Transform SnapTransform { get; private set; }

    public bool IsDropEnabled { get; private set; } = true;
    public bool CanInteractWithoutOwnership => false;


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

    public void OnChoosingSelect()
    {
        OnDraggingSelect();
    }

    public void OnChoosingDeselect()
    {
        OnDraggingDeselect();
    }

    public Receiver GetReceiverStruct(ValidDropLocation actionDropLocation) => 
        new (actionDropLocation, Owner, -1, -1);
    
}
