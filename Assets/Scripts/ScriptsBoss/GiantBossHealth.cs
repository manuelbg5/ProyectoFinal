using UnityEngine;

[RequireComponent(typeof(GiantBossController))]
public class GiantBossHealth : MonoBehaviour
{
    [Header("Vida del Jefe Gigante")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Fase 2 (Enfurecido)")]
    [SerializeField, Range(0f, 1f)] private float enragePercentage = 0.20f; // 20%
    [SerializeField] private float enragedSummonInterval = 5f; 
    [SerializeField] private int enragedMaxRobots = 4; 
    [SerializeField] private Color enragedColor = Color.red;

    private bool isEnraged = false;
    private bool isDead = false; // <-- Nuevo: para saber si ya murió

    private SpriteRenderer spriteRenderer;
    private GiantBossController bossController;
    private Animator anim; // <-- Nuevo: referencia al animador

    private void Start()
    {
        currentHealth = maxHealth;
        
        // Buscamos los componentes en el hijo (BossVisual)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>(); 
        
        bossController = GetComponent<GiantBossController>();
    }

    public void TakeDamage(int damage)
    {
        // Si ya está muerto, ignoramos el daño extra
        if (isDead || currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log("Vida del Jefe Gigante: " + currentHealth);

        if (!isEnraged && currentHealth <= (maxHealth * enragePercentage))
        {
            EntrarFaseEnfurecida();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void EntrarFaseEnfurecida()
    {
        isEnraged = true;
        Debug.Log("¡El Jefe Gigante se ha enfurecido!");

        if (spriteRenderer != null)
        {
            spriteRenderer.color = enragedColor;
        }

        if (bossController != null)
        {
            bossController.CambiarIntervaloDeInvocacion(enragedSummonInterval, enragedMaxRobots);
        }
    }

    private void Die()
    {
        isDead = true; // Marcamos que ya murió
        Debug.Log("¡Jefe Gigante Derrotado!");

        // 1. Activamos la animación
        if (anim != null)
        {
            anim.SetBool("estaMuerto", true);
        }

        // 2. Apagamos el script de invocaciones para que deje de tirar robots
        if (bossController != null)
        {
            bossController.enabled = false;
        }

        // 3. Apagamos su Collider para que las balas ya no choquen contra él
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 4. Destruimos el objeto con un RETRASO (ej. 2 segundos) para dejar que la animación se vea.
        // ¡Cambia el 2f por los segundos exactos que dura tu animación!
        Destroy(gameObject, 2.5f); 
    }
}
