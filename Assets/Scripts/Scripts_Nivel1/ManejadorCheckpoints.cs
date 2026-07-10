using UnityEngine;

public class ManejadorCheckpoints : MonoBehaviour
{
    // Guardamos la posición del último punto tocado
    public Vector3 posicionUltimoCheckpoint;
    public GameObject jugador;

    void Start()
    {
        // Al empezar, el primer checkpoint es la posición inicial
        posicionUltimoCheckpoint = jugador.transform.position;
    }

    public void Reaparecer()
    {
        // Movemos al jugador a la posición guardada
        jugador.transform.position = posicionUltimoCheckpoint;

        // Restauramos su vida y estado al máximo
        HealthSystem vida = jugador.GetComponent<HealthSystem>();
        if (vida != null)
            vida.RestaurarVida();
    }

    // Método para actualizar el punto de guardado
    public void ActualizarCheckpoint(Vector3 nuevaPosicion)
    {
        posicionUltimoCheckpoint = nuevaPosicion;
    }
}
