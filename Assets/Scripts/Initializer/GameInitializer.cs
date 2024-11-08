using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private GameConfig _config;
    [SerializeField] private Deck[] _decks;
    [SerializeField] private bool _isTestScene;
    private void Awake()
    {
        var gameModel = new GameModel(_config.TurnOrder[0], _decks.Cast<IDeck>().ToArray());
        
        ServiceLocator.Register<IModel>(gameModel);
        
        ServiceLocator.Register<IInteractionSystem>(FindAnyObjectByType<InteractionManager>());
        
        // ServiceLocator.Register<IRulesSystem>(FindAnyObjectByType<DummyRulesManager>()); //de pega
        ServiceLocator.Register<IRulesSystem>(FindAnyObjectByType<RulesManager>()); 
        
        ServiceLocator.Register<IExecutor>(new EffectExecutor());
        
        ServiceLocator.Register<IView>(FindAnyObjectByType<ViewManager>());
        ServiceLocator.Register<ITurnSystem>(FindAnyObjectByType<TurnManager>());
        
        if(_isTestScene)  ServiceLocator.Register<ICommunicationSystem>(FindAnyObjectByType<TestModeCommunications>());
        else ServiceLocator.Register<ICommunicationSystem>(FindAnyObjectByType<GameNetwork>());

        EffectCommands.Config = _config;
    }

    private IEnumerator Start()
    {
        yield return null;
        ServiceLocator.Get<ITurnSystem>().StartGame();
    }
}