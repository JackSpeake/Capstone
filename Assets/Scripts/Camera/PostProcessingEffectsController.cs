using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    Vignette currVignette;


    public void SetColor(Color tintColor)
    {
        this.tintColor = tintColor;
        this.tintColor.a = 1;
        if (currVignette)
            currVignette.color.value = this.tintColor;
            
    }

    public void SetVignetteActive(float active, Volume volume)
    {
        Vignette tmp;
        if (volume && volume.sharedProfile.TryGet<Vignette>(out tmp))
        {
            Debug.Log("Work pog!!!");
            currVignette = tmp;
            currVignette.active = (active == 1.0);
            currVignette.color = new ColorParameter(tintColor, true, true, false, true);
            // currVignette.
        }
    }

    /*private void OnRenderImage(RenderTexture source, RenderTexture destination)
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
    }*/
}
