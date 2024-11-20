using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCelShader : MonoBehaviour
{
    [SerializeField] private Shader _celShader;
    private Material _celMaterial;

    void OnEnable()
    {
        if (_celShader != null && _celMaterial == null)
        {
            _celMaterial = new Material(_celShader);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_celMaterial != null)
        {
            // Make sure your properties match the ones in your shader
            _celMaterial.SetFloat("_Lighting_cutoff", 0.3f); // Match your shader property
            _celMaterial.SetFloat("_Shadow_Brightness", 0.5f); // Match your shader property
            Graphics.Blit(src, dest, _celMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    void OnDisable()
    {
        if (_celMaterial != null)
        {
            if (Application.isEditor)
                DestroyImmediate(_celMaterial);
            else
                Destroy(_celMaterial);
        }
    }
}