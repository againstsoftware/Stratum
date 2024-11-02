using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Platform.Android;

public class MenuObject : MonoBehaviour, IMenuInteractable
{
    private Vector3 initVelocity = Vector3.zero;
    private Vector3 posToMove;
    private float smoothVelocity = 0.4f;
    private Camera _camera;
    private bool isMoving = false;
    private bool isReturning = false;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 2, -3);
    [SerializeField] private float rotationSpeed = 3f;
    private Vector3 cameraInitPosition = new Vector3(0, 1, -10);

    public void OnPointerPress(Camera camera)
    {
        _camera = camera;
        posToMove = transform.position + cameraOffset;

        isMoving = true;
    }

    public void ResetCamera(Camera camera)
    {
        _camera = camera;
        posToMove = cameraInitPosition;

        isReturning = true;
    }

    void Update()
    {
        // mirar si esto convendría más en otra clase para cameraMovement
        if (isMoving)
        {
            // movimiento
            _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, posToMove, ref initVelocity, smoothVelocity);

            // rotacion
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
            _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Vector3.Distance(_camera.transform.position, posToMove) < 0.01f)
            {
                isMoving = false;

                // Debug: para que vuelva al sitio la camara
                //StartCoroutine(EsperarDosSegundos(_camera)); 
            }
        }

        if (isReturning)
        {
            _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, posToMove, ref initVelocity, smoothVelocity);

            // rotacion
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
            _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Vector3.Distance(_camera.transform.position, posToMove) < 0.01f)
            {
                isReturning = false;
            }
            //MoveCamera();
        }
    }

    private void MoveCamera()
    {
        _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, posToMove, ref initVelocity, smoothVelocity);

        // rotacion
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
        _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(_camera.transform.position, posToMove) < 0.01f)
        {
            isReturning = false;
        }
    }

    // debug
    private IEnumerator EsperarDosSegundos(Camera camera)
    {
        yield return new WaitForSeconds(2f);
        camera.transform.position = new Vector3(0, 1, -10);
    }
}
