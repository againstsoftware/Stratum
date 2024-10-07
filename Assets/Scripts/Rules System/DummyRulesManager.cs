using System;


public class DummyRulesManager : IRulesSystem
{
    
    public bool IsValidAction(PlayerAction action) => true;
    public void PerformAction(PlayerAction action)
    {
        ServiceLocator.Get<IExecutor>().ExecuteEffectCommands(action);
    }

}