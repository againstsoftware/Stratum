
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


    [SerializeField] private float _sagitarioRotation, _fungalothRotation, _ygdraRotation, _overlordRotation;
    
    
    private Material _material;
    private Vector3 _defaultEulers;
    
    private void Awake()
    {
        _material = _tableMesh.material;
        _defaultEulers = SnapTransform.localRotation.eulerAngles;
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

    public Transform GetSnapTransform(PlayerCharacter character)
    {
        var newRot = _defaultEulers;
        switch (character)
        {
            case PlayerCharacter.None:
            case PlayerCharacter.Sagitario:
                newRot.z = _sagitarioRotation;
                break;
            case PlayerCharacter.Ygdra:
                newRot.z = _ygdraRotation;
                break;
            case PlayerCharacter.Fungaloth:
                newRot.z = _fungalothRotation;
                break;
            case PlayerCharacter.Overlord:
                newRot.z = _overlordRotation;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        SnapTransform.localRotation = Quaternion.Euler(newRot);
        return SnapTransform;
    }
}
