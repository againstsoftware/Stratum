using System;
using System.Collections.Generic;


public interface IEffectContainer
{
    public IEnumerable<Effect> GetEffects(int index);
}


public enum Effect
{
    PlacePopulationCardFromPlayer,
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
    Construct,
    PlaceInitialCards,
    PlayAndDiscardInfluenceCard,
    MovePopulationToEmptySlot,
    PlaceInfluenceOnPopulation,
    GiveRabies,
    DestroyAllInTerritory,
    DestroyNonFungiInTerritory
    
}

