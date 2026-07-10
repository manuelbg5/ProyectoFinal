using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 2;
    private int currentHealth;
    private Animator anim;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    // Agrégalo dentro de tu clase HealthSystem, abajo de TakeDamage por ejemplo:
    public void Heal(int amount)
    {
        if (isDead) return;

        // Sumamos la cura a la vida actual
        currentHealth += amount;

        // Evitamos que la vida supere el máximo permitido
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("¡Jugador curado! Vida actual: " + currentHealth);
    }

    void Die()
    {
        isDead = true;
        anim.SetBool("estaMuerto", true);
        GetComponent<PlayerController>().enabled = false;
        // El Rigidbody se deja Dynamic: el cadáver mantiene la inercia del último knockback
        // y cae con gravedad — efecto parabólico estilo plataformero clásico.

        ManejadorCheckpoints gestor = FindAnyObjectByType<ManejadorCheckpoints>();
        if (gestor != null)
            Invoke(nameof(SolicitarRespawn), 2f);
    }

    void SolicitarRespawn()
    {
        // Si ya volvió a la vida (p. ej. el cadáver cayó en ZonaMuerte y ya respawneó), no hacer nada
        if (!isDead) return;
        FindAnyObjectByType<ManejadorCheckpoints>().Reaparecer();
    }

    // Llamado por ManejadorCheckpoints al respawnear
    public void RestaurarVida()
    {
        currentHealth = maxHealth;
        isDead = false;
        anim.SetBool("estaMuerto", false);
        GetComponent<PlayerController>().enabled = true;

        // Restaurar físicas del player
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
        }
    }

    public int GetHealth() => currentHealth;
}
