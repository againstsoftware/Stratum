using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RulesManager : IRulesSystem
{
    public bool IsValidAction(PlayerAction action)
    {   
        /*
        general:
            x - que sea el turno de quien juegue 

        cartas:
            x - comprobar indice de la carta en la mano en model a ver si coincide

        tokens:
            x - comprobar que el personaje (actor) que hace la accion puede jugar un token (overlord o fungaloth)



        si es una carta de criatura:
            - comprobar que los receivers es una lista de 1 solo elemento
            - comprobar que ese elemento es un slot y esta vacio

        token de construccion:
            - comprobar que los receivers es una lista de 1 solo elemento
            - que ese elemento es un territorio
            - que ese territorio no este construido
            - que no haya un carnivoro en ningun slot de ese territorio
            - que haya por lo menos 2 plantas en los slots de ese territorio

        token de macrohongo:
            - comprobar que los receiver es una lista de 3 elementos
            - que cada elemento sea una carta de seta
            - que cada una de esas cartas este presente en el model en el territorio y posicion que indica
        */

        // NO IMPLEMENTADO: comprobar que es una carta de poblacion 
            // una vez se implemente eso tambien poner que si no es de ningun tipo de los que hay -> trampas

        // solo se comprueba si es una carta 
        if(action.ActionItem is ICard)
        {
            // COMPROBACIONES GENERICAS PARA CARTAS
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
            // el jugador es Ovelord (token de construccion)
            if(action.Actor == PlayerCharacter.Overlord)
            {
                // estoy suponiendo que no puede haber errores de que el overlord pille el token de fungi

                // receivers 1 solo elemento
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
                // no hay s2 plantas al menos
                if(herbCount < 2)
                {
                    return false;
                } 
            }

            // token de fungi
            else if(action.Actor == PlayerCharacter.Fungaloth)
            {

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
