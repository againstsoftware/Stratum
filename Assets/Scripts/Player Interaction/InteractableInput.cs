using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(IInteractable))]
public class InteractableInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    private IInteractable _interactable;
    private void Start()
    {
        _interactable = GetComponent<IInteractable>();
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
        InteractionManager.Instance.DragInteractable(_interactable);                 
    }                                                        

    public void OnPointerUp(PointerEventData eventData)
    {
        InteractionManager.Instance.DropInteractable(_interactable);
    }

    
    
    
    
}
