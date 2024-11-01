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
        AppetizingMushroom
    }
    [field: SerializeField] public Type InfluenceType { get; private set; }
    public override bool CanHaveInfluenceCardOnTop => false;
    public override ICard.Card CardType => ICard.Card.Influence;
    public override IEnumerable<ICard.Population> GetPopulations() => null;

}