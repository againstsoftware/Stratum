using System.Linq;


public class ConstructionTokenRC : IRulesComponent
{
    public bool CheckAction(PlayerAction action)
    {
        var receivers = action.Receivers;

        if (receivers.Length != 1) return false;

        var owner = ServiceLocator.Get<IModel>().GetPlayer(receivers[0].LocationOwner);

        if (owner is null ||
            receivers[0].Location != ValidDropLocation.AnyTerritory ||
            owner.Territory.HasConstruction)
            return false;


        // comprobar si hay algun carnivoro y 2 o mas plantas
        int plants = 0;
        foreach (var slot in owner.Territory.Slots)
        {
            foreach (var placedCard in slot.PlacedCards)
            {
                if (placedCard.Card is not PopulationCard) continue;

                if (placedCard.GetPopulations().Contains(Population.Carnivore) ||
                    placedCard.HasRabids)
                    return false;

                if (placedCard.GetPopulations().Contains(Population.Plant))
                    plants++;
            }
        }

        // 2 plantas al menos
        return plants >= 2;
    }
}
