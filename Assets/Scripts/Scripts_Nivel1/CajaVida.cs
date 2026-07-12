using UnityEngine;

public class CajaVida : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int cantidadCura = 2; // Si cura un corazón, debería ser 2

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem sistemaVida = other.GetComponent<HealthSystem>();

            if (sistemaVida != null)
            {
                // Ahora verifica si la vida es menor al MÁXIMO, no a 7
                if (sistemaVida.GetHealth() < sistemaVida.GetMaxHealth()) 
                {
                    sistemaVida.Heal(cantidadCura);
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("El jugador ya tiene la vida al máximo.");
                }
            }
        }
    }
}
