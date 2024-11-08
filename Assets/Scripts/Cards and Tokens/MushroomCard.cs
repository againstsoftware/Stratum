using System;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Mushroom Card")]
public class MushroomCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => false;
    protected override ACardRulesComponent _cardRC => null;

    

}
