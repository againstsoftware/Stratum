using System;


public class DummyRulesManager : IRulesSystem
{
    
    public bool IsValidAction(PlayableItem playableItem, IDropLocation dropLocation) => true;
    public void TryPerformAction(PlayableItem item, IDropLocation dropLocation, Action onApproved) => onApproved();
}