
using UnityEngine;

public class Territory : MonoBehaviour, IActionReceiver
{
    [field:SerializeField] public PlayerCharacter Owner { get; private set; }
    [field:SerializeField] public Slot[] Slots { get; private set; }
    [field:SerializeField] public Transform SnapTransform { get; private set; }

    public bool IsDropEnabled { get; private set; } = true;
    public bool CanInteractWithoutOwnership => true;


    private Material _material;
    private void Awake()
    {
        _material = transform.Find("Mesh").GetComponent<MeshRenderer>().material;
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
}
