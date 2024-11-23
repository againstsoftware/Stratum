using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class UpscaleRenderer : MonoBehaviour
{
    [SerializeField] private float _downscaleFactor = 2.0f;
    private Camera _mainCamera;
    private RenderTexture _lowResRT;
    private Material _upscaleMaterial;

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = GetComponent<Camera>();

        // Create Render Textures
        CreateRenderTexture();

        // Load the upscaling shader
        _upscaleMaterial = new Material(Shader.Find("Custom/UpscalePass"));
        _upscaleMaterial.SetFloat("_UpscaleFactor", _downscaleFactor);
    }

    public void OnPreRender()
    {
        // Set camera to render to low res texture before rendering
        _mainCamera.targetTexture = _lowResRT;
    }

    public void OnPostRender()
    {
        // Clear target texture after rendering to render to the screen
        _mainCamera.targetTexture = null;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Upscale the low resolution render texture (game image texture)
        Graphics.Blit(_lowResRT, destination, _upscaleMaterial);
    }

    void OnDestroy()
    {
        if (_lowResRT != null) { _lowResRT.Release(); }
    }

    void CreateRenderTexture()
    {
        int lowWidth  = (int)(Screen.width / _downscaleFactor);
        int lowHeight = (int)(Screen.height / _downscaleFactor);

        _lowResRT = new RenderTexture(lowWidth, lowHeight, 24);

        _mainCamera.targetTexture = _lowResRT;
    }
}
