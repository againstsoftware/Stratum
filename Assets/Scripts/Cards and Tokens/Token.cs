using System;
using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Token")]
public class Token : AActionItem, IEffectContainer
{
    public string Name { get => _name.GetLocalizedString(); }
    public string Description { get => _description.GetLocalizedString(); }
    
    [SerializeField] private LocalizedString _name, _description;

    [SerializeField] public ValidAction[] ValidActions;
    
    [SerializeField] private Effect[] _effects;
    public override IEnumerable<ValidAction> GetValidActions() => ValidActions;

    public IEnumerable<Effect> GetEffects(int index)
    {
        if (index != 0) throw new Exception($"token solo tiene 1 secuencia de efectos!!! no {index}!");
        return _effects;
    }
}