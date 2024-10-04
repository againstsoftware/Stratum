using System;


public class DummyRulesManager : IRulesSystem
{
    
    public bool IsValidAction(APlayableItem playableItem, IActionReceiver[] receivers) => true;
    public void PerformAction(APlayableItem item, IActionReceiver[] receivers, Action onApproved) => onApproved();
}