using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Book : MonoBehaviour, IMenuInteractable
{
    private float scaleIncrease = 1.2f;
    public InteractablesObjects InteractableObject {get; private set; } = InteractablesObjects.Book;
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.localScale *= scaleIncrease;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.localScale /= scaleIncrease;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerCLick - MenuObject"); 
    }

    public void Interact()
    {
        Debug.Log("interacting with BOOK");
    }
}
