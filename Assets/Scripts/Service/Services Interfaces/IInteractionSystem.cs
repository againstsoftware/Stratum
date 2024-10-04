using UnityEngine.InputSystem;

public interface IInteractionSystem : IService
{
    public IInteractable SelectedInteractable { get; }
    public IActionReceiver SelectedDropLocation { get; }
    public InputActionAsset InputActions { get; }
    public bool IsDragging { get; }

    public void SelectInteractable(IInteractable item);
    public void DeselectInteractable(IInteractable item);
    public void DragPlayableItem(APlayableItem item);
    public void DropPlayableItem(APlayableItem item);

}