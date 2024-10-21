
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Deck")]
public class Deck : ScriptableObject, IDeck
{
    [field:SerializeField] public PlayerCharacter Owner { get; private set; }

    [System.Serializable]
    private class CardAmount
    {
        public ACard Card;
        public int Amount;
    }
    
    public int Size
    {
        get
        {
            if (!_initialized) Initialize();
            return _size;
        }
    }
    
    
    //para cartas que NO esten en el mazo
    public ICard Mushroom => _mushroom;
    public ICard Macrofungi => _macrofungi;

    [SerializeField] private CardAmount[] Cards;

    [SerializeField] private MushroomCard _mushroom;
    [SerializeField] private MacrofungiCard _macrofungi;
    
    private int _size;
    private bool _initialized;
    private ACard[] _deck;
    
    
    public ICard DrawCard()
    {
        if (!_initialized) Initialize();
        return _deck[Random.Range(0, _size)];
    }

    private void Initialize()
    {
        _initialized = true;
        _size = Cards.Aggregate(0, (total, current) => total + current.Amount);
        _deck = new ACard[_size];
        int i = 0;
        foreach (var c in Cards)
        {
            for (int j = 0; j < c.Amount; j++)
            {
                _deck[i] = c.Card;
                i++;
            }
        }
    }
}
