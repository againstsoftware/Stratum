using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

// [CreateAssetMenu(menuName = "Token")]
public abstract class AToken : AActionItem
{
    public string Name { get => LocalizationGod.GetLocalized("Cards", _tokenName); }
    public string Description { get => LocalizationGod.GetLocalized("Cards", _tokenDescription); }
    
    [FormerlySerializedAs("_newName")] [SerializeField] private string _tokenName;
    [FormerlySerializedAs("_newDescription")] [SerializeField] private string _tokenDescription;

    [SerializeField] public ValidAction[] ValidActions;
    
    [SerializeField] private Effect[] _effects;
    public override IEnumerable<ValidAction> GetValidActions() => ValidActions;

    


    public override IEnumerable<Effect> GetEffects(int index)
    {
        if (index != 0) throw new Exception($"token solo tiene 1 secuencia de efectos!!! no {index}!");
        return _effects;
    }
}