using System;
using System.Collections.Generic;
using UnityEngine;

public static class ActionAssembler
{
    private static readonly Queue<ValidActionReceiver> _receiversQueue = new();
    private static readonly List<IActionReceiver> _receiversList = new();
    private static APlayableItem _playableItem;

    private static IInteractionSystem _interactionSystem;
    
    public enum AssemblyState {Failed, Ongoing, Completed}
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

    private static bool CheckIfValid(APlayableItem playableItem, IActionReceiver dropLocation, ValidAction validAction) =>
        validAction.DropLocation switch
        {
            ValidDropLocation.OwnerSlot => dropLocation is Slot && playableItem.Owner == dropLocation.Owner,
            ValidDropLocation.AnySlot => dropLocation is Slot,
            ValidDropLocation.AnyTerritory => dropLocation is Territory,
            ValidDropLocation.OwnerCard => dropLocation is PlayableCard pc && !pc.IsPlayed && playableItem.Owner == dropLocation.Owner,
            ValidDropLocation.AnyCard => dropLocation is PlayableCard pc && !pc.IsPlayed,
            ValidDropLocation.TableCenter => dropLocation is TableCenter,
            _ => throw new ArgumentOutOfRangeException()
        };


    private static AssemblyState AssembleAction(APlayableItem playableItem, IActionReceiver dropLocation, ValidAction validAction)
    {
        _receiversQueue.Clear();
        _receiversList.Clear();
        _playableItem = playableItem;
        foreach (var receiver in validAction.Receivers) _receiversQueue.Enqueue(receiver);
        if (dropLocation is not TableCenter) _receiversList.Add(dropLocation);
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
        bool isValid = _receiversQueue.Dequeue() switch
        {
            ValidActionReceiver.OwnerSlot => receiver is Slot && _playableItem.Owner == receiver.Owner,
            ValidActionReceiver.AnySlot => receiver is Slot,
            ValidActionReceiver.AnyTerritory => receiver is Territory,
            ValidActionReceiver.OwnerCard => receiver is PlayableCard && _playableItem.Owner == receiver.Owner,
            ValidActionReceiver.AnyCard => receiver is PlayableCard,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (!isValid)
        {
            //le decimos al sistema que vuelva a idle
            return AssemblyState.Failed;
        }

        _receiversList.Add(receiver);

        if (_receiversQueue.Count > 0) return AssemblyState.Ongoing;
        
        //si la cola esta vacia, hemos terminado y hay que enviar la jugada al sistema de reglas
        return SendCompletedAction() ? AssemblyState.Completed : AssemblyState.Failed;


    }

    private static bool SendCompletedAction()
    {
        if (!ServiceLocator.Get<IRulesSystem>().IsValidAction(_playableItem, _receiversList.ToArray())) return false; 
        
        //desactivar el Interaction System, lo vuelve a activar el sistema de turnos
        
        ServiceLocator.Get<IRulesSystem>().PerformAction(_playableItem, _receiversList.ToArray(),
            () =>
            {
                // item.OnDrop(SelectedDropLocation);
                // if (!item.OnlyVisibleOnOverview) _cameraMovement.ChangeToDefault();
                // //asi de momento, esto hay que meterselo a un patron command que ejecute todas los pasos de la accion secuencialmente
                    
            });
        return true;
    }
}