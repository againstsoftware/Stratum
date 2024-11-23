using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private GameConfig _config;
    [SerializeField] private Deck[] _decks;
    
    public enum GameMode { Match, Tutorial, Test }

    [SerializeField] private GameMode _gameMode;
    private void Awake()
    {
        var gameModel = new GameModel(_config, _decks);
        
        ServiceLocator.Register<IModel>(gameModel);
        
        ServiceLocator.Register<IInteractionSystem>(FindAnyObjectByType<InteractionManager>());
        
        ServiceLocator.Register<IRulesSystem>(FindAnyObjectByType<RulesManager>()); 
        
        ServiceLocator.Register<IExecutor>(new EffectExecutor());
        
        ServiceLocator.Register<IView>(FindAnyObjectByType<ViewManager>());


        switch (_gameMode)
        {
            case GameMode.Match:
                ServiceLocator.Register<ITurnSystem>(FindAnyObjectByType<TurnManager>());
                ServiceLocator.Register<ICommunicationSystem>(FindAnyObjectByType<GameNetwork>());
                break;
            
            case GameMode.Test:
                ServiceLocator.Register<ITurnSystem>(FindAnyObjectByType<TurnManager>());
                ServiceLocator.Register<ICommunicationSystem>(FindAnyObjectByType<TestModeCommunications>());
                break;
            
            case GameMode.Tutorial:
                ServiceLocator.Register<ITurnSystem>(FindAnyObjectByType<TutorialManager>());
                ServiceLocator.Register<ICommunicationSystem>(FindAnyObjectByType<TutorialManager>());
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    private IEnumerator Start()
    {
        yield return null;
        ServiceLocator.Get<ITurnSystem>().StartGame();
    }
}