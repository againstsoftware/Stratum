
using System;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _starter;
    [SerializeField] private Deck[] _decks;
    private void Awake()
    {
        ServiceLocator.Register<IModel>(new GameModel(_starter, _decks));
    }
}
