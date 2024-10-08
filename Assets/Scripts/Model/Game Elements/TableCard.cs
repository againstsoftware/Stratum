public class TableCard
{
    public readonly ICard Card;
    public int TurnsAlive { get; private set; }
    public TableCard InfluenceCardOnTop { get; private set; }

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