using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionAssembler : MonoBehaviour
{
    public void TryAssembleAction(APlayableItem playableItem, IActionReceiver dropLocation)
    {
        foreach (var validAction in playableItem.ActionItem.ValidActions)
        {
            if (!CheckIfValid(playableItem, dropLocation, validAction)) continue;
            AssembleAction(playableItem, dropLocation, validAction);
            return;
        }
        //notificar fallo
    }

    private static bool CheckIfValid(APlayableItem playableItem, IActionReceiver dropLocation, ValidAction validAction) =>
        validAction.DropLocation switch
        {
            ValidDropLocation.OwnerSlot => dropLocation is Slot && playableItem.Owner == dropLocation.Owner,
            ValidDropLocation.AnySlot => dropLocation is Slot,
            ValidDropLocation.AnyTerritory => dropLocation is Territory,
            ValidDropLocation.OwnerCard => dropLocation is PlayableCard && playableItem.Owner == dropLocation.Owner,
            ValidDropLocation.AnyCard => dropLocation is PlayableCard,
            ValidDropLocation.TableCenter => dropLocation is TableCenter,
            _ => throw new ArgumentOutOfRangeException()
        };


    private void AssembleAction(APlayableItem playableItem, IActionReceiver dropLocation, ValidAction validAction)
    {
        List<IActionReceiver> receivers = new();
        if(dropLocation is not TableCenter) receivers.Add(dropLocation);
        foreach (var receiver in validAction.Receivers)
        {
            
        }
    }
}