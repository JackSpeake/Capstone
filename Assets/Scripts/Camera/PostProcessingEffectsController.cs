using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessingEffectsController : MonoBehaviour
{
    public Material postProcessingEffectsMaterial;
    [Tooltip("Radius of the vignette")]
    [SerializeField] private float radius;
    [Tooltip("Strength of feathering effect")]
    [SerializeField] private float feathering;
    [Tooltip("Color of the vignette")]
    private Color tintColor;
    private float vignetteActive = 0.0f; // shaders don't support bools, so using a float instead

    public void SetColor(Color tintColor)
    {
        this.tintColor = tintColor;
    }

    public void SetVignetteActive(float active)
    {
        this.vignetteActive = active;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int width = source.width;
        int height = source.height;

        RenderTexture startRenderTexture = RenderTexture.GetTemporary(width, height);

        postProcessingEffectsMaterial.SetFloat("_Radius", radius);
        postProcessingEffectsMaterial.SetFloat("_Feathering", radius);
        postProcessingEffectsMaterial.SetColor("_TintColor", tintColor);
        postProcessingEffectsMaterial.SetFloat("_VignetteActive", vignetteActive);
        

        Graphics.Blit(source, startRenderTexture, postProcessingEffectsMaterial);
        Graphics.Blit(startRenderTexture, destination);
        RenderTexture.ReleaseTemporary(startRenderTexture);
    }
}
