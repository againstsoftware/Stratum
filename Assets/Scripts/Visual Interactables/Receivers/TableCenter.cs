
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class TableCenter : MonoBehaviour, IActionReceiver
{
    [field:SerializeField] public Transform SnapTransform { get; private set; }

    public PlayerCharacter Owner => PlayerCharacter.None;
    public bool IsDropEnabled => true;
    public bool CanInteractWithoutOwnership => true;

    [SerializeField] private MeshRenderer _tableMesh;
    
    
    private Material _material;
    private void Awake()
    {
        _material = _tableMesh.material;
    }

    public void OnDraggingSelect()
    {
        _tableMesh.material = null;
    }

    public void OnDraggingDeselect()
    {
        _tableMesh.material = _material;
    }
    
    
    public void OnChoosingSelect()
    {
        OnDraggingSelect();
    }

    public void OnChoosingDeselect()
    {
        OnDraggingDeselect();
    }

    public Receiver GetReceiverStruct(ValidDropLocation actionDropLocation)
    {
        throw new Exception("El centro de la mesa no es un receiver valido!! solo sirve para ensamblar la jugada!");
    }
}
