
using System.Collections.Generic;
using System.Linq;
using PlasticGui.WorkspaceWindow;
using UnityEngine;

[CreateAssetMenu(menuName = "Deck")]
public class Deck : ScriptableObject
{
    [field:SerializeField] public PlayerCharacter Owner { get; private set; }

    [System.Serializable]
    public class CardAmount
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

    [SerializeField] private CardAmount[] Cards;


    private int _size;
    private bool _initialized;
    private ACard[] _deck;
    
    
    public ACard DrawCard()
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
