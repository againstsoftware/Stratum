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

    

    // TODO ESTO DE REGLAS ESTÁ MUY GUARRO SON PRUEBAS
    public void CheckEcosystemRules(List<TableCard> plants, List<TableCard> herbivorous, List<TableCard> carnivorous)
    {
        // esto es una guarrería lo hago para probar cosas de lógica. Habría que cambiar seguramente cómo se le pasa la info más bien
        List<List<TableCard>> populationList = new List<List<TableCard>>();
        populationList.Add(plants);
        populationList.Add(herbivorous);
        populationList.Add(carnivorous);

        for(int i = 1;i < populationList.Count; i++)
        {
            // se comprueba que haya alguno del tipo, no tiene sentido hacerlo sino
            if(populationList[i].Count > 0)
            {
                bool dies = CheckPopulationDeath(populationList[i], populationList[i - 1], populationList[i + 1]);
                // si muere  no incrementa
                if(!dies)
                {
                    bool increases = CheckPopulationIncrease(populationList[i], populationList[i - 1], populationList[i + 1]);   
                }
                // muere 
                else
                {
                    // -1 populationTipo
                }
            }
        }
    }

    // se está haciendo pensando para herbívoros realmente no es genérico
    private bool CheckPopulationDeath(List<TableCard> pToCheck, List<TableCard> pInferior, List<TableCard> pSuperior)
    {
        // hay dos plantas más (population inferior)
        if((pToCheck.Count - pInferior.Count) > 1)
        {
            return true;
        }
        // hay dos carnivoros mas (population superior)
        if((pSuperior.Count - pToCheck.Count) > 1)
        {
            return true;
        }
        // no se cumple alguna de las condiciones, no muere
        return false;
    }

    private bool CheckPopulationIncrease(List<TableCard> pToCheck, List<TableCard> pInferior, List<TableCard> pSuperior)
    {
        return false;
    }

}
