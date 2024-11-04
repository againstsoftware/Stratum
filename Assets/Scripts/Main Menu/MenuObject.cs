using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Platform.Android;

public class MenuObject : MonoBehaviour, IMenuInteractable
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.localScale *= 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.localScale /= 1.2f;
    }
}
