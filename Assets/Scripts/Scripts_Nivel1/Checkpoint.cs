using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private ManejadorCheckpoints gestor;

    void Start()
    {
        gestor = FindObjectOfType<ManejadorCheckpoints>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Le enviamos la posición de este objeto al gestor
            gestor.ActualizarCheckpoint(transform.position);
            
            // Opcional: Cambiar color o activar luz para dar feedback visual
            GetComponent<SpriteRenderer>().color = Color.cyan; 
            Debug.Log("¡Checkpoint guardado!");
        }
    }
}
