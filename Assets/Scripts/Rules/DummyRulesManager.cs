using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DummyRulesManager : MonoBehaviour, IRulesSystem
{
    private bool _hasCheckedRoundEndEffects;
    
    
    private void Start()
    {
        ServiceLocator.Get<ITurnSystem>().OnGameStart += OnGameStart;
        ServiceLocator.Get<ITurnSystem>().OnTurnChanged += OnTurnChanged;
        ServiceLocator.Get<IModel>().OnPopulationGrow += OnPopulationGrow;
        ServiceLocator.Get<IModel>().OnPopulationDie += OnPopulationDie;
    }

    private void OnDisable()
    {
        ServiceLocator.Get<ITurnSystem>().OnGameStart -= OnGameStart;
        ServiceLocator.Get<ITurnSystem>().OnTurnChanged -= OnTurnChanged;
        ServiceLocator.Get<IModel>().OnPopulationGrow -= OnPopulationGrow;
        ServiceLocator.Get<IModel>().OnPopulationDie -= OnPopulationDie;
    }


    public bool IsValidAction(PlayerAction action) => true;

    public void PerformAction(PlayerAction action)
    {
        StartCoroutine(SendToExecute(action));
    }


    private IEnumerator SendToExecute(PlayerAction action)
    {
        yield return null;

        ServiceLocator.Get<ICommunicationSystem>().SendActionToAuthority(action);

        // ServiceLocator.Get<IExecutor>().ExecuteEffectCommands(action);
    }


    private void OnGameStart()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        var comms = ServiceLocator.Get<ICommunicationSystem>();
        comms.SyncRNGs();
        while (!comms.IsRNGSynced) yield return null;
        Debug.Log("random sincronizado!");

        ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(new[] { Effect.Draw5 }, null);
    }

    private void OnTurnChanged(PlayerCharacter onTurn)
    {
        if (onTurn is not PlayerCharacter.None) return;

        //ecosistema 
        PlayEcosystemTurn();
    }

    private void PlayEcosystemTurn()
    {
        List<Effect> effects = new();
        //revisa las reglas del ecosistema
        //añade efectos de crecer y/o morir
        ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(effects, RoundEndCheck);
    }

    //estos metodos se llaman en medio de la ejecucion de efectos del turno del ecosistema
    //comprueban si la carta de poblacion respectiva tiene un efecto al crecer/morir que se deba empujar a la cola
    private void OnPopulationGrow(TableCard tableCard)
    {
        
    }

    private void OnPopulationDie(TableCard tableCard)
    {
        
    }

    private void RoundEndCheck()
    {
        //comprueba si hay algun efecto de final de ronda que se deba aplicar y lo aplica
        var effects = GetRoundEndEffects();
        if (effects is not null && effects.Any())
        {
            ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(effects, RoundEndCheck);
            return;
        }

        _hasCheckedRoundEndEffects = false; //reset de check bool
        
        //comprobar si hay condiciones de victoria
        if (HasSomeoneWon())
        {
            //hacer cosas de game over
            return;
        }
        
        //inicia la siguiente ronda con todos los jugadores robando y luego empieza el turno del primero
        ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(new[] { Effect.Draw2 }, null);
    }

    private Effect[] GetRoundEndEffects()
    {
        if (_hasCheckedRoundEndEffects) return null;
        _hasCheckedRoundEndEffects = true;
        return null;
    }

    private bool HasSomeoneWon() => false;

}
