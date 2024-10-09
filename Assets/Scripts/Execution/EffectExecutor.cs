using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectExecutor : IExecutor
{
    private Queue<EffectCommand> _commandQueue;
    private PlayerAction _currentAction;

    public void ExecuteEffectCommands(PlayerAction action)
    {
        _currentAction = action;
        _commandQueue = new();
        var card = _currentAction.ActionItem as ACard;
        var token = _currentAction.ActionItem as Token;
        var effects = card is not null ? card.Effects : token.Effects;
        
        foreach(var e in effects) _commandQueue.Enqueue(EffectCommands.Get(e));
        
        TryExecuteNextCommand();
    }

    private void Execute(EffectCommand effectCommand)
    {
        effectCommand(_currentAction, TryExecuteNextCommand);
    }

    private void TryExecuteNextCommand()
    {
        if (_commandQueue.TryDequeue(out var command))
        {
            Execute(command);
            return;
        }
        
        //notificamos final de la accion
        Debug.Log("accion ejecutada!");
        ServiceLocator.Get<ITurnSystem>().OnActionEnded();
    }
}