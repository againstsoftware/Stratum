using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(IInteractable))]
public class InteractableInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    private IInteractable _interactable;
    private PlayableItem _playable;
    private void Start()
    {
        _interactable = GetComponent<IInteractable>();
        _playable = _interactable as PlayableItem;;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InteractionManager.Instance.SelectInteractable(_interactable);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InteractionManager.Instance.DeselectInteractable(_interactable);
    }
    
    public void OnPointerDown(PointerEventData eventData)    
    {
        if(_playable is not null) InteractionManager.Instance.DragPlayableItem(_playable);                 
    }                                                        

    public void OnPointerUp(PointerEventData eventData)
    {
        if(_playable is not null) InteractionManager.Instance.DropPlayableItem(_playable);       
    }

    
    
    
    
}
