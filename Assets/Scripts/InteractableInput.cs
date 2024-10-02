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
        InputManager.Instance.Select(_interactable);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InputManager.Instance.Deselect(_interactable);
    }
    
    public void OnPointerDown(PointerEventData eventData)    
    {                                                        
        InputManager.Instance.Drag(_interactable);                 
    }                                                        

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    
    
    
    
}
