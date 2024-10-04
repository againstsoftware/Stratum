

using UnityEngine;

public interface IActionReceiver
{
    public PlayerCharacter Owner { get; }
    public bool CanInteractWithoutOwnership { get; }

    public bool IsDropEnabled { get; }
    public Transform SnapTransform { get; }
    
    public void OnDraggingSelect();
    public void OnDraggingDeselect();
    public void OnChoosingSelect();
    public void OnChoosingDeselect();

}
