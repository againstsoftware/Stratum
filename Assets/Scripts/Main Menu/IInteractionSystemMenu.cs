using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractionSystemMenu 
{
    public Camera Camera { get;}
    public InputHandlerMenu Input { get; }
    public LayerMask InteractablesLayer { get; }
    public void SetState(IMenuInteractable menuInteractable);
    public void ClearState();
}
