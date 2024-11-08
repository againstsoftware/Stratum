
using System.Linq;

public class CompostRC : AInfluenceCardRulesComponent
{
    protected override bool CheckInfluenceCardAction(PlayerAction action)
    {
        if (action.Receivers.Length != 1)
        {
            return false;
        }

        var receiver = action.Receivers[0];

        if (receiver.Location != ValidDropLocation.AnySlot)
        {
            return false;
        }

        if (receiver.Index is < 0 or >= 5)
        {
            return false;
        }
        
        var slotOwner = ServiceLocator.Get<IModel>().GetPlayer(receiver.LocationOwner);
        var slot = slotOwner.Territory.Slots[receiver.Index];
        if (slot.PlacedCards.Any())
        {
            return false;
        }

        return true;
    }
}
