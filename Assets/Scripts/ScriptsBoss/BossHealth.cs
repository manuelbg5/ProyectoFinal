using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Vida del jefe")]
    [SerializeField] private int maxHealth = 300;
    [SerializeField] private int currentHealth;

    private bool isDead;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Vida del Boss: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("Boss derrotado");

        Collider2D bossCollider = GetComponent<Collider2D>();

        if (bossCollider != null)
        {
            bossCollider.enabled = false;
        }
    }
}