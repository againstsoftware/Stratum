using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Population Card")]
public class PopulationCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => true;
    
    [SerializeField] private Population _populationType;

    public Population PopulationType => _populationType;

    protected override ACardRulesComponent _cardRC => new PopulationCardRulesComponent().Init(this);
    
}

public enum Population { None, Carnivore, Herbivore, Plant}

