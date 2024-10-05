using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputHandler
{
    private InputActionAsset _inputActions;

    public event Action<float> Scroll;
    public event Action<Vector2> PointerPosition;

    public InputHandler(InputActionAsset inputActions)
    {
        _inputActions = inputActions;
        _inputActions.FindAction("Scroll").performed += OnScroll;
        _inputActions.FindAction("PointerPosition").performed += OnPointerPositionChanged;
    }

    ~InputHandler()
    {
        _inputActions.FindAction("Scroll").performed -= OnScroll;

    }

    private void OnScroll(InputAction.CallbackContext ctx)
    {
        Scroll?.Invoke(ctx.ReadValue<Vector2>().y);
    }

    private void OnPointerPositionChanged(InputAction.CallbackContext ctx)
    {
        PointerPosition?.Invoke(ctx.ReadValue<Vector2>());
    }
    
}
