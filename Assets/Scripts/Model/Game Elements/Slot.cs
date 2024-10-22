using System.Collections.Generic;

public class Slot
{
    public readonly Territory Territory;
    public readonly int SlotIndexInTerritory;
    public IReadOnlyList<TableCard> PlacedCards => _placedCards;
    private readonly List<TableCard> _placedCards = new();


    public Slot(Territory territory, int slotIndexInTerritory)
    {
        Territory = territory;
        SlotIndexInTerritory = slotIndexInTerritory;   
    }    
    internal TableCard PlaceCard(ICard card, bool atTheBottom = false)
    {
        var tableCard = new TableCard(card);

        if (atTheBottom)
        {
            _placedCards.Insert(0, tableCard);
            int i = 0;
            foreach (var pc in _placedCards) pc.IndexInSlot = i++;
        }
        else _placedCards.Add(tableCard);

        tableCard.Slot = this;
        tableCard.IndexInSlot = _placedCards.IndexOf(tableCard);
        return tableCard;
    }

    internal void MoveCard(TableCard card)
    {
        _placedCards.Add(card);
        card.Slot = this;
        card.IndexInSlot = _placedCards.Count - 1;
    }

    internal void RemoveCard(TableCard card)
    {
        card.OnRemove();
        _placedCards.Remove(card);
        int i = 0;
        foreach (var pc in _placedCards) pc.IndexInSlot = i++;
    }

    internal void AdvanceTurn()
    {
        foreach (var card in _placedCards) card.AdvanceTurn();
    }
    
}