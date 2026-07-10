using UnityEngine;

public class enemyController : MonoBehaviour, IDamageable
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float chaseVerticalLimit = 2.5f; // si el player está más arriba/abajo que esto, no lo persigue (está en otra plataforma)
    [SerializeField] private float chaseDeadzoneX = 0.4f;     // zona muerta lateral para no oscilar cuando el player está casi alineado

    [Header("Detección de Suelo / Borde")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float edgeCheckDistance = 0.5f;
    [SerializeField] private float edgeCheckDepth = 1f;
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private float wallCheckUpperHeight = 0.4f; // altura del segundo raycast, para detectar muros que no estén a la altura del centro

    [Header("Knockback al atacar")]
    [SerializeField] private float knockbackX = 6f;
    [SerializeField] private float knockbackY = 0f;

    [Header("Resbalar al player encima")]
    [SerializeField] private float topThreshold = 0.3f; // diferencia vertical mínima para considerar al player "encima"
    [SerializeField] private float slideForce = 5f;     // fuerza con la que se empuja al player a un lado
    [SerializeField] private float slideDuration = 0.2f; // cuánto dura el empuje (mientras dura, el player no controla su movimiento horizontal)

    [Header("Sistema de Vida")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;
    private float lastAttackTime;
    private bool isDead = false;
    private bool isTouchingPlayer = false;
    private float patrolDirection = 1f;

    private enum State { Patrol, Chase, Attacking, Dead }
    private State currentState = State.Patrol;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("¡No se encontró el Player! Verifica el Tag.");

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float verticalDiff = Mathf.Abs(player.position.y - transform.position.y);

        if (isTouchingPlayer)
            Attack();
        // Solo persigue si está cerca Y el player está aproximadamente al mismo nivel vertical
        else if (distanceToPlayer <= detectionRange && verticalDiff <= chaseVerticalLimit)
            ChasePlayer();
        else
            Patrol();
    }

    void Patrol()
    {
        if (currentState == State.Attacking)
            anim.ResetTrigger("atacar");

        currentState = State.Patrol;

        // Si no hay suelo adelante o hay muro, dar la vuelta
        if (!HaySueloAdelante(patrolDirection) || HayMuroAdelante(patrolDirection))
            patrolDirection *= -1f;

        Mover(patrolDirection);
    }

    void ChasePlayer()
    {
        if (currentState == State.Attacking)
            anim.ResetTrigger("atacar");

        currentState = State.Chase;

        float xDiff = player.position.x - transform.position.x;

        // Zona muerta: si el player está casi alineado verticalmente, no oscilar entre direcciones
        if (Mathf.Abs(xDiff) < chaseDeadzoneX)
        {
            OrientarSprite(patrolDirection);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            anim.SetFloat("velocidad", 0f);
            return;
        }

        float direction = xDiff > 0 ? 1f : -1f;

        // Si avanzar lo haría caer al vacío o chocar con un muro, se queda quieto en el borde
        if (!HaySueloAdelante(direction) || HayMuroAdelante(direction))
        {
            OrientarSprite(direction);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            anim.SetFloat("velocidad", 0f);
            return;
        }

        patrolDirection = direction; // sincroniza dirección al volver al patrol
        Mover(direction);
    }

    void Attack()
    {
        currentState = State.Attacking;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // ← conserva gravedad
        anim.SetFloat("velocidad", 0f);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            anim.SetTrigger("atacar");

            // Knockback PRIMERO: si el siguiente daño mata al player, así su cadáver sale con impulso
            PlayerController playerCtrl = player.GetComponent<PlayerController>();
            if (playerCtrl != null)
            {
                float kbDir = player.position.x >= transform.position.x ? 1f : -1f;
                playerCtrl.ApplyKnockback(new Vector2(kbDir * knockbackX, knockbackY), slideDuration);
            }

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
        // Lanzamos dos rayos a alturas distintas para cubrir muros de cualquier altura
        return WallCheckAtHeight(direction, 0f) || WallCheckAtHeight(direction, wallCheckUpperHeight);
    }

    bool WallCheckAtHeight(float direction, float height)
    {
        Vector2 origin = (Vector2)transform.position + Vector2.up * height;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.right * direction, wallCheckDistance, groundMask);
        foreach (var h in hits)
        {
            if (h.collider == null) continue;
            if (h.collider.gameObject == gameObject) continue;          // su propio collider
            if (h.collider.CompareTag("Enemy")) continue;                // otros enemigos
            if (h.collider.GetComponent<enemyController>() != null) continue; // por si no tienen tag
            return true; // muro real
        }
        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
        // Si choca físicamente con otro enemigo, da la vuelta para no quedar trabado
        else if (collision.gameObject.CompareTag("Enemy") ||
                 collision.gameObject.GetComponent<enemyController>() != null)
        {
            patrolDirection *= -1f;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
            anim.ResetTrigger("atacar");
        }
    }

    // Se llama cada FixedUpdate mientras el player sigue tocando al enemigo.
    // Si el player está apoyado en la cabeza, lo "resbalamos" hacia un costado.
    void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        Transform p = collision.transform;
        float verticalDiff = p.position.y - transform.position.y;

        // ¿Está el player claramente por encima?
        if (verticalDiff < topThreshold) return;

        PlayerController playerCtrl = collision.gameObject.GetComponent<PlayerController>();
        if (playerCtrl == null) return;

        // Empujamos hacia el lado más cercano. Si está justo en el centro, lo mandamos detrás del enemigo.
        float horizontalDiff = p.position.x - transform.position.x;
        float kbDir;
        if (Mathf.Abs(horizontalDiff) < 0.05f)
            kbDir = -Mathf.Sign(transform.localScale.x); // detrás del enemigo
        else
            kbDir = Mathf.Sign(horizontalDiff);

        // Usamos ApplyKnockback para que el PlayerController NO sobreescriba la velocidad mientras dura
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
        GetComponent<Collider2D>().enabled = false;
        // 0.5s ≈ duración de la animación de muerte (0.42s) + pequeño margen
        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        float dir = patrolDirection != 0 ? patrolDirection : 1f;
        Vector3 origin = transform.position;

        Vector3 edgeOrigin = origin + Vector3.right * dir * edgeCheckDistance;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(edgeOrigin, edgeOrigin + Vector3.down * edgeCheckDepth);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(origin, origin + Vector3.right * dir * wallCheckDistance);
        Vector3 upperOrigin = origin + Vector3.up * wallCheckUpperHeight;
        Gizmos.DrawLine(upperOrigin, upperOrigin + Vector3.right * dir * wallCheckDistance);
    }
}
