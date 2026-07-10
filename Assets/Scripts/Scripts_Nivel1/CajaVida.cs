using UnityEngine;

public class CajaVida : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int cantidadCura = 1; // Cuántos puntos de vida va a recuperar

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos si lo que tocó la cajita es el Jugador
        if (other.CompareTag("Player"))
        {
            // Intentamos obtener el sistema de vida del jugador
            HealthSystem sistemaVida = other.GetComponent<HealthSystem>();

            if (sistemaVida != null)
            {
                // Opcional: Solo agarrar la cura si al jugador realmente le falta vida
                if (sistemaVida.GetHealth() < 7) 
                {
                    // Curamos al jugador
                    sistemaVida.Heal(cantidadCura);

                    // Destruimos la cajita para que desaparezca del mapa
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
