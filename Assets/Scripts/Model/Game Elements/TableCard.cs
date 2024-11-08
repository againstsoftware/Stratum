using System;
using System.Collections.Generic;

public class TableCard
{
    public readonly ACard Card;
    public int TurnsAlive { get; private set; }
    public TableCard InfluenceCardOnTop { get; private set; }
    public Slot Slot { get; internal set; }
    public int IndexInSlot { get; internal set; }
    public bool HasRabids { get; internal set; }

    public bool IsOmnivore { get; set; }

    private PopulationCard _populationCard;

    public IEnumerable<Population> GetPopulations()
    {
        if (_populationCard is null) return new[]{Population.None};
        return IsOmnivore
            ? new[] { Population.Carnivore, Population.Herbivore }
            : new[] { _populationCard.PopulationType };
    }


    internal TableCard(ACard card)
    {
        Card = card;
        if (Card is PopulationCard pc) _populationCard = pc;
    }

    internal void AdvanceTurn()
    {
        TurnsAlive++;
        InfluenceCardOnTop?.AdvanceTurn();
    }

    internal void PlaceInlfuenceCard(ACard card)
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