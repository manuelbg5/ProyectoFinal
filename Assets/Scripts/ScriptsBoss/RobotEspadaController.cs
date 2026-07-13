using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RobotEspadaController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Movimiento")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackDistance = 1.2f;
    [SerializeField] private float attackCooldown = 1.5f;

    private Rigidbody2D rb;
    private float nextAttackTime;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
                player = p.transform;
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        float dx = player.position.x - transform.position.x;
        float distance = Mathf.Abs(dx);

        // Mirar al jugador
        spriteRenderer.flipX = dx < 0;

        if (distance > attackDistance)
        {
            float dir = Mathf.Sign(dx);

            rb.linearVelocity = new Vector2(
                dir * speed,
                rb.linearVelocity.y
            );

            animator.SetFloat(SpeedHash, 1);
        }
        else
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            animator.SetFloat(SpeedHash, 0);

            if (Time.time >= nextAttackTime)
            {
                animator.SetTrigger(AttackHash);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    [Header("Daño al Jugador")]
    [SerializeField] private int damageAmount = 2; // 2 equivale a un corazón completo según tu HUDVida

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log("¡Robot pequeño dañó al jugador!");
            }
        }
    }
}