using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private Slider healthSlider;

    private void Start()
    {
        if (bossHealth == null || healthSlider == null)
        {
            Debug.LogError("Faltan referencias en BossHealthUI.");
            return;
        }

        healthSlider.minValue = 0;
        healthSlider.maxValue = bossHealth.MaxHealth;
        healthSlider.value = bossHealth.CurrentHealth;
    }

    private void Update()
    {
        if (bossHealth == null || healthSlider == null)
            return;

        healthSlider.value = bossHealth.CurrentHealth;
    }
}