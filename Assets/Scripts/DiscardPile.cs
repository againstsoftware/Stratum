using System;
using UnityEngine;

public class DiscardPile : MonoBehaviour, IDropLocation
{
    public PlayerCharacter Owner { get; }
    public bool IsDropEnabled { get; } = true;
    public IActionReceiver Receiver { get; }


    private Material _material;
    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    public void OnSelect()
    {
        GetComponent<MeshRenderer>().material = null;
    }

    public void OnDeselect()
    {
        GetComponent<MeshRenderer>().material = _material;
    }
}
