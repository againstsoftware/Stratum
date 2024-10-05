using System;
using System.Collections.Generic;
using UnityEngine;

public class SlotReceiver : MonoBehaviour, IActionReceiver
{
    [field:SerializeField] public PlayerCharacter Owner { get; private set; }
    [field:SerializeField] public TerritoryReceiver Territory { get; private set; }
    [field:SerializeField] public Transform SnapTransform { get; private set; }

    public bool IsDropEnabled { get; private set; } = true;
    public bool CanInteractWithoutOwnership => true;
    public int IndexOnTerritory { get; set; }

    private List<PlayableCard> _cardsOnTop = new();
    
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
        new (actionDropLocation, Owner, IndexOnTerritory, -1);

    
    public void AddCardOnTop(PlayableCard card)
    {
        _cardsOnTop.Add(card);    
        int i = 0;
        foreach(var c in _cardsOnTop) c.IndexOnSlot = i++; 
    }

    public void RemoveCardOnTop(PlayableCard card)
    {
        _cardsOnTop.Remove(card);
        int i = 0;
        foreach(var c in _cardsOnTop) c.IndexOnSlot = i++; 
    }

}