using System;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Population Card")]
public class MacrofungiCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => false;
    public override ICard.Card CardType => ICard.Card.Macrofungi;
    public override IEnumerable<ICard.Population> GetPopulations() => null;
}
