using UnityEngine;
using UnityEngine.UI; // For debugging UI

[RequireComponent(typeof(Camera))]
public class xdRenderer : MonoBehaviour
{
    [SerializeField] private float downscaleFactor = 2f;
    [SerializeField] private bool showDebugInfo = true; // Toggle debug overlay
    [SerializeField] private KeyCode toggleKey = KeyCode.F1; // Toggle downscaling on/off

    private RenderTexture lowResRT;
    private Material upscaleMaterial;
    private bool isDownscalingEnabled = true;

    // Debug UI
    private GUIStyle debugStyle;

    void Start()
    {
        CreateResources();

        // Setup debug text style
        debugStyle = new GUIStyle();
        debugStyle.normal.textColor = Color.yellow;
        debugStyle.fontSize = 20;
    }

    void CreateResources()
    {
        // Round to even numbers to avoid artifacts
        int lowWidth = ((int)(Screen.width / downscaleFactor) / 2) * 2;
        int lowHeight = ((int)(Screen.height / downscaleFactor) / 2) * 2;

        if (lowResRT != null)
            lowResRT.Release();

        lowResRT = new RenderTexture(lowWidth, lowHeight, 24);
        lowResRT.filterMode = FilterMode.Bilinear;
        lowResRT.Create();

        if (upscaleMaterial == null)
        {
            upscaleMaterial = new Material(Shader.Find("Custom/SimpleUpscale"));
        }
        upscaleMaterial.SetFloat("_UpscaleFactor", downscaleFactor);

        Debug.Log($"Created RT: {lowWidth}x{lowHeight} (Original: {Screen.width}x{Screen.height})");
    }

    void Update()
    {
        // Toggle downscaling with F1
        if (Input.GetKeyDown(toggleKey))
        {
            isDownscalingEnabled = !isDownscalingEnabled;
            Debug.Log($"Downscaling: {( isDownscalingEnabled ? "Enabled" : "Disabled" )}");
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (isDownscalingEnabled)
        {
            Graphics.Blit(source, lowResRT);
            Graphics.Blit(lowResRT, destination, upscaleMaterial);
        }
        else
        {
            Graphics.Blit(source, destination); // Direct rendering at full res
        }
    }

    void OnGUI()
    {
        if (!showDebugInfo)
            return;

        int y = 10;
        GUI.Label(new Rect(10, y, 400, 25),
            $"Original Resolution: {Screen.width}x{Screen.height}", debugStyle);

        y += 25;
        GUI.Label(new Rect(10, y, 400, 25),
            $"Downscaled Resolution: {lowResRT.width}x{lowResRT.height}", debugStyle);

        y += 25;
        GUI.Label(new Rect(10, y, 400, 25),
            $"Downscaling: {( isDownscalingEnabled ? "Enabled" : "Disabled" )} (Press {toggleKey} to toggle)",
            debugStyle);

        y += 25;
        GUI.Label(new Rect(10, y, 400, 25),
            $"Scale Factor: {downscaleFactor}x", debugStyle);
    }

    void OnDestroy()
    {
        if (lowResRT != null)
        {
            lowResRT.Release();
            Destroy(lowResRT);
        }
        if (upscaleMaterial != null)
        {
            Destroy(upscaleMaterial);
        }
    }
}