using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class RulesManager : MonoBehaviour, IRulesSystem
{
    [SerializeField] private GameConfig _config;
    private void Start()
    {
        ServiceLocator.Get<ITurnSystem>().OnGameStart += OnGameStart;
        ServiceLocator.Get<ITurnSystem>().OnTurnChanged += OnTurnChanged;
        ServiceLocator.Get<IModel>().OnPopulationGrow += OnPopulationGrow;
        ServiceLocator.Get<IModel>().OnPopulationDie += OnPopulationDie;

        RulesCheck.Config = _config;
    }

    private void OnDisable()
    {
        ServiceLocator.Get<ITurnSystem>().OnGameStart -= OnGameStart;
        ServiceLocator.Get<ITurnSystem>().OnTurnChanged -= OnTurnChanged;
        ServiceLocator.Get<IModel>().OnPopulationGrow -= OnPopulationGrow;
        ServiceLocator.Get<IModel>().OnPopulationDie -= OnPopulationDie;
    }


    public bool IsValidAction(PlayerAction action) => RulesCheck.CheckAction(action);


    public void PerformAction(PlayerAction action)
    {
        StartCoroutine(SendToExecute(action));
    }

    private IEnumerator SendToExecute(PlayerAction action)
    {
        yield return null;
        ServiceLocator.Get<ICommunicationSystem>().SendActionToAuthority(action);
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

        ServiceLocator.Get<IExecutor>().ExecuteRulesEffects
            (new[] { Effect.Draw5, Effect.PlaceInitialCards }, null);
    }

    private void OnTurnChanged(PlayerCharacter onTurn)
    {
        if (onTurn is not PlayerCharacter.None) return;

        //ecosistema 
        PlayEcosystemTurn();
    }

    private void PlayEcosystemTurn()
    {
        var effects = RulesCheck.CheckEcosystem();
        ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(effects, EndRound);
    }

    //estos metodos se llaman en medio de la ejecucion de efectos del turno del ecosistema
    //o cuando por una carta de inlfuencia, muere o crece una poblacion en cualquier momento
    //comprueban si la carta de poblacion respectiva tiene un efecto al crecer/morir que se deba empujar a la cola
    private void OnPopulationGrow(TableCard parent, TableCard child)
    {
    }

    private void OnPopulationDie(TableCard tableCard)
    {
    }


    private void EndRound() //comprueba si hay algun efecto de final de ronda que se deba aplicar y lo aplica
    {
        if (RulesCheck.CheckRoundEnd( out var effects))
            ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(effects, StartNextRound);

        else StartNextRound();
    }

    private void StartNextRound() //comprueba condicion de victoria e inicia la siguiente ronda robando 2
    {
        //comprobar si hay condiciones de victoria
        if (HasSomeoneWon())
        {
            //hacer cosas de game over
            Debug.Log("hay un ganador");
            return;
        }

        //inicia la siguiente ronda con todos los jugadores robando y luego empieza el turno del primero
        ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(new[] { Effect.Draw2 }, null);
    }

    private bool HasSomeoneWon() => false;
}