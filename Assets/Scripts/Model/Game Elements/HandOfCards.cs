using System;
using System.Collections.Generic;

public class HandOfCards
{

    private readonly List<ACard> _cards = new();

    internal void AddCard(ACard card)
    {
        _cards.Add(card);
        if (_cards.Count > 5)
            throw new IndexOutOfRangeException("Error! Mano con + de 5 cartas.");
    }

    internal bool RemoveCard(ACard card) => _cards.Remove(card);

    public bool Contains(ACard card) => _cards.Contains(card);

    public int Count => _cards.Count;  
}