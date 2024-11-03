using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Influence Card")]
public class InfluenceCard : ACard
{
    public enum Type
    {
        None,
        Migration,
        PheromoneFragance,
        Fireworks,
        AppetizingMushroom,
        Rabies,
        Wildfire,
        Arson,
        ExplosiveSpores,
        Mold,
        Omnivore,
        Compost
    }
    [field: SerializeField] public Type InfluenceType { get; private set; }
    [field: SerializeField]public bool IsPersistent { get; private set; }
    public override bool CanHaveInfluenceCardOnTop => false;
    public override ICard.Card CardType => ICard.Card.Influence;
    public override ICard.Population PopulationType => ICard.Population.None;

}