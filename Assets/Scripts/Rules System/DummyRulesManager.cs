using System;


public class DummyRulesManager : IRulesSystem
{
    
    public bool IsValidAction(PlayerCharacter actor, IActionItem actionItem, Receiver[] receivers) => true;
    public void PerformAction(PlayerCharacter actor, IActionItem actionItem, Receiver[] receivers)
    {
        
    }

}