using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class AInteractableObject : MonoBehaviour, IMenuInteractable
{
    protected float scaleIncrease = 1.2f;
    protected bool _isEnabled;
    public abstract void OnPointerClick(PointerEventData eventData);

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if(!_isEnabled) gameObject.transform.localScale *= scaleIncrease;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if(!_isEnabled) gameObject.transform.localScale /= scaleIncrease;
    }

    public virtual void EnableInteraction()
    {
        _isEnabled = true;
    }

    public virtual void DisableInteraction()
    {
        _isEnabled = false;

        gameObject.transform.localScale /= scaleIncrease;
    }    
}
