using System;
using System.Collections.Generic;

public class HandOfCards
{

    private readonly List<ICard> _cards = new();

    internal void AddCard(ICard card)
    {
        _cards.Add(card);
        if (_cards.Count > 5)
            throw new IndexOutOfRangeException("Error! Mano con + de 5 cartas.");
    }

    internal bool RemoveCard(ICard card) => _cards.Remove(card);

    public bool Contains(ICard card) => _cards.Contains(card);
}