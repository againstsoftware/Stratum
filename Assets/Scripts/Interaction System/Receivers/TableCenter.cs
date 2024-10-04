
using UnityEngine;

public class TableCenter : MonoBehaviour, IActionReceiver
{
    [field:SerializeField] public Transform SnapTransform { get; private set; }

    public PlayerCharacter Owner => PlayerCharacter.None;
    public bool IsDropEnabled => true;
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
    
    
    public void OnChoosingSelect()
    {
        OnDraggingSelect();
    }

    public void OnChoosingDeselect()
    {
        OnDraggingDeselect();
    }
}
