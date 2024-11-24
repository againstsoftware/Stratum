using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystemMenu : MonoBehaviour, IInteractionSystemMenu
{
    private IMenuInteractable _interactable;
    public Camera Camera { get; private set; }
    public InputHandlerMenu Input { get; private set; }
    [SerializeField] private InputActionAsset _inputActions;
    [field: SerializeField] public LayerMask InteractablesLayer { get; private set; }

    private void Awake()
    {
        Input = new(this, _inputActions);
    }

    private void Start()
    {
        Camera = Camera.main;
    }

    public void SetState(IMenuInteractable interactable)
    {
        if (interactable != _interactable)
        {
            _interactable = interactable;  //imenuinteractable
            _interactable.EnableInteraction();
        }
    }

    public void ClearState()
    {
        if (_interactable != null)
        {

            _interactable.DisableInteraction();
            _interactable = null;
        }
    }

    private void Update()
    {
        /*
        lo dejo por si lo necesito para luego la implementación de lo que hace cada uno pero creo que no será necesario

        switch(_currentInteractable)
        {
            case InteractablesObjects.None:
                break;
            case InteractablesObjects.Book:
                _interactable.Interact();
                break;
            case InteractablesObjects.Radio:
                _interactable.Interact();
                break;
            
        } 
        */

        //_interactable?.Interact();
    }

    private void OnDestroy()
    {
        Input.Dispose();
        Input = null;
    }

}
