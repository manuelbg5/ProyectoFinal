using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida del Enemigo")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Aquí puedes agregar un sonido o partículas de explosión más adelante
        Destroy(gameObject); 
    }
}
