using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Influence Card")]
public abstract class AInfluenceCard : ACard
{
    [field: SerializeField]public bool IsPersistent { get; private set; }
    public override bool CanHaveInfluenceCardOnTop => false;


    protected override bool CheckCardAction(PlayerAction action)
    {
        if (ServiceLocator.Get<IModel>().GetPlayer(action.Actor).InfluencePlayed)
        {
            Debug.Log("rechazada porque ya jugo influencia este turno");
            return false;
        }

        return CheckInfluenceCardAction(action);
    }

    protected abstract bool CheckInfluenceCardAction(PlayerAction action);
    
    
    protected static bool ArePlayersOpposites(PlayerCharacter a, PlayerCharacter b)
    {
        var turnOrder = RulesCheck.Config.TurnOrder.ToList();
        turnOrder.Remove(PlayerCharacter.None);

        var aIndex = turnOrder.IndexOf(a);
        var bIndex = turnOrder.IndexOf(b);

        return (aIndex == 0 && bIndex == 2) || (aIndex == 2 && bIndex == 0) ||
               (aIndex == 1 && bIndex == 3) || (aIndex == 3 && bIndex == 1);
    }
    protected static bool ExistsFungiOnTerritory(Territory territory)
    {
        return territory.Slots.SelectMany(slot => slot.PlacedCards)
            .Any(card => card.Card is MushroomCard or MacrofungiCard);
    }


}