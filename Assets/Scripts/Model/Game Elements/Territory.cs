using System;

public class Territory
{
    public readonly PlayerCharacter Owner;
    public readonly Slot[] Slots;
    public event Action OnConstructionPlaced, OnConstructionRemoved;

    public bool HasConstruction
    {
        get => _hasConstruction;
        set
        {
            bool had = _hasConstruction;
            _hasConstruction = value;
            
            if(!had && _hasConstruction) OnConstructionPlaced?.Invoke();
            else if(had && !_hasConstruction) OnConstructionRemoved?.Invoke();
        }
    }
    private bool _hasConstruction;

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