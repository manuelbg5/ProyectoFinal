using UnityEngine;
using UnityEngine.UI;

public class BarraGlitch : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GlitchMode glitchMode;
    [SerializeField] private Image fillImage;        // Image con Type = Filled (Horizontal o Radial)

    [Header("Colores")]
    [SerializeField] private Color colorNormal = new Color(0.7f, 0.1f, 1f, 1f);
    [SerializeField] private Color colorBajo   = Color.red;
    [SerializeField] private float umbralBajo  = 0.25f; // bajo este porcentaje, la barra se pinta de "bajo"

    void Start()
    {
        if (glitchMode == null)
            glitchMode = FindAnyObjectByType<GlitchMode>();
    }

    void Update()
    {
        if (glitchMode == null || fillImage == null) return;

        float pct = glitchMode.EnergyPercent;
        fillImage.fillAmount = pct;
        fillImage.color = pct <= umbralBajo ? colorBajo : colorNormal;
    }
}
