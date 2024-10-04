using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float _transitionDuration;
    [SerializeField] private Transform _defaultTransform, _overviewTransform;

    private (Vector3, Quaternion) _defaultPerspective, _overviewPerspective, _startPerspective, _targetPerspective;

    private bool _isInDefault = true;
    private bool _isChangingPerspective;
    private float t = 0f;
    private IInteractionSystem _interactionSystem;
    
    private void Awake()
    {
        _defaultPerspective = (_defaultTransform.position, _defaultTransform.rotation);
        _overviewPerspective = (_overviewTransform.position, _overviewTransform.rotation);
    }

    private void Start()
    {
        _interactionSystem = ServiceLocator.Get<IInteractionSystem>();
        _interactionSystem.InputActions.FindAction("Scroll").performed += OnScroll;
    }

    private void OnDisable()
    {
        _interactionSystem.InputActions.FindAction("Scroll").performed -= OnScroll;
    }

    private void Update()
    {
        if (!_isChangingPerspective) return;

        transform.position = Vector3.Slerp(_startPerspective.Item1, _targetPerspective.Item1, t);
        transform.rotation = Quaternion.Slerp(_startPerspective.Item2, _targetPerspective.Item2, t);
        t += Time.deltaTime / _transitionDuration;

        if (t >= 1f)
        {
            _isChangingPerspective = false;
            // _isInDefault = !_isInDefault;
        }
    }
    
    private void OnScroll(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<Vector2>().y;
        if (scroll == 0f || _interactionSystem.CurrentState is IInteractionSystem.State.Dragging) return;
        if (scroll > 0f && _isInDefault)
        {
            ChangeToOverview();
            _isInDefault = false;
        }
        else if (scroll < 0f && !_isInDefault)
        {
            ChangeToDefault();
            _isInDefault = true;
        }
    }

    public void ChangeToOverview()
    {
        //if (!_isInDefault && !_isChangingPerspective) return;
        _targetPerspective = _overviewPerspective;
        _startPerspective = _defaultPerspective;
        if (!_isChangingPerspective)
        {
            _isChangingPerspective = true;
            t = 0f;
        }
        else
        {
            t = 1f - t;
        }
    }

    public void ChangeToDefault()
    {
       // if (_isInDefault && !_isChangingPerspective) return;
        _targetPerspective = _defaultPerspective;
        _startPerspective = _overviewPerspective;
        if (!_isChangingPerspective)
        {
            _isChangingPerspective = true;
            t = 0f;
        }
        else
        {
            t = 1f - t;
        }
    }
}
