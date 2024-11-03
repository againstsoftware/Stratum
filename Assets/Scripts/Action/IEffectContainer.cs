using System;
using System.Collections.Generic;


public interface IEffectContainer
{
    public IEnumerable<Effect> GetEffects(int index);
}


public enum Effect
{
    PlacePopulationCardFromPlayer,
    GrowHerbivoreEcosystem,
    GrowCarnivoreEcosystem,
    KillHerbivoreEcosystem,
    KillCarnivoreEcosystem,
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
    DestroyNonFungiInTerritory,
    GrowMushroom,
    GrowMushroomEndOfAction,
    MakeOmnivore,
    GrowPlant,
    GrowPlantEndOfAction,
    
    
}

