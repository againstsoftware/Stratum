using System;
using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Population Card")]
public class InfluenceCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => false;
    public override ICard.Card CardType => ICard.Card.Influence;
    public override ICard.Population[] GetPopulations() => null;

}