
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PesticideRC : AInfluenceCardRulesComponent
{
    protected override bool CheckInfluenceCardAction(PlayerAction action)
    {
        if (action.Receivers.Length != 1)
        {
            return false;
        }

        var receiver = action.Receivers[0];

        if (receiver.Location != ValidDropLocation.AnyCard)
        {
            return false;
        }

        if (receiver.Index is < 0 or >= 5)
        {
            return false;
        }

        var cardOwner = ServiceLocator.Get<IModel>().GetPlayer(receiver.LocationOwner);
        var slotCards = cardOwner.Territory.Slots[receiver.Index].PlacedCards;

        if (receiver.SecondIndex < 0 || receiver.SecondIndex >= slotCards.Count)
        {
            return false;
        }

        var card = slotCards[receiver.SecondIndex];

        if (!card.GetPopulations().Contains(Population.Plant))
        {
            return false;
        }

        return true;
    }
}
