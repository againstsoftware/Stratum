using UnityEngine;
using UnityEngine.Localization;

public abstract class ACard : ScriptableObject, IRulebookEntry
{
    public string CardName { get => _name.GetLocalizedString(); }
    public string Description { get => _description.GetLocalizedString(); }
    
    [SerializeField] private LocalizedString _name, _description;

    public string GetName() => CardName;
    public string GetDescription() => Description;
}
