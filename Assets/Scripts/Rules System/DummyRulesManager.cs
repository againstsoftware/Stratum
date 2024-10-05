using System;


public class DummyRulesManager : IRulesSystem
{
    
    public bool IsValidAction(TurnAction action) => true;
    public void PerformAction(TurnAction action, Action onApproved) => onApproved();
}