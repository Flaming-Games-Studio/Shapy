using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BatteryShaderController : MonoBehaviour
{
    public Material batteryShaderMaterial;
    public Color coilColor;
    public Texture2D coilTexture;
    public Vector2 scrollSpeed;
    public Vector2 baseTiling;
    public Vector2 emissionTiling;
    public Color emissionColor;
    public Texture2D emissionTexture;

    public float emissionIntensity;

    private void Start()
    {
        batteryShaderMaterial.SetColor("_CoilColor", coilColor);
        batteryShaderMaterial.SetTexture("_BaseMap", coilTexture);
        batteryShaderMaterial.SetVector("_ScrollSpeed", scrollSpeed);
        batteryShaderMaterial.SetVector("_BaseTiling", baseTiling);
        batteryShaderMaterial.SetVector("_EmissionTiling", emissionTiling);
        batteryShaderMaterial.SetColor("_EmissionColor", new Color(emissionColor.r, emissionColor.g, emissionColor.b, 1f));
        batteryShaderMaterial.SetTexture("_EmissionTexture", emissionTexture);
        batteryShaderMaterial.EnableKeyword("_EMISSION");

        InvokeRepeating("SetEmissionColor", 3, 3);
        InvokeRepeating("SetEmissionIntensity", 3, 3);
    }

    public void SetEmissionColor()
    {
        // Update the emission color
        batteryShaderMaterial.SetColor("_EmissionColor", new Color(emissionColor.r, emissionColor.g, emissionColor.b, 1f));
        batteryShaderMaterial.SetColor("_CoilColor", coilColor);
        batteryShaderMaterial.SetTexture("_BaseMap", coilTexture);
        batteryShaderMaterial.SetVector("_ScrollSpeed", scrollSpeed);
        batteryShaderMaterial.SetVector("_BaseTiling", baseTiling);
        batteryShaderMaterial.SetVector("_EmissionTiling", emissionTiling);
        batteryShaderMaterial.SetTexture("_EmissionTexture", emissionTexture);
    }

    public void SetEmissionIntensity()
    {
        // Update the emission intensity
        batteryShaderMaterial.SetColor("_EmissionColor", new Color(emissionColor.r * emissionIntensity, emissionColor.g * emissionIntensity, emissionColor.b * emissionIntensity, 1f));
    }
}
