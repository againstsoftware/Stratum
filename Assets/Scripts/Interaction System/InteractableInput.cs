using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InteractableInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    private IInteractionSystem _interactionSystem;
    private IInteractable _interactable;
    private APlayableItem _playable;
    private IActionReceiver _receiver;
    private void Start()
    {
        _interactionSystem = ServiceLocator.Get<IInteractionSystem>();
        _interactable = GetComponent<IInteractable>();
        _playable = _interactable as APlayableItem;
        _receiver = GetComponent<IActionReceiver>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_interactable is not null) _interactionSystem.SelectInteractable(_interactable);
        if(_receiver is not null) _interactionSystem.SelectReceiver(_receiver);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(_interactable is not null) _interactionSystem.DeselectInteractable(_interactable);
        if(_receiver is not null) _interactionSystem.DeselectReceiver(_receiver);
    }
    
    public void OnPointerDown(PointerEventData eventData)    
    {
        if (_playable is not null) _interactionSystem.DragPlayableItem(_playable);
        if (_receiver is not null) _interactionSystem.ClickReceiver(_receiver);
    }                                                        

    public void OnPointerUp(PointerEventData eventData)
    {
        if(_playable is not null) _interactionSystem.DropPlayableItem(_playable);       
    }

}
