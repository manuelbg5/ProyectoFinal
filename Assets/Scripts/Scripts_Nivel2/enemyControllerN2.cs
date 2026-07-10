using UnityEngine;

public class enemyControllerN2 : MonoBehaviour, IDamageable
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float chaseVerticalLimit = 2.5f;
    [SerializeField] private float chaseDeadzoneX = 0.4f;

    [Header("Disparo")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float shootRange = 6f;       // distancia a la que empieza a disparar
    [SerializeField] private float shootVerticalLimit = 2f; // margen vertical para disparar

    [Header("Detecci�n de Suelo / Borde")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float edgeCheckDistance = 0.5f;
    [SerializeField] private float edgeCheckDepth = 1f;
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private float wallCheckUpperHeight = 0.4f;

    [Header("Knockback al contacto")]
    [SerializeField] private float knockbackX = 6f;
    [SerializeField] private float knockbackY = 0f;
    [SerializeField] private float slideDuration = 0.2f;

    [Header("Resbalar al player encima")]
    [SerializeField] private float topThreshold = 0.3f;
    [SerializeField] private float slideForce = 5f;

    [Header("Sistema de Vida")]
    [SerializeField] private int maxHealth = 8;
    private int currentHealth;

    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;
    private float lastFireTime;
    private bool isDead = false;
    private bool isTouchingPlayer = false;
    private float patrolDirection = 1f;

    private enum State { Patrol, Chase, Shoot, Dead }
    private State currentState = State.Patrol;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("No se encontr� el Player.");

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        float vertDiff = Mathf.Abs(player.position.y - transform.position.y);

        // Contacto directo: knockback sin dejar de disparar
        if (isTouchingPlayer)
            ApplyContactKnockback();

        // Rango de disparo: se detiene y dispara
        if (dist <= shootRange && vertDiff <= shootVerticalLimit)
            ShootState();
        // Rango de detecci�n: persigue
        else if (dist <= detectionRange && vertDiff <= chaseVerticalLimit)
            ChasePlayer();
        else
            Patrol();
    }

    void Patrol()
    {
        currentState = State.Patrol;

        if (!HaySueloAdelante(patrolDirection) || HayMuroAdelante(patrolDirection))
            patrolDirection *= -1f;

        Mover(patrolDirection);
    }

    void ChasePlayer()
    {
        currentState = State.Chase;

        float xDiff = player.position.x - transform.position.x;

        if (Mathf.Abs(xDiff) < chaseDeadzoneX)
        {
            OrientarSprite(patrolDirection);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            anim.SetFloat("velocidad", 0f);
            return;
        }

        float direction = xDiff > 0 ? 1f : -1f;

        if (!HaySueloAdelante(direction) || HayMuroAdelante(direction))
        {
            OrientarSprite(direction);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            anim.SetFloat("velocidad", 0f);
            return;
        }

        patrolDirection = direction;
        Mover(direction);
    }

    void ShootState()
    {
        currentState = State.Shoot;

        // Se detiene y mira al player
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        anim.SetFloat("velocidad", 0f);

        float xDiff = player.position.x - transform.position.x;
        OrientarSprite(xDiff > 0 ? 1f : -1f);

        // Dispara seg�n fireRate
        if (Time.time >= lastFireTime + fireRate)
        {
            lastFireTime = Time.time;
            Disparar();
            anim.SetTrigger("disparar");
        }
    }

    void Disparar()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // 1. Instanciamos la bala
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 2. Calculamos la dirección en X hacia el jugador
        float dirX = player.position.x > transform.position.x ? 1f : -1f;

        // 3. Le pasamos la dirección al script de la bala (Esto soluciona la dirección y el comportamiento)
        bullet bulletScript = bulletObj.GetComponent<bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(dirX);
        }

        // 4. Ignorar colisiones entre el clon del enemigo y su propia bala
        Collider2D bulletCol = bulletObj.GetComponent<Collider2D>();
        Collider2D myCol = GetComponent<Collider2D>();
        if (bulletCol != null && myCol != null)
            Physics2D.IgnoreCollision(bulletCol, myCol);
    }

    void ApplyContactKnockback()
    {
        if (player == null) return;
        PlayerController playerCtrl = player.GetComponent<PlayerController>();
        if (playerCtrl == null) return;

        if (Time.time >= lastFireTime + 0.5f) // cooldown de contacto
        {
            lastFireTime = Time.time;
            float kbDir = player.position.x >= transform.position.x ? 1f : -1f;
            playerCtrl.ApplyKnockback(new Vector2(kbDir * knockbackX, knockbackY), slideDuration);

            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
                playerHealth.TakeDamage(1);
        }
    }

    void Mover(float direction)
    {
        OrientarSprite(direction);
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        anim.SetFloat("velocidad", 1f);
    }

    void OrientarSprite(float direction)
    {
        transform.localScale = new Vector3(
            direction * Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }

    bool HaySueloAdelante(float direction)
    {
        Vector2 origin = (Vector2)transform.position + Vector2.right * direction * edgeCheckDistance;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, edgeCheckDepth, groundMask);
        return hit.collider != null;
    }

    bool HayMuroAdelante(float direction)
    {
        return WallCheckAtHeight(direction, 0f) || WallCheckAtHeight(direction, wallCheckUpperHeight);
    }

    bool WallCheckAtHeight(float direction, float height)
    {
        Vector2 origin = (Vector2)transform.position + Vector2.up * height;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.right * direction, wallCheckDistance, groundMask);
        foreach (var h in hits)
        {
            if (h.collider == null) continue;
            if (h.collider.gameObject == gameObject) continue;
            if (h.collider.CompareTag("Enemy")) continue;
            if (h.collider.GetComponent<enemyControllerN2>() != null) continue;
            return true;
        }
        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isTouchingPlayer = true;
        else if (collision.gameObject.CompareTag("Enemy") ||
                 collision.gameObject.GetComponent<enemyControllerN2>() != null)
            patrolDirection *= -1f;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isTouchingPlayer = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead || !collision.gameObject.CompareTag("Player")) return;

        float verticalDiff = collision.transform.position.y - transform.position.y;
        if (verticalDiff < topThreshold) return;

        PlayerController playerCtrl = collision.gameObject.GetComponent<PlayerController>();
        if (playerCtrl == null) return;

        float horizontalDiff = collision.transform.position.x - transform.position.x;
        float kbDir = Mathf.Abs(horizontalDiff) < 0.05f
            ? -Mathf.Sign(transform.localScale.x)
            : Mathf.Sign(horizontalDiff);

        Rigidbody2D playerRb = collision.rigidbody;
        float vy = playerRb != null ? playerRb.linearVelocity.y : 0f;
        playerCtrl.ApplyKnockback(new Vector2(kbDir * slideForce, vy), slideDuration);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        isTouchingPlayer = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        anim.SetBool("estaMuerto", true);
        anim.SetFloat("velocidad", 0f);
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; // ← null check
        
        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        float dir = patrolDirection != 0 ? patrolDirection : 1f;
        Vector3 origin = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, shootRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, detectionRange);

        Vector3 edgeOrigin = origin + Vector3.right * dir * edgeCheckDistance;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(edgeOrigin, edgeOrigin + Vector3.down * edgeCheckDepth);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(origin, origin + Vector3.right * dir * wallCheckDistance);
        Vector3 upperOrigin = origin + Vector3.up * wallCheckUpperHeight;
        Gizmos.DrawLine(upperOrigin, upperOrigin + Vector3.right * dir * wallCheckDistance);
    }
}
