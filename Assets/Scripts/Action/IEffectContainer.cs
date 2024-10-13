using System;
using System.Collections.Generic;


public interface IEffectContainer
{
    public IReadOnlyList<Effect> Effects { get; }
}


public enum Effect
{
    PlacePopulationCard,
    GrowHerbivore,
    GrowCarnivore,
    KillHerbivore,
    KillCarnivore,
    Discard,
    Draw2,
    Draw5
}

