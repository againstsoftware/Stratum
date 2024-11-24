using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelizePass : ScriptableRenderPass
{
    // Almacenar ajustes
    private PixelizeFeature.CustomPassSettings _settings;

    // Textura de color de la cámara y búfer de píxeles
    private RenderTargetIdentifier _colorBuffer, _pixelBuffer;
    // ID para la escritura de la textura en cada frame
    private int _pixelBufferID = Shader.PropertyToID("_PixelBuffer");

    private Material _material;
    private int _pixelScreenHeight, _pixelScreenWidth;

    public PixelizePass(PixelizeFeature.CustomPassSettings settings)
    {
        _settings = settings;
        renderPassEvent = settings.RenderPassEvent;                                                     // Se asigna aquí pero el valor se ajusta en el inspector
        if(_material == null) _material = CoreUtils.CreateEngineMaterial("Custom/Pixelize");            // Si no tiene material se crea y asigna el del shader
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        // Se asigna el color del target de la cámara al búfer de color y se almacena el descriptor.
        _colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

        //cmd.GetTemporaryRT(pointBufferID, descriptor.width, descriptor.height, 0, FilterMode.Point);
        //pointBuffer = new RenderTargetIdentifier(pointBufferID);

        // Valores de resolución de pantalla usando aspect ratio
        _pixelScreenHeight = _settings.ScreenHeight;
        _pixelScreenWidth = (int) ( _pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f );

        // Block count calculado en cpu para no hacerlo en el shader.
        _material.SetVector("_BlockCount", new Vector2(_pixelScreenWidth, _pixelScreenHeight));
        _material.SetVector("_BlockSize", new Vector2(1.0f / _pixelScreenWidth, 1.0f / _pixelScreenHeight));
        _material.SetVector("_HalfBlockSize", new Vector2(0.5f / _pixelScreenWidth, 0.5f / _pixelScreenHeight));

        // Se copia la nueva altura y anchura
        descriptor.height = _pixelScreenHeight;
        descriptor.width = _pixelScreenWidth;

        // Filtrado de punto para la textura temporal porque si se deja en bilineal se ve borroso.
        cmd.GetTemporaryRT(_pixelBufferID, descriptor, FilterMode.Point);
        _pixelBuffer = new RenderTargetIdentifier(_pixelBufferID);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();                                                    // Pillar el commandbuffer de la pool
        using (new ProfilingScope(cmd, new ProfilingSampler("Pixelize Pass")))
        {
            // Para hacerlo sin shaders:
            //Blit(cmd, colorBuffer, pointBuffer);
            //Blit(cmd, pointBuffer, pixelBuffer);
            //Blit(cmd, pixelBuffer, colorBuffer);

            // Copiar el target de la cámara al búfer. Después se copia al revés para renderizar en pantalla.
            Blit(cmd, _colorBuffer, _pixelBuffer, _material);
            Blit(cmd, _pixelBuffer, _colorBuffer);
        }

        // Ejecutar el commandbuffer y liberarlo para evitar memory leaks.
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        // Liberar la textura temporal creada.
        if (cmd == null)
            throw new System.ArgumentNullException("cmd");
        cmd.ReleaseTemporaryRT(_pixelBufferID);
        //cmd.ReleaseTemporaryRT(pointBufferID);
    }
}