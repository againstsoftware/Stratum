
using System.Linq;

public class MigrationRC : AInfluenceCardRulesComponent
{
    protected override bool CheckInfluenceCardAction(PlayerAction action)
    {
        var receivers = action.Receivers;

        if (receivers.Length != 2)
        {
            return false;
        }


        if (receivers[0].Location is not ValidDropLocation.OwnerCard)
        {
            return false;
        }


        var player = ServiceLocator.Get<IModel>().GetPlayer(action.Actor);
        if (receivers[0].Index is < 0 or >= 5)
        {
            return false;
        }

        var modelCards = player.Territory.Slots[receivers[0].Index].PlacedCards;

        if (receivers[0].SecondIndex < 0 || receivers[0].SecondIndex >= modelCards.Count)
        {
            return false;
        }

        var card = modelCards[receivers[0].SecondIndex];

        if (card.Card.CardType is not ICard.Card.Population)
        {
            return false;
        }


        if (!card.GetPopulations().Contains(ICard.Population.Carnivore) &&
            !card.GetPopulations().Contains(ICard.Population.Herbivore))
        {
            return false;
        }


        if (receivers[1].LocationOwner == action.Actor)
        {
            return false;
        }

        if (receivers[1].Location is not ValidDropLocation.AnySlot)
        {
            return false;
        }

        if (receivers[1].Index is < 0 or >= 5)
        {
            return false;
        }

        var slotOwner = ServiceLocator.Get<IModel>().GetPlayer(receivers[1].LocationOwner);
        var slot = slotOwner.Territory.Slots[receivers[1].Index];
        if (slot.PlacedCards.Count > 0)
        {
            return false;
        }

        return true;
    }
}
