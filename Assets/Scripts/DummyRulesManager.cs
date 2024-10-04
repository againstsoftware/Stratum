using System;


public class DummyRulesManager : IRulesSystem
{
    
    public bool IsValidAction(APlayableItem playableItem, IActionReceiver dropLocation) => true;
    public void TryPerformAction(APlayableItem item, IActionReceiver dropLocation, Action onApproved) => onApproved();
}