using System;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Macrofungi Cad")]
public class MacrofungiCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => false;
    protected override ACardRulesComponent _cardRC => null;

}
