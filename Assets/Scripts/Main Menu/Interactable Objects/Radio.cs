using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class Radio : AInteractableObject
{
    [SerializeField] private GameObject _ButtonLobby;
    public override void EnableInteraction()
    {
        _isEnabled = true;
        _ButtonLobby.SetActive(true);
    }

    public override void DisableInteraction()
    {
        _isEnabled = false;
        _ButtonLobby.SetActive(false);
        gameObject.transform.localScale /= scaleIncrease;

    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("radio clicada");
    }
   
}
