using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Token")]
public class Token : AActionItem, IEffectContainer
{
    public string Name { get => _name.GetLocalizedString(); }
    public string Description { get => _description.GetLocalizedString(); }
    
    [SerializeField] private LocalizedString _name, _description;

    [field: SerializeField] public override ValidAction[] ValidActions { get; protected set; } = 
        { new ValidAction(ValidDropLocation.DiscardPile) };
    
    [SerializeField] private Effect[] _effects;
    public IReadOnlyList<Effect> Effects => _effects;
}