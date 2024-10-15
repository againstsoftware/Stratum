using System;
using System.Collections.Generic;
using UnityEngine;

public class RulesManager : IRulesSystem
{
    public bool IsValidAction(PlayerAction action)
    {   
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
            if(ServiceLocator.Get<IModel>().GetPlayer(action.Actor).Character != ServiceLocator.Get<IModel>().PlayerOnTurn)
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

            // REGLAS SI MUSHROOM
            if(playedCard.CardType is ICard.Card.Mushroom)
            {

            }

            // REGLAS SI MACROFUNGI
            if(playedCard.CardType is ICard.Card.Macrofungi)
            {

            }

            
        }

        return true;

    }


    // rpc
    public void PerformAction(PlayerAction action)
    {
        bool isPerformable = false;

        // pondria lo mismo que en IsValidAction, lo que cambia es que se estan comprobando las cosas con el model del host y
        // le pasa la info de la accion al ejecutor 
            
        if(!isPerformable) 
        {
            Debug.Log("trampas");
        }
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
