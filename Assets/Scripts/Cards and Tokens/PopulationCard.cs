using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Population Card")]
public class PopulationCard : ACard
{
    public override bool CanHaveInfluenceCardOnTop => true;
    
    [SerializeField] private Population _populationType;

    public Population PopulationType => _populationType;

    protected override bool CheckCardAction(PlayerAction action)
    {
        if (action.Receivers.Length != 1)
        {
            return false;
        }

        if (action.Receivers[0].Location is not ValidDropLocation.OwnerSlot)
        {
            Debug.Log($"rechazada porque la carta de poblacion se jugo en {action.Receivers[0].Location}");
            return false;
        }

        var slotOwner = action.Receivers[0].LocationOwner;
        if (slotOwner != action.Actor)
        {
            Debug.Log("rechazada porque el slot no es del que jugo la carta de poblacion!");
            return false;
        }

        if (ServiceLocator.Get<IModel>().GetPlayer(action.Actor).Territory.Slots[action.Receivers[0].Index]
                .PlacedCards.Count != 0)
        {
            Debug.Log("rechazada porque el slot donde se jugo la carta de poblacion no esta vacio");
            return false;
        }

        return true;
    }    
}

public enum Population { None, Carnivore, Herbivore, Plant}

