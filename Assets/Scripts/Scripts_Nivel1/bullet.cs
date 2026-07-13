using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private int damage = 1;

    [Header("Configuración de Bando")]
    [SerializeField] private bool isEnemyBullet = false;

    [Header("Duración")]
    [SerializeField] private float lifeTime = 4f;

    private float direction = 1f;

    private void Start()
    {
        // Evita que las balas permanezcan para siempre fuera de la cámara.
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(float dir)
    {
        direction = Mathf.Sign(dir);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    private void Update()
    {
        transform.Translate(
            Vector2.right * direction * bulletSpeed * Time.deltaTime,
            Space.World
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ================================================================
        // 1. CHOQUE ENTRE BALAS DE BANDOS OPUESTOS
        // ================================================================

        bullet otherBullet = other.GetComponent<bullet>();

        if (otherBullet != null)
        {
            if (isEnemyBullet != otherBullet.isEnemyBullet)
            {
                Destroy(otherBullet.gameObject);
                Destroy(gameObject);
            }

            return;
        }

        // ================================================================
        // 2. BALA DEL JUGADOR CONTRA EL BOSS
        // ================================================================

        if (!isEnemyBullet)
        {
            BossHealth bossHealth = other.GetComponentInParent<BossHealth>();

            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        // ================================================================
        // 2.5 BALA DEL JUGADOR CONTRA EL JEFE GIGANTE
        // ================================================================
        if (!isEnemyBullet)
        {
            GiantBossHealth giantBoss = other.GetComponentInParent<GiantBossHealth>();
            if (giantBoss != null)
            {
                giantBoss.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        // ================================================================
        // 3. BALA DEL JUGADOR CONTRA ENEMIGOS NORMALES
        // ================================================================

        if (!isEnemyBullet && other.CompareTag("Enemy"))
        {
            enemyController enemy =
                other.GetComponentInParent<enemyController>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            enemyControllerN2 enemyN2 =
                other.GetComponentInParent<enemyControllerN2>();

            if (enemyN2 != null)
            {
                enemyN2.TakeDamage(damage);
            }

            // --- ESTO ES LO NUEVO PARA LOS ROBOTS ---
            EnemyHealth robotHealth = other.GetComponentInParent<EnemyHealth>();
            if (robotHealth != null)
            {
                robotHealth.TakeDamage(damage);
            }
            // ----------------------------------------

            Destroy(gameObject);
            return;
        }

        // ================================================================
        // 4. BALA ENEMIGA CONTRA EL JUGADOR
        // ================================================================

        if (isEnemyBullet && other.CompareTag("Player"))
        {
            HealthSystem playerHealth =
                other.GetComponentInParent<HealthSystem>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        // ================================================================
        // 5. SUELO Y PAREDES
        // ================================================================

        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}