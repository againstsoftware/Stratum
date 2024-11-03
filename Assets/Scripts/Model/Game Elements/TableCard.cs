using System;
using System.Collections.Generic;

public class TableCard
{
    public readonly ICard Card;
    public int TurnsAlive { get; private set; }
    public TableCard InfluenceCardOnTop { get; private set; }
    public Slot Slot { get; internal set; }
    public int IndexInSlot { get; internal set; }
    public bool HasRabids { get; internal set; }

    public bool IsOmnivore { get; set; }

    public IEnumerable<ICard.Population> GetPopulations()
    {
        return IsOmnivore
            ? new[] { ICard.Population.Carnivore, ICard.Population.Herbivore }
            : new[] { Card.PopulationType };
    }


    internal TableCard(ICard card)
    {
        Card = card;
    }

    internal void AdvanceTurn()
    {
        TurnsAlive++;
        InfluenceCardOnTop?.AdvanceTurn();
    }

    internal void PlaceInlfuenceCard(ICard card)
    {
        InfluenceCardOnTop = new TableCard(card);
    }

    internal void RemoveInfluenceCard()
    {
        InfluenceCardOnTop.OnRemove();
        InfluenceCardOnTop = null;
    }

    internal void OnRemove()
    {
        InfluenceCardOnTop?.OnRemove();
        //...
    }
}