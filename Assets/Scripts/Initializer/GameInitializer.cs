using System;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _starter;
    [SerializeField] private Deck[] _decks;

    private void Awake()
    {
        var gameModel = new GameModel(_starter, Array.ConvertAll(_decks, d => d as IDeck));
        ServiceLocator.Register<IModel>(gameModel);
        
        ServiceLocator.Register<IInteractionSystem>(FindAnyObjectByType<InteractionManager>());
        
        ServiceLocator.Register<IRulesSystem>(new DummyRulesManager()); //de pega
        
        ServiceLocator.Register<IExecutor>(new EffectExecutor());
        
        ServiceLocator.Register<IView>(FindAnyObjectByType<ViewManager>());

    }
}