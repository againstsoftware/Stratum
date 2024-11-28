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

    // por ahora así seguramente se podría mejorar
    private RaycastHit lastHit;


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
        Ray ray = _interactionSystem.Camera.ScreenPointToRay(_pointerPosition);
        var hit = Physics.Raycast(ray, out var hitInfo, float.MaxValue, _interactionSystem.InteractablesLayer);

        // hit con interactableLayer / zoom
        if (hit && hitInfo.collider)
        {
            // para que no afecte a los interactuables de dentro de cada objeto 
            if (hitInfo.transform.gameObject.GetComponentInChildren<IMenuInteractable>() != null &&  (lastHit.collider != hitInfo.collider))
            {
                lastHit = hitInfo;
                _interactionSystem.ClearState();

                //hitInfo.collider.GetComponent<IMenuInteractable>().OnPointerPress();
                _interactionSystem.Camera.GetComponent<MenuCameraMovement>().StartMoving(hitInfo.transform.position);
                _interactionSystem.SetState(hitInfo.collider.GetComponent<IMenuInteractable>());
            }

        }
        // reset camera
        else
        {
            lastHit = default;
            _interactionSystem.Camera.GetComponent<MenuCameraMovement>().StartReturning();
            _interactionSystem.ClearState();
        }
    }
}
