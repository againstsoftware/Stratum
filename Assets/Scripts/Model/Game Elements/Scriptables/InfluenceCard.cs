using System;
using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Population Card")]
public class InfluenceCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => false;
}