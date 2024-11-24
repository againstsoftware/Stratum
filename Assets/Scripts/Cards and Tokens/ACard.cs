using System;
using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;

public abstract class ACard : AActionItem
{
    public string Name
    {
        get
        {
            if (_name is null)
            {
                Debug.LogWarning($"CARTA {name} CON NOMBRE NULL");
                return "UNNAMED";
            }
            var loc = _name.GetLocalizedString();
            return loc is null ? "UNNAMED" : loc;
        }
    }

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
    
    public override bool CheckAction(PlayerAction action)
    {
        var p = ServiceLocator.Get<IModel>().GetPlayer(action.Actor);
        if (!p.HandOfCards.Contains(this))
        {
            Debug.Log($"rechazada porque la carta no esta en la mano del model");
            return false;
        }

        //si es accion de descarte
        if (action.Receivers.Length == 1 && action.Receivers[0].Location is ValidDropLocation.DiscardPile)
        {
            var owner = action.Receivers[0].LocationOwner;
            if (owner == action.Actor) return true;
            Debug.Log("rechazada porque la pila de descarte no es del que jugo la carta!");
            return false;
        }

        return CheckCardAction(action);

    }

    protected virtual bool CheckCardAction(PlayerAction action) => false;

}
