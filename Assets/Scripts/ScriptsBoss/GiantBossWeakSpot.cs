using UnityEngine;

public class GiantBossWeakSpot : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GiantBossHealth mainBossHealth;
    [SerializeField] private int criticalDamage = 8; // Daño aumentado (pasa de 4 a 8)

    private void Start()
    {
        // Intenta buscar el script de salud en el objeto padre si no se asignó a mano
        if (mainBossHealth == null)
        {
            mainBossHealth = GetComponentInParent<GiantBossHealth>();
        }
    }

    // OPCIÓN 1: Si tus balas interactúan usando Triggers (Pasar a través/Chocar)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // IMPORTANTE: Cambia "Bullet" por el Tag exacto que usen las balas de tu jugador
        if (collision.CompareTag("Bullet"))
        {
            if (mainBossHealth != null)
            {
                mainBossHealth.TakeDamage(criticalDamage); // Le aplica el daño de 8 directamente al jefe principal
                Destroy(collision.gameObject); // Destruye la bala para que no atraviese la cabeza e impacte el cuerpo
            }
        }
    }

    // OPCIÓN 2: Si el script de la bala de tu jugador busca un método llamado "TakeDamage"
    public void TakeDamage(int normalDamage)
    {
        if (mainBossHealth != null)
        {
            // Multiplicamos por 2 el daño que reciba la cabeza
            mainBossHealth.TakeDamage(normalDamage * 2); 
        }
    }
}
