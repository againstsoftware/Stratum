using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraMovement : MonoBehaviour
{
    private enum CameraState { Moving, Returning, Idle }
    private CameraState _currentState = CameraState.Idle;
    private Camera _camera;
    private Vector3 _initPosition;
    private Quaternion _initRotation;
    private Vector3 _initVelocity;
    private Vector3 _posToMove;
    private Vector3 _objectPos;
    [SerializeField] private float _smoothVelocity = 0.3f;
    [SerializeField] private float _rotationSpeed = 2f;
    [SerializeField] private Vector3 _cameraOffset = new Vector3(0, 2, -3);

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _initPosition = _camera.transform.position;
        _initRotation = _camera.transform.rotation;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case CameraState.Moving:
                MoveCamera();
                break;

            case CameraState.Returning:
                ReturnCamera();
                break;

            case CameraState.Idle:
                break;
        }
    }

    public void StartMoving(Vector3 pos)
    {
        _posToMove = pos + _cameraOffset;;
        _objectPos = pos;
        _currentState = CameraState.Moving;
    }

    public void StartReturning()
    {
        _posToMove = _initPosition;
        _currentState = CameraState.Returning;
    }

    private void MoveCamera()
    {
        // movimiento
        _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, _posToMove, ref _initVelocity, _smoothVelocity);

        // rotacion
        Quaternion targetRotation = Quaternion.LookRotation(_objectPos - _camera.transform.position);
        _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(_camera.transform.position, _posToMove) < 0.01f)
        {
            _currentState = CameraState.Idle;
        }
    }

    private void ReturnCamera()
    {
        _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, _initPosition, ref _initVelocity, _smoothVelocity);

        _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, _initRotation, _rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(_camera.transform.position, _initPosition) < 0.01f)
        {
            _currentState = CameraState.Idle;
        }
    }
}
