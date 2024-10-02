public interface IInteractionSystem : IService
{
    public IInteractable SelectedInteractable { get; }
    public IDropLocation SelectedDropLocation { get; }

    public void SelectInteractable(IInteractable item);
    public void DeselectInteractable(IInteractable item);
    public void DragPlayableItem(PlayableItem item);
    public void DropPlayableItem(PlayableItem item);

}