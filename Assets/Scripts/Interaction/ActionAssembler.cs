using System;
using System.Collections.Generic;
using UnityEngine;

public static class ActionAssembler
{
    public static IReadOnlyList<IActionReceiver> ActionReceivers => _actionReceivers;
    public static APlayableItem PlayableItem;

    private static readonly Queue<ValidActionReceiver> _receiversQueue = new();
    private static readonly List<Receiver> _receiversList = new();
    private static readonly List<IActionReceiver> _actionReceivers = new();

    private static IInteractionSystem _interactionSystem;
    private static ValidAction _validAction;

    public enum AssemblyState
    {
        Failed,
        Ongoing,
        Completed
    }

    public static AssemblyState TryAssembleAction(APlayableItem playableItem, IActionReceiver dropLocation)
    {
        _interactionSystem ??= ServiceLocator.Get<IInteractionSystem>();

        PlayableItem = null;
        _receiversQueue.Clear();
        _receiversList.Clear();
        _actionReceivers.Clear();
        _validAction = null;

        foreach (var validAction in playableItem.ActionItem.GetValidActions())
        {
            if (!CheckIfValid(playableItem, dropLocation, validAction)) continue;
            return AssembleAction(playableItem, dropLocation, validAction);
        }

        //notificar fallo
        return AssemblyState.Failed;
    }

    public static bool CheckIfValid(APlayableItem playableItem, IActionReceiver dropLocation,
        ValidAction validAction) =>
        validAction.DropLocation switch
        {
            ValidDropLocation.OwnerSlot => dropLocation is SlotReceiver && playableItem.Owner == dropLocation.Owner,
            ValidDropLocation.AnySlot => dropLocation is SlotReceiver,
            ValidDropLocation.AnyTerritory => dropLocation is TerritoryReceiver,
            ValidDropLocation.OwnerCard => dropLocation is PlayableCard pc && pc.CurrentState is APlayableItem.State.Played &&
                                           playableItem.Owner == dropLocation.Owner,
            ValidDropLocation.AnyCard => dropLocation is PlayableCard pc && pc.CurrentState is APlayableItem.State.Played,
            ValidDropLocation.TableCenter => dropLocation is TableCenter,
            ValidDropLocation.DiscardPile => dropLocation is DiscardPileReceiver && playableItem is PlayableCard &&
                                             playableItem.Owner == dropLocation.Owner,
            _ => throw new ArgumentOutOfRangeException()
        };


    private static AssemblyState AssembleAction(APlayableItem playableItem, IActionReceiver dropLocation,
        ValidAction validAction)
    {
        _receiversQueue.Clear();
        _receiversList.Clear();
        _validAction = validAction;
        PlayableItem = playableItem;
        foreach (var receiver in validAction.Receivers) _receiversQueue.Enqueue(receiver);
        if (dropLocation is not TableCenter)
        {
            _actionReceivers.Add(dropLocation);
            _receiversList.Add(dropLocation.GetReceiverStruct(validAction.DropLocation));
        }

        if (_receiversQueue.Count > 0)
        {
            //decirle al IS que se ponga en modo choosing
            return AssemblyState.Ongoing;
        }

        //le mandamos la accion ya completada al sistema de reglas
        return SendCompletedAction() ? AssemblyState.Completed : AssemblyState.Failed;
    }

    public static AssemblyState AddReceiver(IActionReceiver receiver)
    {
        var validReceiver = _receiversQueue.Dequeue();
        bool isValid = validReceiver switch
        {
            ValidActionReceiver.OwnerSlot => receiver is SlotReceiver && PlayableItem.Owner == receiver.Owner,
            ValidActionReceiver.AnySlot => receiver is SlotReceiver,
            ValidActionReceiver.AnyTerritory => receiver is TerritoryReceiver,
            ValidActionReceiver.OwnerCard => receiver is PlayableCard && PlayableItem.Owner == receiver.Owner,
            ValidActionReceiver.AnyCard => receiver is PlayableCard,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (!isValid)
        {
            //le decimos al sistema que vuelva a idle
            return AssemblyState.Failed;
        }

        _actionReceivers.Add(receiver);
        _receiversList.Add(receiver.GetReceiverStruct(ValidAction.ActionReceiverToDropLocation(validReceiver)));

        if (_receiversQueue.Count > 0) return AssemblyState.Ongoing;

        //si la cola esta vacia, hemos terminado y hay que enviar la jugada al sistema de reglas
        return SendCompletedAction() ? AssemblyState.Completed : AssemblyState.Failed;
    }

    private static bool SendCompletedAction()
    {
        var playerActionStruct = new PlayerAction(
            PlayableItem.Owner,
            PlayableItem.ActionItem,
            _receiversList.ToArray(),
            _validAction.Index);

        if (!ServiceLocator.Get<IRulesSystem>().IsValidAction(playerActionStruct))
        {
            Debug.Log("Accion no valida tras comprobacion en local.");
            return false;
        }
        
        Debug.Log("Comprobacion de la accion en local exitosa! mandando la accion a la autoridad");
        
        ServiceLocator.Get<IRulesSystem>().PerformAction(playerActionStruct);
        //devolver true desactiva el Interaction System, lo vuelve a activar el sistema de turnos cuando acaben los efectos
        return true;
    }
}