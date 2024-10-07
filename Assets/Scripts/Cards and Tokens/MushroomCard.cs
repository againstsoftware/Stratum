using System;
using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Population Card")]
public class MushroomCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => false;
    public override ICard.Card CardType => ICard.Card.Mushroom;
    public override ICard.Population[] GetPopulations() => null;
}
