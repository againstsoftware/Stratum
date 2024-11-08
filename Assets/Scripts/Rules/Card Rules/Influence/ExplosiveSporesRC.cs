
using System.Linq;

public class ExplosiveSporesRC :AInfluenceCardRulesComponent
{
    protected override bool CheckInfluenceCardAction(PlayerAction action)
    {
        if (action.Receivers.Length != 1)
        {
            return false;
        }

        var receiver = action.Receivers[0];

        if (receiver.Location != ValidDropLocation.AnyTerritory)
        {
            return false;
        }

        var playerOwner = ServiceLocator.Get<IModel>().GetPlayer(receiver.LocationOwner);

        return playerOwner.Territory.Slots.SelectMany(slot => slot.PlacedCards)
            .Any(card => card.Card is MushroomCard or MacrofungiCard);
    }
}
