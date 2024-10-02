

public interface IDropLocation
{
    public PlayerCharacter Owner { get; }
    public bool IsDropEnabled { get; }
    public IActionReceiver Receiver { get; }
    
    public void OnSelect();
    public void OnDeselect();
    
}
