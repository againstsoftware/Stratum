using System;
using System.Collections.Generic;

public class HandOfCards
{
    public IReadOnlyList<ICard> Cards => _cards;

    private readonly List<ICard> _cards = new();

    internal void AddCard(ICard card)
    {
        _cards.Add(card);
        if (_cards.Count > 5)
            throw new IndexOutOfRangeException("Error! Mano con + de 5 cartas.");
    }

    internal ICard RemoveCard(int index)
    {
        var card = _cards[index];
        _cards.RemoveAt(index);
        return card;
    }
}