using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class RulesCheck
{
    public static bool CheckAction(PlayerAction action)
    {
        Debug.Log($@"
        recibida player action para checkear:
        ------------------------------------------------
        {action.Actor}
        {action.ActionItem.name}
        Receivers:
        {string.Join("\n    ", action.Receivers
            .Select(r => $"{r.Location} - {r.LocationOwner} ->idx: {r.Index} ->idx2: {r.SecondIndex}"))}
        effects idx: {action.EffectsIndex}
        ------------------------------------------------
        ");

        // es el jugador del turno actual
        if (action.Actor != ServiceLocator.Get<IModel>().PlayerOnTurn)
        {
            Debug.Log($"rechazada porque le toca a {ServiceLocator.Get<IModel>().PlayerOnTurn}");
            return false;
        }


        switch (action.ActionItem)
        {
            //carta
            case ICard playedCard:
            {
                // tiene en la mano la carta
                if (!ServiceLocator.Get<IModel>().GetPlayer(action.Actor).HandOfCards.Contains(playedCard))
                {
                    Debug.Log($"rechazada porque la carta no esta en la mano del model");
                    return false;
                }

                return playedCard.CardType switch
                {
                    // REGLAS SI ES DE POBLACION 
                    ICard.Card.Population => CheckPopulationCardAction(action),
                    // REGLAS SI INFLUENCIA
                    ICard.Card.Influence => CheckInfluenceCardAction(action),
                    _ => false
                };
            }

            // construccion
            case Token when action.Actor == PlayerCharacter.Overlord:
                return CheckConstructionAction(action);

            // macrohongo
            case Token when action.Actor == PlayerCharacter.Fungaloth:
                return CheckMacrofungiAction(action);

            default: return false;
        }
    }


    public static IEnumerable<Effect> CheckEcosystem()
    {
        // new EffectsList

        IReadOnlyList<TableCard> plants = ServiceLocator.Get<IModel>().Ecosystem.Plants;
        IReadOnlyList<TableCard> herbivores = ServiceLocator.Get<IModel>().Ecosystem.Herbivores;
        IReadOnlyList<TableCard> carnivores = ServiceLocator.Get<IModel>().Ecosystem.Carnivores;

        int plantsNum = plants.Count;
        int herbivoresNum = herbivores.Count;
        int carnivoresNum = carnivores.Count;

        bool herbivoresDeath, herbivoresGrow;
        bool carnivoresDeath, carnivoresGrow;

        // 1. Herbivores Death
        if (herbivoresNum == 0) // Tiene que haber al menos una carta de herbívoro
        {
            // no die
            herbivoresDeath = false;
        }
        else if (plantsNum == 0) // No hay cartas de planta en la mesa.
        {
            herbivoresDeath = true;
            herbivoresNum--;
        }
        else if (herbivoresNum - plantsNum >= 2) // Debe haber al menos 2 cartas más de herbívoros que de plantas.
        {
            herbivoresDeath = true;
            herbivoresNum--;
        }
        else if (carnivoresNum - herbivoresNum >= 2) // Debe haber al menos 2 cartas mas de carnívoros que de herbiboros
        {
            herbivoresDeath = true;
            herbivoresNum--;
        }
        else herbivoresDeath = false;

        // 2. Herbivores Grow
        if (herbivoresDeath || //No puede haber muerto ninguna carta de herbívoro
            herbivoresNum == 0 || //Tiene que haber al menos una carta de herbívoro.
            plantsNum == 0) //Tiene que haber al menos una carta de planta.
        {
            herbivoresGrow = false;
        }
        else if (carnivoresNum == 0) //No hay cartas de carnívoro en la mesa.
        {
            herbivoresGrow = true;
            herbivoresNum++;
        }
        else if (herbivoresNum - carnivoresNum >= 2) //Debe haber al menos 2 cartas más de herbívoros que de carnívoros.
        {
            herbivoresGrow = true;
            herbivoresNum++;
        }
        else if (plantsNum - herbivoresNum >= 2) //Debe haber al menos 2 cartas más de plantas que de herbívoros.
        {
            herbivoresGrow = true;
            herbivoresNum++;
        }
        else herbivoresGrow = false;

        // 3. Carnivores Death
        if (carnivoresNum == 0) //Tiene que haber al menos una carta de carnívoro.
        {
            carnivoresDeath = false;
        }
        else if (herbivoresNum == 0) //No hay cartas de herbívoros en la mesa.
        {
            carnivoresDeath = true;
            carnivoresNum--;
        }
        else if (carnivoresNum - herbivoresNum >= 2) //Debe haber al menos 2 cartas más de carnívoros que de herbívoros.
        {
            carnivoresDeath = true;
            carnivoresNum--;
        }
        else carnivoresDeath = false;

        // 4. Carnivores Grow
        if (carnivoresDeath || //No puede haber muerto ningún carnívoro en la condición anterior.
            carnivoresNum == 0) //Tiene que haber al menos una carta de carnívoro.
        {
            carnivoresGrow = false;
        }
        else if (herbivoresNum - carnivoresNum >= 2) //Debe haber al menos 2 cartas más de herbívoros que de carnívoros.
        {
            carnivoresGrow = true;
            carnivoresNum++;
        }
        else carnivoresGrow = false;

        List<Effect> effects = new() { Effect.OverviewSwitch };

        if (herbivoresDeath)
        {
            effects.Add(Effect.KillHerbivore);
            effects.Add(Effect.GrowMushroomEcosystem);
        }
        else if (herbivoresGrow)
        {
            effects.Add(Effect.GrowHerbivore);
        }

        if (carnivoresDeath)
        {
            effects.Add(Effect.KillCarnivore);
            effects.Add(Effect.GrowMushroomEcosystem);
        }
        else if (carnivoresGrow)
        {
            effects.Add(Effect.GrowCarnivore);
        }

        return effects;
    }


    public static bool CheckRoundEnd(out IEnumerable<Effect> effects)
    {
        effects = null;
        return false;
    }


    private static bool CheckPopulationCardAction(PlayerAction action)
    {
        switch (action.Receivers[0].Location)
        {
            case ValidDropLocation.OwnerSlot:
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

            case ValidDropLocation.DiscardPile:
                var owner = action.Receivers[0].LocationOwner;
                if (owner != action.Actor)
                {
                    Debug.Log("rechazada porque la pila de descarte no es del que jugo la carta de poblacion!");
                    return false;
                }

                return true;

            default:
                Debug.Log($"rechazada porque la carta de poblacion se jugo en {action.Receivers[0].Location}");
                return false;
        }
    }

    private static bool CheckInfluenceCardAction(PlayerAction action)
    {
        return true;
    }

    private static bool CheckConstructionAction(PlayerAction action)
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
                if (placedCard.Card.CardType is not ICard.Card.Population) continue;
                
                if (placedCard.Card.GetPopulations().Contains(ICard.Population.Carnivore))
                    return false;

                if (placedCard.Card.GetPopulations().Contains(ICard.Population.Plant))
                    plants++;
            }
        }

        // 2 plantas al menos
        return plants >= 2;
    }

    private static bool CheckMacrofungiAction(PlayerAction action)
    {
        // comprobar receivers 3 elementos
        if (action.Receivers.Length != 3)
            return false;


        foreach (var receiver in action.Receivers)
        {
            Slot slot = ServiceLocator.Get<IModel>().GetPlayer(receiver.LocationOwner).Territory.Slots[receiver.Index];

            var placedCard = slot.PlacedCards[receiver.SecondIndex];
            if (placedCard.Card.CardType is not ICard.Card.Mushroom)
                return false;
        }

        return true;
    }
}