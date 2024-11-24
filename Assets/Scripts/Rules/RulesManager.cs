using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class RulesManager : MonoBehaviour, IRulesSystem
{
    public event Action<PlayerCharacter[]> OnGameOver;

    private readonly List<IRoundEndObserverEffectCommand> _roundEndObservers = new();

    [SerializeField] private Effect[] _initialEffects;
    [SerializeField] private bool _playEcosystem = true;
    
    private GameConfig _config;
    
    private IReadOnlyList<PlayerAction> _forcedActions;
    private bool _checkOnlyActionItem;

    private void Start()
    {
        ServiceLocator.Get<ITurnSystem>().OnGameStart += OnGameStart;
        ServiceLocator.Get<ITurnSystem>().OnTurnChanged += OnTurnChanged;
        ServiceLocator.Get<IModel>().OnPopulationGrow += OnPopulationGrow;
        ServiceLocator.Get<IModel>().OnPopulationDie += OnPopulationDie;

        _config = ServiceLocator.Get<IModel>().Config;
        RulesCheck.Config = _config;
    }

    private void OnDisable()
    {
        ServiceLocator.Get<ITurnSystem>().OnGameStart -= OnGameStart;
        ServiceLocator.Get<ITurnSystem>().OnTurnChanged -= OnTurnChanged;
        ServiceLocator.Get<IModel>().OnPopulationGrow -= OnPopulationGrow;
        ServiceLocator.Get<IModel>().OnPopulationDie -= OnPopulationDie;
    }


    public bool IsValidAction(PlayerAction action)
    {
        if (_forcedActions is not null && _forcedActions.Count > 0)
        {
            if (IsValidForcedAction(action))
            {
                return RulesCheck.CheckAction(action);
            }

            Debug.Log("accion del jugador incorrecta");
            return false;
        }
        else return RulesCheck.CheckAction(action);   
        
    }

    private bool IsValidForcedAction(PlayerAction action)
    {
        return !_checkOnlyActionItem ? 
            _forcedActions.Any(forcedAction => forcedAction.Equals(action)) : 
            _forcedActions.Any(forcedAction => forcedAction.ActionItem.Equals(action.ActionItem));
    }

    public void SetForcedAction(IReadOnlyList<PlayerAction> forcedActions, bool checkOnlyActionItem = false)
    {
        _forcedActions = forcedActions;
        _checkOnlyActionItem = checkOnlyActionItem;
    }


    public void DisableForcedAction() => _forcedActions = null;



    public void PerformAction(PlayerAction action)
    {
        StartCoroutine(SendToExecute(action));
    }

    public void RegisterRoundEndObserver(IRoundEndObserverEffectCommand reo)
    {
        _roundEndObservers.Add(reo);
    }

    public void RemoveRoundEndObserver(IRoundEndObserverEffectCommand reo)
    {
        _roundEndObservers.Remove(reo);
    }

    public IEnumerable<IEffectCommand> GetRoundEndObserversEffects()
    {
        //hacemos una copia de la lista de observers para que los efectos observers se puedan quitar de la lista original
        var observers = _roundEndObservers.ToArray();
        var observerCommands =
            observers.SelectMany(reo => reo.GetRoundEndEffects());

        return observerCommands;
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

        // ServiceLocator.Get<IExecutor>().ExecuteRulesEffects (new[] { Effect.Draw5, Effect.PlaceInitialCards }, null);
        //if (_initialEffects.Any())
        ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(_initialEffects, null);
    }

    private void OnTurnChanged(PlayerCharacter onTurn)
    {
        if (onTurn is not PlayerCharacter.None || !_playEcosystem) return;

        PlayEcosystemTurn();
    }

    private void PlayEcosystemTurn()
    {
        var effects = RulesCheck.CheckEcosystem();
        if (!effects.Any())
        {
            Debug.Log("No habia efectos de ecosistema");
            EndRound();
        }
        else
        {
            effects.Insert(0, Effect.OverviewSwitch);
            ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(effects, EndRound);
        }
    }

    //estos metodos se llaman en medio de la ejecucion de efectos del turno del ecosistema
    //o cuando por una carta de inlfuencia, muere o crece una poblacion en cualquier momento
    //comprueban si la carta de poblacion respectiva tiene un efecto al crecer/morir que se deba empujar a la cola
    //lo de arriba es medio bait xd
    private void OnPopulationGrow(TableCard parent, TableCard child)
    {
    }

    private void OnPopulationDie(TableCard tableCard)
    {
    }


    private void EndRound() //comprueba si hay algun efecto de final de ronda que se deba aplicar y lo aplica
    {
        Debug.Log("fin de ronda.");
        List<IEffectCommand> roundEndCommands = new();
        //comprobacion de destruir construccion si no tiene animales
        var destroyConstructionCommands = RulesCheck.CheckConstructions();

        roundEndCommands.AddRange(destroyConstructionCommands);

        
        roundEndCommands.AddRange(GetRoundEndObserversEffects());

        if (roundEndCommands.Any())
        {
            Debug.Log("no habia efectos de fin de ronda");
            ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(roundEndCommands, StartNextRound);
        }

        else StartNextRound();
    }

    private void StartNextRound() //comprueba condicion de victoria e inicia la siguiente ronda robando 2
    {
        Debug.Log("comenzando ronda");
        //comprobar si hay condiciones de victoria
        if (HasSomeoneWon(out PlayerCharacter[] winners))
        {
            //hacer cosas de game over
            Debug.Log("game over");
            OnGameOver?.Invoke(winners);
            return;
        }

        //inicia la siguiente ronda con todos los jugadores robando y luego empieza el turno del primero
        ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(new[] { Effect.Draw2 }, null);
    }

    private bool HasSomeoneWon(out PlayerCharacter[] winners)
    {
        var config = ServiceLocator.Get<IModel>().Config;

        // int plantsNum = ServiceLocator.Get<IModel>().Ecosystem.Plants.Count;
        // int herbivoresNum = ServiceLocator.Get<IModel>().Ecosystem.Herbivores.Count;
        // int carnivoresNum = ServiceLocator.Get<IModel>().Ecosystem.Carnivores.Count;


        int growthsNum = ServiceLocator.Get<IModel>().Ecosystem.Growths;
        int macrofungiNum = ServiceLocator.Get<IModel>().Ecosystem.Macrofungi.Count;

        // bool natureWon = plantsNum >= config.NaturePlantsToWin &&
        //                  herbivoresNum >= config.NatureHerbivoresToWin &&
        //                  carnivoresNum >= config.NatureCarnivoresToWin;

        bool natureWon = growthsNum >= config.GrowthsToWin;

        bool fungalothWon = macrofungiNum >= config.MacrofungiToWin;
        bool overlordWon = ServiceLocator.Get<IModel>().NumberOfConstructions >= config.ConstructionsToWin;

        var winnersList = new List<PlayerCharacter>();
        if (natureWon)
        {
            winnersList.Add(PlayerCharacter.Ygdra);
            winnersList.Add(PlayerCharacter.Sagitario);
        }

        if (fungalothWon) winnersList.Add(PlayerCharacter.Fungaloth);
        if (overlordWon) winnersList.Add(PlayerCharacter.Overlord);

        winners = winnersList.ToArray();
        return winners.Any();
    }
}