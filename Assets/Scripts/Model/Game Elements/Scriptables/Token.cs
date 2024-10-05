using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Token")]
public class Token : ScriptableObject, IActionItem
{
    public string Name { get => _name.GetLocalizedString(); }
    public string Description { get => _description.GetLocalizedString(); }
    
    [SerializeField] private LocalizedString _name, _description;

    [field: SerializeField] public ValidAction[] ValidActions { get; private set; } = 
        { new ValidAction(ValidDropLocation.DiscardPile) };


    
    
}