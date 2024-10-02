using System;
using UnityEngine;

public class DiscardPile : MonoBehaviour, IDropLocation
{
    [field:SerializeField] public PlayerCharacter Owner { get; private set; }
    public bool IsDropEnabled { get; private set; } = true;
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
