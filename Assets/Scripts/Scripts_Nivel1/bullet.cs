using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private int damage = 1;
    
    [Header("Configuración de Bando")]
    [SerializeField] private bool isEnemyBullet = false; // ACTIVA ESTO SOLO EN EL PREFAB DE LA BALA ENEMIGA

    private float direction = 1f;

    public void SetDirection(float dir)
    {
        direction = dir;
        if (dir < 0)
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * bulletSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ====================================================================
        // 1. DETECCIÓN DE CHOQUE ENTRE BALAS (Bala del Jugador vs Bala del Enemigo)
        // ====================================================================
        bullet otherBullet = other.GetComponent<bullet>();
        if (otherBullet != null)
        {
            // Solo se destruyen si son de bandos opuestos (evita que tus propias balas se destruyan entre sí)
            if (this.isEnemyBullet != otherBullet.isEnemyBullet)
            {
                // Aquí puedes instanciar un pequeño efecto de explosión si quieres antes de destruir
                Destroy(gameObject);
                return; // Importante poner 'return' para que no ejecute el resto del código
            }
        }

        // ====================================================================
        // 2. CASO JUGADOR: Es bala del JUGADOR y golpea a un ENEMIGO
        // ====================================================================
        if (!isEnemyBullet && other.CompareTag("Enemy"))
        {
            enemyController enemy = other.GetComponent<enemyController>();
            if (enemy != null) enemy.TakeDamage(damage);

            enemyControllerN2 enemyN2 = other.GetComponent<enemyControllerN2>();
            if (enemyN2 != null) enemyN2.TakeDamage(damage);

            Destroy(gameObject);
        }

        // ====================================================================
        // 3. CASO ENEMIGO: Es bala del ENEMIGO y golpea al JUGADOR
        // ====================================================================
        if (isEnemyBullet && other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        // ====================================================================
        // 4. DESTRUCCIÓN GENERAL (Suelo/Paredes)
        // ====================================================================
        if (other.CompareTag("Ground"))
            Destroy(gameObject);
    }
}
