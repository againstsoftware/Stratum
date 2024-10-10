using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

public class RulesManager : IRulesSystem
{
    public bool IsValidAction(PlayerAction action)
    {   
        PlayerCharacter actor = action.Actor;
        IActionItem actionItem = action.ActionItem;
        IReadOnlyList<Receiver> receivers = action.Receivers;

        foreach(var validAction in actionItem.ValidActions)
        {
            foreach(var receiver in receivers)
            {
                if(validAction.DropLocation == receiver.Location || validAction.DropLocation == ValidDropLocation.AnySlot)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void PerformAction(PlayerAction action)
    {
        bool isPerformable = false;

        PlayerCharacter actor = action.Actor;
        IActionItem actionItem = action.ActionItem;
        IReadOnlyList<Receiver> receivers = action.Receivers;

        foreach(var validAction in actionItem.ValidActions)
        {
            foreach(var receiver in receivers)
            {
                if(validAction.DropLocation == receiver.Location || validAction.DropLocation == ValidDropLocation.AnySlot)
                {
                    isPerformable = true;
                    // SendCommandExec();
                }
            }
        }
            
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
