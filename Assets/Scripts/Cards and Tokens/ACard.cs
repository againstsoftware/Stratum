using System;
using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;

public abstract class ACard : AActionItem
{
    public string Name { get => _name.GetLocalizedString(); }
    public string Description { get => _description.GetLocalizedString(); }
    
    public abstract bool CanHaveInfluenceCardOnTop { get; }
    
    [field:SerializeField] public Texture ObverseTex { get; private set; }
    
    
    [SerializeField] private LocalizedString _name, _description;
    

    [Serializable]
    public class ActionEffect
    {
        public ValidAction ValidAction;
        public Effect[] Effects;
    }

    [SerializeField] private ActionEffect[] _actionEffects;


    public override IEnumerable<Effect> GetEffects(int index) => _actionEffects[index].Effects;
    

    public override IEnumerable<ValidAction> GetValidActions()
    {
        List<ValidAction> validActions = new();
        int i = 0;
        foreach (var ae in _actionEffects)
        {
            ae.ValidAction.Index = i++;
            validActions.Add(ae.ValidAction);
        }
        return validActions;
    }


    public override IRulesComponent RulesComponent => _cardRC;

    protected abstract ACardRulesComponent _cardRC { get; }

}
