using System.Collections.Generic;

public class Slot
{
    public IReadOnlyList<TableCard> PlacedCards => _placedCards;
    private readonly List<TableCard> _placedCards = new();

    internal TableCard PlaceCard(ICard card, bool atTheBottom = false)
    {
        var tableCard = new TableCard(card);

        if (atTheBottom) _placedCards.Insert(0, tableCard);
        else _placedCards.Add(tableCard);

        return tableCard;
    }

    internal void MoveCard(TableCard card)
    {
        _placedCards.Add(card);
    }

    internal void RemoveCard(TableCard card)
    {
        card.OnRemove();
        _placedCards.Remove(card);
    }

    internal void AdvanceTurn()
    {
        foreach (var card in _placedCards) card.AdvanceTurn();
    }
}