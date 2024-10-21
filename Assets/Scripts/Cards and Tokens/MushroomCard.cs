using System;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Mushroom Card")]
public class MushroomCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => false;
    public override ICard.Card CardType => ICard.Card.Mushroom;
    public override IEnumerable<ICard.Population> GetPopulations() => null;
}
