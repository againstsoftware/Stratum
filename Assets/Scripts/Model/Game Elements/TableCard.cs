public class TableCard
{
    public readonly ACard Card;
    public int TurnsAlive { get; private set; }
    public TableCard InfluenceCardOnTop { get; private set; }

    internal TableCard(ACard card)
    {
        Card = card;
    }

    internal void AdvanceTurn()
    {
        TurnsAlive++;
        InfluenceCardOnTop?.AdvanceTurn();
    }

    internal void PlaceInlfuenceCard(InfluenceCard card)
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