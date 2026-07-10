using UnityEngine;
using UnityEngine.UI;

public class HUDVida : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private HealthSystem playerHealth;
    [SerializeField] private Image[] corazones;

    [Header("Sprites")]
    [SerializeField] private Sprite spriteCorazonLleno;
    [SerializeField] private Sprite spriteCorazonVacio; // opcional — si lo dejas vacío, los corazones perdidos se ocultan

    void Start()
    {
        // Si no se asignó el HealthSystem, lo buscamos por tag
        if (playerHealth == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) playerHealth = p.GetComponent<HealthSystem>();
        }
    }

    void Update()
    {
        if (playerHealth == null || corazones == null) return;

        int vida = playerHealth.GetHealth();

        for (int i = 0; i < corazones.Length; i++)
        {
            if (corazones[i] == null) continue;

            if (i < vida)
            {
                // Corazón lleno
                if (spriteCorazonLleno != null) corazones[i].sprite = spriteCorazonLleno;
                corazones[i].enabled = true;
            }
            else
            {
                // Corazón perdido
                if (spriteCorazonVacio != null)
                {
                    corazones[i].sprite = spriteCorazonVacio;
                    corazones[i].enabled = true;
                }
                else
                {
                    corazones[i].enabled = false;
                }
            }
        }
    }
}
