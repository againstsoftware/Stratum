using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractionSystem : IService
{
    public IInteractable SelectedInteractable { get; }
    public IActionReceiver SelectedDropLocation { get; }
    public enum State { Waiting, Idle, Dragging, Choosing }
    public State CurrentState { get; }
    public InputHandler Input { get; }
    public Camera Camera { get;}
    public LayerMask InteractablesLayer { get; }
    public IReadOnlyList<IActionReceiver> CurrentActionReceivers { get; }
    public APlayableItem CurrentActionPlayableItem { get; }


    public void SelectInteractable(IInteractable item);
    public void DeselectInteractable(IInteractable item);
    public void DragPlayableItem(APlayableItem item);
    public void DropPlayableItem(APlayableItem item);
    public void SelectReceiver(IActionReceiver receiver);
    public void DeselectReceiver(IActionReceiver receiver);
    public void ClickReceiver(IActionReceiver receiver);

}