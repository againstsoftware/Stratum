using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;

public abstract class ACard : AActionItem, ICard, IEffectContainer
{
    public string Name { get => _name.GetLocalizedString(); }
    public string Description { get => _description.GetLocalizedString(); }
    
    public abstract bool CanHaveInfluenceCardOnTop { get; }
    
    [SerializeField] private LocalizedString _name, _description;

    [field: SerializeField] public override ValidAction[] ValidActions { get; protected set; } = 
        { new ValidAction(ValidDropLocation.DiscardPile) };


    [SerializeField] private Effect[] _effects;
    public IReadOnlyList<Effect> Effects => _effects;
    
    public abstract ICard.Card CardType { get; }
    public abstract ICard.Population[] GetPopulations();





}
