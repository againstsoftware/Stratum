using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystemMenu : MonoBehaviour, IInteractionSystemMenu
{
    private InteractablesObjects _currentInteractable = InteractablesObjects.None;
    private IMenuInteractable _interactable;
    public Camera Camera { get; private set; }
    public InputHandlerMenu Input { get; private set; }
    [SerializeField] private InputActionAsset _inputActions;
    [field: SerializeField] public LayerMask InteractablesLayer { get; private set; }

    // esto revisarlo en la interfaz
    private GamePrefsInitializer gamePrefs;

    private void Awake()
    {
        Input = new(this, _inputActions);
    }

    private void Start()
    {
        Camera = Camera.main;

        // por ahora así
        gamePrefs = new GamePrefsInitializer();
    }

    public void SetState(IMenuInteractable interactable)
    {
        if (interactable != _interactable)
        {

            _currentInteractable = interactable.InteractableObject; //state, quizás lo quito lueog 
            _interactable = interactable;  //imenuinteractable
            Debug.Log("_currentInteractable - Interactionsystemmenu: " + _currentInteractable);

            _interactable.EnableInteraction();
        }
    }

    public void ClearState()
    {
        _interactable.DisableInteraction();
        _currentInteractable = InteractablesObjects.None;
        //_interactable.Disable();
        _interactable = null;
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
        _interactable?.Interact();
    }

    private void OnDestroy()
    {
        Input.Dispose();
        Input = null;
    }

}
