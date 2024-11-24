using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class Radio : AInteractableObject
{
    [SerializeField] private GameObject _ButtonLobby;
    public override void EnableInteraction()
    {
        _ButtonLobby.SetActive(true);
    }

    public override void DisableInteraction()
    {
        _ButtonLobby.SetActive(false);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("radio clicada");
    }
   
}
