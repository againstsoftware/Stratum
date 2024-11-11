using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class RulesCheck
{
    public static GameConfig Config { get; set; }

    public static bool CheckAction(PlayerAction action)
    {
        // Debug.Log($@"
        // recibida player action para checkear:
        // ------------------------------------------------
        // {action.Actor}
        // {action.ActionItem.name}
        // Receivers:
        // {string.Join("\n    ", action.Receivers
        //     .Select(r => $"{r.Location} - {r.LocationOwner} ->idx: {r.Index} ->idx2: {r.SecondIndex}"))}
        // effects idx: {action.EffectsIndex}
        // ------------------------------------------------
        // ");

        if (action.Actor is PlayerCharacter.None)
        {
            return false;
        }

        // es el jugador del turno actual
        if (action.Actor != ServiceLocator.Get<IModel>().PlayerOnTurn)
        {
            Debug.Log($"rechazada porque le toca a {ServiceLocator.Get<IModel>().PlayerOnTurn}");
            return false;
        }

        return action.ActionItem.CheckAction(action);
    }


    public static IEnumerable<Effect> CheckEcosystem()
    {
        Debug.Log("check ecosystem");
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
            effects.Add(Effect.KillHerbivoreEcosystem);
            effects.Add(Effect.GrowMushroomEcosystem);
        }
        else if (herbivoresGrow)
        {
            effects.Add(Effect.GrowHerbivoreEcosystem);
        }

        if (carnivoresDeath)
        {
            effects.Add(Effect.KillCarnivoreEcosystem);
            effects.Add(Effect.GrowMushroomEcosystem);
        }
        else if (carnivoresGrow)
        {
            effects.Add(Effect.GrowCarnivoreEcosystem);
        }

        return effects;
    }
}