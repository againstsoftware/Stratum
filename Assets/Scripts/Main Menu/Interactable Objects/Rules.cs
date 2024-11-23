using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rules : AInteractableObject
{
    // pruebas
    Material materialOG;

    public override void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Libro pulsaod");
    }

    public override void EnableInteraction()
    {
        if (!_isEnabled)
        {
            // abrir y permitir interacción
            _isEnabled = true;

            Canvas canvas = gameObject.GetComponentInChildren<Canvas>();
            
            // para pruebas
            materialOG = gameObject.GetComponent<Renderer>().material;
            gameObject.GetComponent<Renderer>().material = null;
        }
    }

    public override void DisableInteraction()
    {
        // cerrar libro y no permitir interacción 
        _isEnabled = false;
        gameObject.transform.localScale /= scaleIncrease;

        // pruebas
        gameObject.GetComponent<Renderer>().material = materialOG;

    }
}
