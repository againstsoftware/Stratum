using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Influence Card")]
public abstract class AInfluenceCard : ACard
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
        Compost,
        Pesticide
    }
    [field: SerializeField] public Type InfluenceType { get; private set; }
    [field: SerializeField]public bool IsPersistent { get; private set; }
    public override bool CanHaveInfluenceCardOnTop => false;


    protected override ACardRulesComponent _cardRC => GetRulesComponent().Init(this);

    protected abstract AInfluenceCardRulesComponent GetRulesComponent();


}