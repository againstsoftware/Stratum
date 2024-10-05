using System;
using System.Collections.Generic;

public class HandOfCards
{
    public IReadOnlyList<ACard> Cards => _cards;

    private readonly List<ACard> _cards = new();

    internal void AddCard(ACard card)
    {
        _cards.Add(card);
        if (_cards.Count > 5)
            throw new IndexOutOfRangeException("Error! Mano con + de 5 cartas.");
    }

    public ACard RemoveCard(int index)
    {
        var card = _cards[index];
        _cards.RemoveAt(index);
        return card;
    }
}