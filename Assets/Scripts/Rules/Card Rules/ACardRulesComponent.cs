using UnityEngine;

public abstract class ACardRulesComponent : IRulesComponent
{
    protected ICard _card;
    public void Init(ICard card)
    {
        _card = card;
    }
    
    public bool CheckAction(PlayerAction action)
    {
        if (!ServiceLocator.Get<IModel>().GetPlayer(action.Actor).HandOfCards.Contains(_card))
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

    protected abstract bool CheckCardAction(PlayerAction action);
}
