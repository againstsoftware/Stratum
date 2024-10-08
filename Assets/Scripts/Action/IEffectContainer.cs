using System;
using System.Collections.Generic;


public interface IEffectContainer
{
    public IReadOnlyList<Effect> Effects { get; }
}


public enum Effect
{
    PlacePopulationCard,
}

