
public class MacrofungiTokenRC : IRulesComponent
{
    public bool CheckAction(PlayerAction action)
    {
        // comprobar receivers 3 elementos
        if (action.Receivers.Length != 3)
            return false;


        foreach (var receiver in action.Receivers)
        {
            Slot slot = ServiceLocator.Get<IModel>().GetPlayer(receiver.LocationOwner).Territory.Slots[receiver.Index];

            var placedCard = slot.PlacedCards[receiver.SecondIndex];
            if (placedCard.Card is not MushroomCard)
                return false;
        }

        return true;
    }
}
