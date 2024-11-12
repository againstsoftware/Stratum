using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class RulesManager : MonoBehaviour, IRulesSystem
{
    private readonly List<IRoundEndObserver> _roundEndObservers = new();

    private void Start()
    {
        ServiceLocator.Get<ITurnSystem>().OnGameStart += OnGameStart;
        ServiceLocator.Get<ITurnSystem>().OnTurnChanged += OnTurnChanged;
        ServiceLocator.Get<IModel>().OnPopulationGrow += OnPopulationGrow;
        ServiceLocator.Get<IModel>().OnPopulationDie += OnPopulationDie;

        RulesCheck.Config = ServiceLocator.Get<IModel>().Config;
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

    public void RegisterRoundEndObserver(IRoundEndObserver reo)
    {
        _roundEndObservers.Add(reo);
    }

    public void RemoveRoundEndObserver(IRoundEndObserver reo)
    {
        _roundEndObservers.Remove(reo);
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
        //hacemos una copia de la lista de observers para que los efectos observers se puedan quitar de la lista original
        var observers = _roundEndObservers.ToArray();
        var _roundEndCommands = observers.SelectMany(reo => reo.GetRoundEndEffects()).ToList();

        if (_roundEndCommands.Any())
            ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(_roundEndCommands, StartNextRound);

        else StartNextRound();
    }

    private void StartNextRound() //comprueba condicion de victoria e inicia la siguiente ronda robando 2
    {
        //comprobar si hay condiciones de victoria
        if (HasSomeoneWon(out PlayerCharacter[] winners))
        {
            //hacer cosas de game over
            Debug.Log("hay un ganador");
            Debug.Break();
            return;
        }

        //inicia la siguiente ronda con todos los jugadores robando y luego empieza el turno del primero
        ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(new[] { Effect.Draw2 }, null);
    }

    private bool HasSomeoneWon(out PlayerCharacter[] winners)
    {
        var config = ServiceLocator.Get<IModel>().Config;
        
        int plantsNum = ServiceLocator.Get<IModel>().Ecosystem.Plants.Count;
        int herbivoresNum = ServiceLocator.Get<IModel>().Ecosystem.Herbivores.Count;
        int carnivoresNum = ServiceLocator.Get<IModel>().Ecosystem.Carnivores.Count;
        int macrofungiNum = ServiceLocator.Get<IModel>().Ecosystem.Macrofungi.Count;

        bool natureWon = plantsNum >= config.NaturePlantsToWin &&
                         herbivoresNum >= config.NatureHerbivoresToWin &&
                         carnivoresNum >= config.NatureCarnivoresToWin;

        bool fungalothWon = macrofungiNum >= config.MacrofungiToWin;
        bool overlordWon = ServiceLocator.Get<IModel>().NumberOfConstructions == 4;

        var winnersList = new List<PlayerCharacter>();
        if (natureWon)
        {
            winnersList.Add(PlayerCharacter.Ygdra);
            winnersList.Add(PlayerCharacter.Sagitario);
        }
        if(fungalothWon) winnersList.Add(PlayerCharacter.Fungaloth);
        if(overlordWon) winnersList.Add(PlayerCharacter.Overlord);

        winners = winnersList.ToArray();
        return winners.Any();
    }
}