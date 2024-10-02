using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(IInteractable))]
public class InteractableInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    private IInteractable _interactable;
    private PlayableItem _playable;
    private IInteractionSystem _interactionSystem;
    private void Start()
    {
        _interactable = GetComponent<IInteractable>();
        _playable = _interactable as PlayableItem;;
        _interactionSystem = ServiceLocator.Get<IInteractionSystem>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _interactionSystem.SelectInteractable(_interactable);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _interactionSystem.DeselectInteractable(_interactable);
    }
    
    public void OnPointerDown(PointerEventData eventData)    
    {
        if(_playable is not null) _interactionSystem.DragPlayableItem(_playable);                 
    }                                                        

    public void OnPointerUp(PointerEventData eventData)
    {
        if(_playable is not null) _interactionSystem.DropPlayableItem(_playable);       
    }

}
