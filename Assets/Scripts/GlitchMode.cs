using System.Collections;
using UnityEngine;

public class GlitchMode : MonoBehaviour
{
    public GameObject normalWorld;
    public GameObject glitchWorld;

    public SpriteRenderer[] plataformasNormales;
    public SpriteRenderer[] plataformasGlitch;

    public Color colorNormal = Color.white;
    public Color colorGlitch = new Color(0.7f, 0.1f, 1f, 1f);

    public float transitionTime = 0.25f;

    [Header("Energía Glitch")]
    public float maxEnergy = 100f;
    public float drainRate = 25f;            // unidades/segundo gastadas mientras está activo
    public float rechargeRate = 15f;         // unidades/segundo recuperadas mientras está inactivo
    public float minEnergyToActivate = 10f;  // necesitas al menos esto para activar el glitch

    private float currentEnergy;
    private bool glitchActive = false;

    // Properties públicas para que el HUD lea el estado sin tocar campos privados
    public float CurrentEnergy => currentEnergy;
    public float MaxEnergy => maxEnergy;
    public float EnergyPercent => maxEnergy > 0 ? currentEnergy / maxEnergy : 0f;
    public bool GlitchActive => glitchActive;

    void Start()
    {
        // Auto-rellenado de ambos arrays si están vacíos
        if ((plataformasNormales == null || plataformasNormales.Length == 0) && normalWorld != null)
            plataformasNormales = normalWorld.GetComponentsInChildren<SpriteRenderer>(true);
        if ((plataformasGlitch == null || plataformasGlitch.Length == 0) && glitchWorld != null)
            plataformasGlitch = glitchWorld.GetComponentsInChildren<SpriteRenderer>(true);

        glitchActive = false;
        normalWorld.SetActive(true);
        glitchWorld.SetActive(false);
        currentEnergy = maxEnergy;

        CambiarColorInstantaneo(colorNormal);
    }

    [ContextMenu("Auto-rellenar Plataformas Normales")]
    void AutoRellenarPlataformas()
    {
        if (normalWorld == null)
        {
            Debug.LogWarning("Asignar 'Normal World' antes de auto-rellenar.");
            return;
        }
        plataformasNormales = normalWorld.GetComponentsInChildren<SpriteRenderer>(true);
        Debug.Log($"Plataformas Normales rellenadas con {plataformasNormales.Length} sprites.");
    }

    [ContextMenu("Auto-rellenar Plataformas Glitch")]
    void AutoRellenarPlataformasGlitch()
    {
        if (glitchWorld == null)
        {
            Debug.LogWarning("Asignar 'Glitch World' antes de auto-rellenar.");
            return;
        }
        plataformasGlitch = glitchWorld.GetComponentsInChildren<SpriteRenderer>(true);
        Debug.Log($"Plataformas Glitch rellenadas con {plataformasGlitch.Length} sprites.");
    }

    void Update()
    {
        // Gestión de energía: gastar mientras está activo, recargar mientras no
        if (glitchActive)
        {
            currentEnergy -= drainRate * Time.deltaTime;
            if (currentEnergy <= 0f)
            {
                currentEnergy = 0f;
                DesactivarGlitch(); // se acabó la energía → forzar apagado
            }
        }
        else
        {
            currentEnergy = Mathf.Min(currentEnergy + rechargeRate * Time.deltaTime, maxEnergy);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (glitchActive)
                DesactivarGlitch();
            else if (currentEnergy >= minEnergyToActivate)
                ActivarGlitch();
            // si no hay energía suficiente, se ignora la tecla
        }
    }

    void ActivarGlitch()
    {
        glitchActive = true;
        glitchWorld.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(CambiarColorSuave(colorGlitch));
    }

    void DesactivarGlitch()
    {
        glitchActive = false;
        glitchWorld.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(CambiarColorSuave(colorNormal));
    }

    void CambiarColorInstantaneo(Color color)
    {
        AplicarColorA(plataformasNormales, color);
        AplicarColorA(plataformasGlitch, color);
    }

    void AplicarColorA(SpriteRenderer[] sprites, Color color)
    {
        if (sprites == null) return;
        foreach (SpriteRenderer sr in sprites)
        {
            if (sr != null) sr.color = color;
        }
    }

    IEnumerator CambiarColorSuave(Color targetColor)
    {
        // Guardamos los colores iniciales de los dos arrays
        Color[] iniNorm = CapturarColores(plataformasNormales);
        Color[] iniGlitch = CapturarColores(plataformasGlitch);

        float time = 0f;
        while (time < transitionTime)
        {
            time += Time.deltaTime;
            float t = time / transitionTime;

            InterpolarColor(plataformasNormales, iniNorm, targetColor, t);
            InterpolarColor(plataformasGlitch, iniGlitch, targetColor, t);

            yield return null;
        }
    }

    Color[] CapturarColores(SpriteRenderer[] sprites)
    {
        if (sprites == null) return new Color[0];
        Color[] result = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
            result[i] = sprites[i] != null ? sprites[i].color : Color.white;
        return result;
    }

    void InterpolarColor(SpriteRenderer[] sprites, Color[] iniciales, Color target, float t)
    {
        if (sprites == null) return;
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null)
                sprites[i].color = Color.Lerp(iniciales[i], target, t);
        }
    }
}