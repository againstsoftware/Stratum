using System;
using System.Collections;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private GameConfig _config;
    [SerializeField] private Deck[] _decks;

    private void Awake()
    {
        var gameModel = new GameModel(_config.TurnOrder[0], Array.ConvertAll(_decks, d => d as IDeck));
        ServiceLocator.Register<IModel>(gameModel);
        
        ServiceLocator.Register<IInteractionSystem>(FindAnyObjectByType<InteractionManager>());
        
        ServiceLocator.Register<IRulesSystem>(new DummyRulesManager()); //de pega
        
        ServiceLocator.Register<IExecutor>(new EffectExecutor());
        
        ServiceLocator.Register<IView>(FindAnyObjectByType<ViewManager>());
        ServiceLocator.Register<ITurnSystem>(FindAnyObjectByType<TurnManager>());
    
        ServiceLocator.Register<ICommunicationSystem>(FindAnyObjectByType<GameNetwork>());
    }

    private IEnumerator Start()
    {
        yield return null;
        ServiceLocator.Get<ITurnSystem>().StartInitialTurn();
    }
}