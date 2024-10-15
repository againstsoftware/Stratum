using System.Collections.Generic;

public class Slot
{
    public readonly Territory Territory;
    public readonly int SlotIndex;
    public IReadOnlyList<TableCard> PlacedCards => _placedCards;
    private readonly List<TableCard> _placedCards = new();


    public Slot(Territory territory, int slotIndex)
    {
        Territory = territory;
        SlotIndex = slotIndex;   
    }    
    internal TableCard PlaceCard(ICard card, bool atTheBottom = false)
    {
        var tableCard = new TableCard(card);

        if (atTheBottom) _placedCards.Insert(0, tableCard);
        else _placedCards.Add(tableCard);

        tableCard.Slot = this;
        tableCard.SlotIndex = _placedCards.IndexOf(tableCard);
        return tableCard;
    }

    internal void MoveCard(TableCard card)
    {
        _placedCards.Add(card);
        card.Slot = this;
        card.SlotIndex = _placedCards.Count - 1;
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