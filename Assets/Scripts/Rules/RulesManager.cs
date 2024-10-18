using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;

public class RulesManager : IRulesSystem
{
    public bool IsValidAction(PlayerAction action)
    {   
        if(action.ActionItem is ICard)
        {
            ICard playedCard = action.ActionItem as ICard;
            ICard modelCard = ServiceLocator.Get<IModel>().GetPlayer(action.Actor).HandOfCards.Cards[action.CardIndexInHand];
            
            // tiene en la mano la carta
            if(playedCard != modelCard)
            {
                return false;
            }

            // es el jugador del turno actual
            if(action.Actor != ServiceLocator.Get<IModel>().PlayerOnTurn)
            {
                return false;
            }

            // se ha jugado en slot vacio
            if(ServiceLocator.Get<IModel>().GetPlayer(action.Actor).Territory.Slots[action.Receivers[0].Index].PlacedCards.Count != 0)
            {
                return false;
            }

            // no tiene mas de 5 cartas (este no se si hace falta)
            if(ServiceLocator.Get<IModel>().GetPlayer(action.Actor).HandOfCards.Cards.Count > 5)
            {
                return false;
            }

            // REGLAS SI ES DE POBLACION 
            if(playedCard.CardType is ICard.Card.Population)
            {

            }

            // REGLAS SI INFLUENCIA
            if(playedCard.CardType is ICard.Card.Influence)
            {

            }

            // MUSHROOM
            if(playedCard.CardType is ICard.Card.Mushroom)
            {
                return false;
            }

            // MACROFUNGI
            if(playedCard.CardType is ICard.Card.Macrofungi)
            {
                return false;
            }
        }

        // TOKEN
        if(action.ActionItem is Token)
        {
            // construccion
            if(action.Actor == PlayerCharacter.Overlord)
            {
                if(action.Receivers.Count != 1)
                {
                    return false;
                }

                // comprobar si es territorio
                if(action.Receivers[0].Location != ValidDropLocation.AnyTerritory)
                {
                    return false;
                }

                // comprobar si territorio ya construido
                if(ServiceLocator.Get<IModel>().GetPlayer(action.Receivers[0].LocationOwner).Territory.HasConstruction)
                {
                    return false;
                }

                // comprobar si hay algun carnivoro y 2 plantas
                int herbCount = 0;
                foreach(var slot in ServiceLocator.Get<IModel>().GetPlayer(action.Receivers[0].LocationOwner).Territory.Slots)
                {
                    foreach(var placedCard in slot.PlacedCards)
                    {
                        // hay carnivoros
                        if(placedCard.Card.CardType is (ICard.Card)ICard.Population.Carnivore)
                        {
                            return false;
                        }
                        // hay dos herbivoros
                        if(placedCard.Card.CardType is (ICard.Card)ICard.Population.Herbivore)
                        {
                            herbCount++;
                        }
                    }
                }
                // 2 plantas al menos
                if(herbCount < 2)
                {
                    return false;
                } 
            }
            // macrohongo
            else if(action.Actor == PlayerCharacter.Fungaloth)
            {
                // comprobar receivers 3 elementos
                if(action.Receivers.Count != 3)
                {
                    return false;
                }
                
                int mushroomNum = 0;
                for(int i = 0;i < action.Receivers.Count; i++)
                {
                    Slot slot = ServiceLocator.Get<IModel>().GetPlayer(action.Receivers[i].LocationOwner).Territory.Slots[action.Receivers[i].Index];
                    foreach(var placedCard in slot.PlacedCards)
                    {
                        if(placedCard.Card.CardType is ICard.Card.Mushroom)
                        {
                            mushroomNum++;
                        }
                    }
                }
                if(mushroomNum < 3)
                {
                    return false;
                }
            }
            
            else
            {
                return false;
            }
        }

        return true;

    }


    // rpc
    public void PerformAction(PlayerAction action)
    {
    }


    private void CheckEcosystemRules()
    {
        // new EffectsList
        
        IReadOnlyList<TableCard> plants = ServiceLocator.Get<IModel>().Ecosystem.Plants;
        IReadOnlyList<TableCard> herbivores = ServiceLocator.Get<IModel>().Ecosystem.Herbivores;
        IReadOnlyList<TableCard> carnivores = ServiceLocator.Get<IModel>().Ecosystem.Carnivores;

        int plantsNum = plants.Count;
        int herbivoresNum = herbivores.Count;
        int carnivoresNum = carnivores.Count;

        bool herbivoresDeath = false;
        bool carnivoresDeath = false;

        // 1. Herbivores Death
        if(herbivoresNum < 1)
        {
            // no die
        }
        else if(plantsNum < 1)
        {
            herbivoresDeath = true;
            herbivoresNum--;
        }
        else if((herbivoresNum - plantsNum) >= 2)
        {
            herbivoresDeath = true;
            herbivoresNum--;
            // effect
        }
        else if((carnivoresNum - herbivoresNum) >= 2)
        {
            herbivoresDeath = true;
            herbivoresNum--;
            // effect
        }

        // 2. Herbivores Increase
        if(herbivoresDeath || herbivoresNum < 1 || plantsNum < 1)
        {
            // nada
        }
        else if(carnivoresNum < 1)
        {
            herbivoresNum++;
            //effects
        }
        else if((herbivoresNum - carnivoresNum) >= 2)
        {
            herbivoresNum++;
            //effects
        }
        else if((plantsNum - herbivoresNum) >= 2)
        {
            herbivoresNum++;
        }

        // 3. Carnivores Death
        if(carnivoresNum < 1)
        {
            // no ide
        }
        else if(herbivoresNum < 1)
        {
            carnivoresDeath = true;
            carnivoresNum--;
        }
        else if((carnivoresNum - herbivoresNum) >= 2)
        {
            carnivoresDeath = true;
            carnivoresNum--;
        }

        // 4. Carnivores Increase
        if(carnivoresDeath || carnivoresNum < 1)
        {
            //nada
        }
        else if((herbivoresNum - carnivoresNum) >= 2)
        {
            carnivoresNum++;
        }
    }

}
