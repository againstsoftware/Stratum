using System;
using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Population Card")]
public class MushroomCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => false;
}
