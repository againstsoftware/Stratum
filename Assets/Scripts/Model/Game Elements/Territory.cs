public class Territory
{
    public readonly PlayerCharacter Owner;
    public readonly Slot[] Slots;
    public bool HasConstruction { get; internal set; }

    internal Territory(PlayerCharacter owner)
    {
        Owner = owner;
        Slots = new Slot[5];
        for (int i = 0; i < 5; i++) Slots[i] = new Slot(this, i);
    }

    internal void AdvanceTurn()
    {
        foreach (var slot in Slots) slot.AdvanceTurn();
    }
}