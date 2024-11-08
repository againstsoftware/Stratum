using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectExecutor : IExecutor
{
    private List<EffectCommand> _commandDEQueue;
    private PlayerAction _currentAction;
    private Action _rulesCallback;

    public void ExecutePlayerActionEffects(PlayerAction action)
    {
        _currentAction = action;
        var item = action.ActionItem as IEffectContainer;
        _commandDEQueue = new();

        var effectsIndex = action.EffectsIndex;

        var effects = item.GetEffects(effectsIndex);

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

    private void UpdatePlayedCardsInModel(PlayerAction action)
    {
        if (action.ActionItem is AToken)
        {
            ServiceLocator.Get<IModel>().GetPlayer(action.Actor).TokenPlayed = true;
        }
        else if (action.ActionItem is AInfluenceCard)
        {
            ServiceLocator.Get<IModel>().GetPlayer(action.Actor).InfluencePlayed = true;
        }
    }

    private void Execute(EffectCommand effectCommand)
    {
        effectCommand(_currentAction, TryExecuteNextCommand);
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
        ServiceLocator.Get<ITurnSystem>().OnActionEnded();
    }


    private void EnqueueCommand(EffectCommand ec) => _commandDEQueue.Insert(0, ec);


    private bool DequeueCommand(out EffectCommand ec)
    {
        ec = null;
        if (_commandDEQueue.Count == 0) return false;

        ec = _commandDEQueue[^1];
        _commandDEQueue.RemoveAt(_commandDEQueue.Count - 1);
        return true;
    }


    public void PushCommand(Effect effect) => _commandDEQueue.Add(EffectCommands.Get(effect));
}