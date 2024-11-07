using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Registry : MonoBehaviour, IMenuInteractable
{
    private float scaleIncrease = 1.2f;
    public InteractablesObjects InteractableObject { get; private set; } = InteractablesObjects.Registry;
    private bool isEnabled = false; //por ahora la dejo pero quizás como tal podría no hacer falta
    
    // para pruebas y variables propias de este objeto
    Material materialOG;
    [SerializeField] Collider Link;

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
        if(isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Link))
        {
            Application.OpenURL("https://x.com/home");
        }
    }

    public void Interact()
    {
        if (isEnabled)
        {
            Debug.Log("interacting with REGISTRY");

        }
    }

    public void EnableInteraction()
    {
        if (!isEnabled)
        {
            // abrir el registry y permitir interacción
            isEnabled = true;

            Canvas canvas = gameObject.GetComponentInChildren<Canvas>();
            
            // esto es de prueba
            StartCoroutine(EsperarUnSegundo(canvas));

            // para pruebas
            materialOG = gameObject.GetComponent<Renderer>().material;
            gameObject.GetComponent<Renderer>().material = null;
        }

    }

    public void DisableInteraction()
    {
        // cerrrar libro y no permitir interacción 
        isEnabled = false;

        Canvas canvas = gameObject.GetComponentInChildren<Canvas>();
        foreach(Transform child in canvas.transform)
        {
            child.gameObject.SetActive(false);
        }

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
