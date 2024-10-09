using System;
using System.Collections;
using UnityEngine;


public class DummyRulesManager : IRulesSystem
{
    
    public bool IsValidAction(PlayerAction action) => true;
    public void PerformAction(PlayerAction action)
    {
        (ServiceLocator.Get<ITurnSystem>() as MonoBehaviour). //puede ser cualquier monobehaviour (es de pega)
            StartCoroutine(SendToExecute(action));
    }

    private IEnumerator SendToExecute(PlayerAction action)
    {
        yield return null;
        
        ServiceLocator.Get<ICommunicationSystem>().SendActionToAuthority(action);
        
        // ServiceLocator.Get<IExecutor>().ExecuteEffectCommands(action);
    }

}