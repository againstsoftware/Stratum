using System;
using UnityEngine;

public class RulesManager : IRulesSystem
{
    public bool IsValidAction(PlayerCharacter actor, IActionItem actionItem, Receiver[] receivers)
    {   
        foreach(var validAction in actionItem.ValidActions)
        {
            foreach(var receiver in receivers)
            {
                if(validAction.DropLocation == receiver.Location || validAction.DropLocation == ValidDropLocation.AnySlot)
                {
                    return true;
                    /*
                    queria meter esto tambien pero no funciona y va bien igualmente ¿?

                    foreach (var validReceiver in validAction.Receivers)
                    {
                        var expectedDropLocation = ValidAction.ActionReceiverToDropLocation(validReceiver);
                        if (expectedDropLocation == receiver.Location)
                        {
                            Debug.Log("accion valida");
                            return true;
                        }
                    }
                    */
                }
            }
        }
        return false;
    }

    // ServerRPC
    public void PerformAction(PlayerCharacter actor, IActionItem actionItem, Receiver[] receivers)
    {
        bool isPerformable = false;

        foreach(var validAction in actionItem.ValidActions)
        {
            foreach(var receiver in receivers)
            {
                if(validAction.DropLocation == receiver.Location || validAction.DropLocation == ValidDropLocation.AnySlot)
                {
                    isPerformable = true;
                    // SendCommandExec(actionItem.ValidActions);
                }
            }
        }
            
        if(!isPerformable) 
        {
            // avisar de trampas! :O
        }
    }

   
}