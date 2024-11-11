using System.Linq;

public class Rabies : AInfluenceCard
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
        var cardOwnerPlacedCards = cardOwner.Territory.Slots[receiver.Index].PlacedCards;

        if (receiver.SecondIndex < 0 || receiver.SecondIndex >= cardOwnerPlacedCards.Count)
        {
            return false;
        }

        var card = cardOwnerPlacedCards[receiver.SecondIndex];

        if (card.InfluenceCardOnTop is not null)
        {
            return false;
        }

        if (card.HasRabids)
        {
            return false;
        }

        if (!card.GetPopulations().Contains(Population.Herbivore))
        {
            return false;
        }

        if (!card.Card.CanHaveInfluenceCardOnTop)
        {
            return false;
        }

        return true;
    }
}