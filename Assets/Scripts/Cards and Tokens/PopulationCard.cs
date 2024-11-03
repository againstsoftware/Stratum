using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Population Card")]
public class PopulationCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => true;
    public override ICard.Card CardType => ICard.Card.Population;
    
    [SerializeField] private ICard.Population _populationType;

    public override ICard.Population PopulationType => _populationType;

}
