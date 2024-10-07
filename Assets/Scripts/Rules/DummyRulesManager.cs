using System;
using System.Collections;
using UnityEngine;


public class DummyRulesManager : IRulesSystem
{
    
    public bool IsValidAction(PlayerAction action) => true;
    public void PerformAction(PlayerAction action)
    {
        (ServiceLocator.Get<ITurnSystem>() as MonoBehaviour).
            StartCoroutine(cor(() => ServiceLocator.Get<IExecutor>().ExecuteEffectCommands(action)));
    }

    IEnumerator cor(Action a)
    {
        yield return null;
        a();

    }

}