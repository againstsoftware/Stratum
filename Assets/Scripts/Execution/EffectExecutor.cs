using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectExecutor : IExecutor
{
    private List<IEffectCommand> _commandDEQueue;
    private PlayerAction _currentAction;
    private Action _rulesCallback;

    public void ExecutePlayerActionEffects(PlayerAction action)
    {
        _currentAction = action;
        _commandDEQueue = new();

        var effectsIndex = action.EffectsIndex;

        var effects = action.ActionItem.GetEffects(effectsIndex);

        foreach (var e in effects) EnqueueCommand(EffectCommands.Get(e));

        UpdatePlayedCardsInModel(action);

        TryExecuteNextCommand();
    }

    public void ExecuteRulesEffects(IEnumerable<Effect> effects, Action rulesCallback)
    {
        _commandDEQueue = new();
        _rulesCallback = rulesCallback;
        foreach (var e in effects) EnqueueCommand(EffectCommands.Get(e));
        TryExecuteNextCommand();
    }

    public void ExecuteRulesEffects(IEnumerable<IEffectCommand> commands, Action rulesCallback)
    {
        _commandDEQueue = new();
        _rulesCallback = rulesCallback;
        foreach (var c in commands) EnqueueCommand(c);
        TryExecuteNextCommand();
    }

    private void UpdatePlayedCardsInModel(PlayerAction action)
    {
        if (action.ActionItem is AToken)
        {
            ServiceLocator.Get<IModel>().GetPlayer(action.Actor).TokenPlayed = true;
        }
        else if (action.ActionItem is AInfluenceCard)
        {
            if (action.Receivers.Length > 0 && action.Receivers[0].Location is ValidDropLocation.DiscardPile)
                return;
            
            ServiceLocator.Get<IModel>().GetPlayer(action.Actor).InfluencePlayed = true;
        }
    }

    private void Execute(IEffectCommand effectCommand)
    {
        effectCommand.Execute(_currentAction, TryExecuteNextCommand);
        
    }

    private void TryExecuteNextCommand()
    {
        if (DequeueCommand(out var command))
        {
            Execute(command);
            return;
        }

        if (_rulesCallback is not null)
        {
            var cb = _rulesCallback;
            _rulesCallback = null;
            cb();
            return;
        }

        //notificamos final de la accion
        Debug.Log("accion ejecutada!");
        ServiceLocator.Get<ITurnSystem>().EndAction();
    }


    private void EnqueueCommand(IEffectCommand ec) => _commandDEQueue.Insert(0, ec);


    private bool DequeueCommand(out IEffectCommand ec)
    {
        ec = null;
        if (_commandDEQueue.Count == 0) return false;

        ec = _commandDEQueue[^1];
        _commandDEQueue.RemoveAt(_commandDEQueue.Count - 1);
        return true;
    }


    public void PushDelayedCommand(IEffectCommand effectCommand)
    {
        _commandDEQueue.Add(effectCommand);   
    }
}