using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandlerMenu
{
    private InputActionAsset _inputActions;

    private Vector2 _pointerPosition;
    private IInteractionSystemMenu _interactionSystem;
    
    public InputHandlerMenu(IInteractionSystemMenu interactionSystem, InputActionAsset inputActions)
    {
        _interactionSystem = interactionSystem;
        _inputActions = inputActions;
        //_inputActions.FindAction("Scroll").performed += OnScroll; 
        _inputActions.FindAction("PointerPosition").performed += OnPointerPositionChanged;
        _inputActions.FindAction("PointerPress").performed += OnPointerPress;
    }

    
    public void Dispose()
    {
        //_inputActions.FindAction("Scroll").performed -= OnScroll;
        _inputActions.FindAction("PointerPosition").performed -= OnPointerPositionChanged;
        _inputActions.FindAction("PointerPress").performed -= OnPointerPress;
    }

    private void OnPointerPositionChanged(InputAction.CallbackContext ctx)
    {
        _pointerPosition = ctx.ReadValue<Vector2>();
    }

    private void OnPointerPress(InputAction.CallbackContext ctx)
    {
        Debug.Log("OnPointerPress");

        Ray ray = _interactionSystem.Camera.ScreenPointToRay(_pointerPosition);
        var hit = Physics.Raycast(ray, out var hitInfo, float.MaxValue, _interactionSystem.InteractablesLayer);

        if(hit && hitInfo.collider)
        {            
            Debug.Log("OnPointerPress - hit");
            hitInfo.collider.GetComponent<IMenuInteractable>().OnPointerPress(_interactionSystem.Camera);
        }
        else
        {
            //GetComponent<IMenuInteractable>().ResetCamera(_interactionSystem.Camera);
        }
    }
}
