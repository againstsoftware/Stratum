using System;
using System.Collections.Generic;


public interface IEffectContainer
{
    public IEnumerable<Effect> GetEffects(int index);
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
    Draw5,
    OverviewSwitch,
    GrowMushroomEcosystem,
    GrowMacrofungi,
    Construct
}

