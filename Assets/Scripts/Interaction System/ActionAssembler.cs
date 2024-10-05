using System;
using System.Collections.Generic;
using UnityEngine;

public static class ActionAssembler
{
    private static readonly Queue<ValidActionReceiver> _receiversQueue = new();
    private static readonly List<Receiver> _receiversList = new();
    private static APlayableItem _playableItem;

    private static IInteractionSystem _interactionSystem;

    public enum AssemblyState
    {
        Failed,
        Ongoing,
        Completed
    }

    public static AssemblyState TryAssembleAction(APlayableItem playableItem, IActionReceiver dropLocation)
    {
        _interactionSystem ??= ServiceLocator.Get<IInteractionSystem>();

        foreach (var validAction in playableItem.ActionItem.ValidActions)
        {
            if (!CheckIfValid(playableItem, dropLocation, validAction)) continue;
            return AssembleAction(playableItem, dropLocation, validAction);
        }

        //notificar fallo
        return AssemblyState.Failed;
    }

    private static bool CheckIfValid(APlayableItem playableItem, IActionReceiver dropLocation,
        ValidAction validAction) =>
        validAction.DropLocation switch
        {
            ValidDropLocation.OwnerSlot => dropLocation is SlotReceiver && playableItem.Owner == dropLocation.Owner,
            ValidDropLocation.AnySlot => dropLocation is SlotReceiver,
            ValidDropLocation.AnyTerritory => dropLocation is TerritoryReceiver,
            ValidDropLocation.OwnerCard => dropLocation is PlayableCard pc && !pc.IsPlayed &&
                                           playableItem.Owner == dropLocation.Owner,
            ValidDropLocation.AnyCard => dropLocation is PlayableCard pc && !pc.IsPlayed,
            ValidDropLocation.TableCenter => dropLocation is TableCenter,
            ValidDropLocation.DiscardPile => dropLocation is DiscardPileReceiver && playableItem is PlayableCard,
            _ => throw new ArgumentOutOfRangeException()
        };


    private static AssemblyState AssembleAction(APlayableItem playableItem, IActionReceiver dropLocation,
        ValidAction validAction)
    {
        _receiversQueue.Clear();
        _receiversList.Clear();
        _playableItem = playableItem;
        foreach (var receiver in validAction.Receivers) _receiversQueue.Enqueue(receiver);
        if (dropLocation is not TableCenter)
            _receiversList.Add(dropLocation.GetReceiverStruct(validAction.DropLocation));
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
            ValidActionReceiver.OwnerSlot => receiver is SlotReceiver && _playableItem.Owner == receiver.Owner,
            ValidActionReceiver.AnySlot => receiver is SlotReceiver,
            ValidActionReceiver.AnyTerritory => receiver is TerritoryReceiver,
            ValidActionReceiver.OwnerCard => receiver is PlayableCard && _playableItem.Owner == receiver.Owner,
            ValidActionReceiver.AnyCard => receiver is PlayableCard,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (!isValid)
        {
            //le decimos al sistema que vuelva a idle
            return AssemblyState.Failed;
        }

        _receiversList.Add(receiver.GetReceiverStruct(ValidAction.ActionReceiverToDropLocation(validReceiver)));

        if (_receiversQueue.Count > 0) return AssemblyState.Ongoing;

        //si la cola esta vacia, hemos terminado y hay que enviar la jugada al sistema de reglas
        return SendCompletedAction() ? AssemblyState.Completed : AssemblyState.Failed;
    }

    private static bool SendCompletedAction()
    {
        var actor = _playableItem.Owner;
        var receivers = _receiversList.ToArray();
        if (!ServiceLocator.Get<IRulesSystem>().IsValidAction(actor, _playableItem.ActionItem, receivers))
            return false;

        
        ServiceLocator.Get<IRulesSystem>().PerformAction(actor, _playableItem.ActionItem, receivers);
        //devolver true desactiva el Interaction System, lo vuelve a activar el sistema de turnos cuando acaben los efectos
        return true;
    }
}