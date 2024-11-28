using System;
using System.Collections.Generic;
using UnityEngine;

public class SlotReceiver : MonoBehaviour, IActionReceiver
{
    [field:SerializeField] public PlayerCharacter Owner { get; private set; }
    [field:SerializeField] public TerritoryReceiver Territory { get; private set; }
    [field:SerializeField] public Transform SnapTransform { get; private set; }
    public Transform GetSnapTransform(PlayerCharacter _) => SnapTransform;
    [field:SerializeField] public Transform SnapTransformBottom { get; private set; }
    [SerializeField] private Vector3 _offset;
    public bool IsDropEnabled { get; private set; } = true;
    public bool CanInteractWithoutOwnership => true;
    public int IndexOnTerritory { get; set; }

    public IReadOnlyList<PlayableCard> Cards => _cards;
    
    private readonly List<PlayableCard> _cards = new();
    
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
        _cards.Add(card);    
        UpdateCards();
    }

    public void AddCardAtTheBottom(PlayableCard card)
    {
        _cards.Insert(0, card);
        UpdateCards();
    }

    public void RemoveCard(PlayableCard card)
    {
        _cards.Remove(card);
        UpdateCards();
    }

    private void UpdateCards()
    {
        int i = 0;
        foreach (var c in _cards)
        {
            c.IndexOnSlot = i;
            SnapTransform.localPosition = SnapTransformBottom.localPosition + i * _offset;
            c.transform.position = SnapTransform.position;

            i++;
        }
        SnapTransform.localPosition = SnapTransformBottom.localPosition + _cards.Count * _offset;
    }

}