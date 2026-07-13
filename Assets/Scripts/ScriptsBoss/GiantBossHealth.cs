using UnityEngine;

[RequireComponent(typeof(GiantBossController))]
public class GiantBossHealth : MonoBehaviour
{
    [Header("Vida del Jefe Gigante")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Fase 2 (Enfurecido)")]
    [SerializeField, Range(0f, 1f)] private float enragePercentage = 0.20f; 
    [SerializeField] private float enragedSummonInterval = 5f; 
    [SerializeField] private int enragedMaxRobots = 4; 
    [SerializeField] private Color enragedColor = Color.red;

    [Header("CONFIGURACIÓN DE VICTORIA")]
    [SerializeField] private GameObject metaObject; // Aquí arrastras tu objeto META

    private bool isEnraged = false;
    private bool isDead = false; 

    private SpriteRenderer spriteRenderer;
    private GiantBossController bossController;
    private Animator anim; 

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>(); 
        bossController = GetComponent<GiantBossController>();
    }

    public void TakeDamage(int damage)
    {
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
        isDead = true; 
        Debug.Log("¡Jefe Gigante Derrotado!");

        if (anim != null)
        {
            anim.SetBool("estaMuerto", true);
        }

        if (bossController != null)
        {
            bossController.enabled = false;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 1. ELIMINAR ROBOTS MEDIANOS
        BossHealth[] todosLosMedianos = FindObjectsByType<BossHealth>(FindObjectsSortMode.None);
        foreach (BossHealth robotMediano in todosLosMedianos)
        {
            if (robotMediano != null && !robotMediano.IsDead && robotMediano.gameObject != this.gameObject)
            {
                robotMediano.TakeDamage(99999); 
            }
        }

        // ====================================================================
        // NUEVO: ELIMINAR ROBOTS PEQUEÑOS CON ESPADA
        // ====================================================================
        RobotEspadaController[] todosLosPequenos = FindObjectsByType<RobotEspadaController>(FindObjectsSortMode.None);
        foreach (RobotEspadaController robotPequeno in todosLosPequenos)
        {
            if (robotPequeno != null)
            {
                Destroy(robotPequeno.gameObject); // Los fulminamos de inmediato de la escena
            }
        }

        // 3. APARECER LA META
        if (metaObject != null)
        {
            metaObject.SetActive(true);
        }

        Destroy(gameObject, 2.5f); 
    }
}
