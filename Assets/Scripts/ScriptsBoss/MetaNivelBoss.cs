using UnityEngine;
using UnityEngine.SceneManagement;

public class MetaNivelBoss : MonoBehaviour
{
    [Header("UI de Victoria")]
    public GameObject pantallaVictoria; 

    [Header("Configuración de Escena")]
    public string nombreSiguienteNivel;

    private bool nivelCompletado = false;

    void Update()
    {
        // Si el nivel ya se completó Y el jugador presiona Enter o Espacio
        if (nivelCompletado && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
        {
            CargarSiguienteNivel();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos si lo que entró es el jugador y si no hemos ganado ya
        if (other.CompareTag("Player") && !nivelCompletado)
        {
            Debug.Log("¡Meta alcanzada! Congelando al jugador...");
            nivelCompletado = true;

            // 1. Mostrar la pantalla de victoria
            if (pantallaVictoria != null)
            {
                pantallaVictoria.SetActive(true);
            }

            // 2. CONGELAR AL JUGADOR:
            // Intentamos buscar el componente de movimiento en el objeto que colisionó
            // NOTA: Si tu script de movimiento se llama diferente (ej. "PlayerController" o "MovimientoPlayer"), 
            // cambia la palabra "MonoBehaviour" de abajo por el nombre EXACTO de tu script de movimiento.
            PlayerController scriptMovimiento = other.GetComponent<PlayerController>(); 
            if (scriptMovimiento != null)
            {
                scriptMovimiento.enabled = false; // Desactiva por completo los controles del jugador
            }

            // 3. DETENER FÍSICAS:
            // Evita que el jugador siga deslizándose por inercia o cayendo si estaba en el aire
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // En versiones anteriores de Unity se usa: rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic; // Lo vuelve inmune a la gravedad y fuerzas externas
            }
        }
    }

    private void CargarSiguienteNivel()
    {
        if (!string.IsNullOrEmpty(nombreSiguienteNivel))
        {
            SceneManager.LoadScene(nombreSiguienteNivel);
        }
        else
        {
            Debug.LogWarning("¡Ojo! Olvidaste poner el nombre de la siguiente escena en el Inspector.");
        }
    }
}
