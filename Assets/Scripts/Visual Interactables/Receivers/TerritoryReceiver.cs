
using UnityEngine;

public class TerritoryReceiver : MonoBehaviour, IActionReceiver
{
    [field:SerializeField] public PlayerCharacter Owner { get; private set; }
    [field:SerializeField] public SlotReceiver[] Slots { get; private set; }
    [field:SerializeField] public Transform SnapTransform { get; private set; }
    public Transform GetSnapTransform(PlayerCharacter _) => SnapTransform;

    public bool IsDropEnabled { get; private set; } = true;
    public bool CanInteractWithoutOwnership => true;

    public bool HasConstruction { get; private set; }
    
    private Material _material;
    private GameObject _construction;
    private void Awake()
    {
        _material = transform.Find("Mesh").GetComponent<MeshRenderer>().material;

        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].IndexOnTerritory = i;
        }
    }

    public void BuildConstruction(GameObject prefab)
    {
        _construction = Instantiate(prefab, transform);
        // _construction.transform.localPosition = prefab.transform.position;
        //_construction.transform.localPosition = Vector3.zero;
        _construction.transform.localRotation = prefab.transform.rotation;
        HasConstruction = true;
    }

    public void DestroyConstruction()
    {
        Destroy(_construction);
        _construction = null;
        HasConstruction = false;
    }

    public void OnDraggingSelect()
    {
        transform.Find("Mesh").GetComponent<MeshRenderer>().material = null;
    }

    public void OnDraggingDeselect()
    {
        transform.Find("Mesh").GetComponent<MeshRenderer>().material = _material;
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
