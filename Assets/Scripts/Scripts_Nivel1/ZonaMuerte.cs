using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Buscamos el gestor y le pedimos que reaparezca al jugador
            FindObjectOfType<ManejadorCheckpoints>().Reaparecer();
        }
    }
}
