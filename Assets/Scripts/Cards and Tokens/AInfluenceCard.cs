using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Influence Card")]
public abstract class AInfluenceCard : ACard
{
    [field: SerializeField]public bool IsPersistent { get; private set; }
    public override bool CanHaveInfluenceCardOnTop => false;


    protected override ACardRulesComponent _cardRC => GetRulesComponent().Init(this);

    protected abstract AInfluenceCardRulesComponent GetRulesComponent();


}