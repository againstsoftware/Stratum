

using UnityEngine;

public interface IActionReceiver
{
    public PlayerCharacter Owner { get; }
    public bool CanInteractWithoutOwnership { get; }

    public bool IsDropEnabled { get; }
    
    public void OnDraggingSelect();
    public void OnDraggingDeselect();
    public void OnChoosingSelect();
    public void OnChoosingDeselect();

    public Receiver GetReceiverStruct(ValidDropLocation actionDropLocation);

    public Transform GetSnapTransform(PlayerCharacter character);

}
