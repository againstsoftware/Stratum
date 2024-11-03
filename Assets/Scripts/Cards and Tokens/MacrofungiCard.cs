using System;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Macrofungi Cad")]
public class MacrofungiCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => false;
    public override ICard.Card CardType => ICard.Card.Macrofungi;
    public override ICard.Population PopulationType => ICard.Population.None;
}
