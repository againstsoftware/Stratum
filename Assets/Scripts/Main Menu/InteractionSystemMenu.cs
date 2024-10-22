using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystemMenu : MonoBehaviour, IInteractionSystemMenu
{
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
}
