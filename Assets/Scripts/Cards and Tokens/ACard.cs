using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;

public abstract class ACard : ScriptableObject, IRulebookEntry, IActionItem
{
    public string CardName { get => _name.GetLocalizedString(); }
    public string Description { get => _description.GetLocalizedString(); }
    
    [SerializeField] private LocalizedString _name, _description;
    [field: SerializeField] public ValidAction[] ValidActions { get; private set; }

    public string GetName() => CardName;
    public string GetDescription() => Description;
    
    
}
