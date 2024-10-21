using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Population Card")]
public class PopulationCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => true;
    public override ICard.Card CardType => ICard.Card.Population;
    
    [SerializeField] private ICard.Population _populationType;

    private ICard.Population _secondaryType = ICard.Population.None;
    
    public bool AddSecondaryType(ICard.Population population)
    {
        if (_populationType == population || _secondaryType is not ICard.Population.None) return false;
        _secondaryType = population;
        return true;
    }
    public override IEnumerable<ICard.Population> GetPopulations() => _secondaryType is ICard.Population.None ? 
        new[] { _populationType } : 
        new[] { _populationType, _secondaryType };
    
}
