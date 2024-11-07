using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Registry : AInteractableObject
{
    [SerializeField] Collider Link;

    // para pruebas
    Material materialOG;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if(_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Link))
        {
            Application.OpenURL("https://x.com/home");
        }
    }

    public override void EnableInteraction()
    {
        if (!_isEnabled)
        {
            // abrir el registry y permitir interacción
            _isEnabled = true;

            Canvas canvas = gameObject.GetComponentInChildren<Canvas>();
            
            // esto es de prueba
            StartCoroutine(EsperarUnSegundo(canvas));

            // para pruebas
            materialOG = gameObject.GetComponent<Renderer>().material;
            gameObject.GetComponent<Renderer>().material = null;
        }
    }

    public override void DisableInteraction()
    {
        // cerrar libro y no permitir interacción 
        _isEnabled = false;

        Canvas canvas = gameObject.GetComponentInChildren<Canvas>();
        foreach(Transform child in canvas.transform)
        {
            child.gameObject.SetActive(false);
        }
        
        gameObject.transform.localScale /= scaleIncrease;


        // pruebas
        gameObject.GetComponent<Renderer>().material = materialOG;

    }

    // ESTO ES PARA LAS PRUEBAS PARA NO DARLE SIN QUERER a las rrss
      private IEnumerator EsperarUnSegundo(Canvas canvas)
    {
        yield return new WaitForSeconds(0.5f);

        
        foreach(Transform child in canvas.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
