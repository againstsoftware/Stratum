// Shoutout para @malimakes en YouTube por la ayuda con el shader
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Esta clase genera la funcionalidad en el URP.
// Necesita heredar de ScriptableRendererFeature para poder hacer override en Create y AddRenderPasses
public class PixelizeFeature : ScriptableRendererFeature
{
    // Se crea esta clase para contener los ajustes de la pasada. Aquí se ajusta la altura del render.
    [System.Serializable]
    public class CustomPassSettings
    {
        public RenderPassEvent RenderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public int ScreenHeight = 144;
    }

    // Variables para ajustes y el objeto de la pasada custom.
    [SerializeField] private CustomPassSettings _settings;
    private PixelizePass _customPass;

    //Inicializa la llamada a la pasada custom.
    public override void Create()
    {
        _customPass = new PixelizePass(_settings);
    }

    // Añade la pasada a la cola del motor de render.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera)
            return;
#endif
        renderer.EnqueuePass(_customPass);
    }
}
