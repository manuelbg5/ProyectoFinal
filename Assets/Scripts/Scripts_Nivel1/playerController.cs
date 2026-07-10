using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float forceJump = 5f;
    [SerializeField] private float forceJumpMax = 10f; // ← salto máximo si mantiene
    [SerializeField] private float jumpHoldTime = 0.3f; // ← tiempo máximo que puede mantener

    [Header("Detección de Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Disparo")]
    [SerializeField] private GameObject bulletPrefab;   // ← arrastra el prefab de bala
    [SerializeField] private Transform firePoint;       // ← punto donde sale la bala
    [SerializeField] private float fireRate = 0.3f;     // ← tiempo entre disparos

    // Componentes
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private bool isGrounded;

    // Salto variable
    private bool isJumping = false;
    private float jumpTimeCounter = 0f;

    // Disparo
    private float lastFireTime = 0f;
    private bool facingRight = true;

    // Knockback (cuando un enemigo lo empuja)
    private float knockbackTimer = 0f;

    public void ApplyKnockback(Vector2 velocity, float duration)
    {
        // Si el controller está deshabilitado (player muerto), no aplicar — evita que el cadáver "salga volando"
        if (!enabled) return;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = velocity;
        knockbackTimer = duration;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Si está en knockback, dejamos que la física empuje al player sin sobreescribir su velocidad
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
            anim.SetBool("estaSaltando", !isGrounded);
            return;
        }

        // 1. Detección de suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);

        // 2. Input horizontal
        float moveX = Input.GetAxis("Horizontal");

        // 3. Giro del personaje
        if (moveX > 0)
        {
            facingRight = true;
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }
        else if (moveX < 0)
        {
            facingRight = false;
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }

        // 4. Animaciones
        anim.SetFloat("velocidad", Mathf.Abs(moveX));
        anim.SetBool("estaSaltando", !isGrounded);

        // 5. Salto variable
        float velocityY = rb.linearVelocity.y;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = 0f;
            velocityY = forceJump; // ← salto mínimo al presionar
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter < jumpHoldTime)
            {
                // Mientras mantiene, agrega fuerza extra gradualmente
                float extraForce = Mathf.Lerp(0, forceJumpMax - forceJump, jumpTimeCounter / jumpHoldTime);
                velocityY = forceJump + extraForce;
                jumpTimeCounter += Time.deltaTime;
            }
            else
            {
                isJumping = false; // llegó al máximo
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false; // soltó el botón
        }

        // 6. Disparo con tecla E o clic izquierdo
        if (Input.GetKeyDown(KeyCode.E))
        {
            Shoot();
        }

        // 7. Aplicar movimiento
        rb.linearVelocity = new Vector2(moveX * speed, velocityY);
    }

    void Shoot()
    {
        if (Time.time < lastFireTime + fireRate) return;
        if (bulletPrefab == null || firePoint == null) return;

        lastFireTime = Time.time;
        anim.SetTrigger("Disparar");

        // Instancia la bala
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Le da dirección según hacia dónde mira
        bullet bulletScript = bullet.GetComponent<bullet>();
        if (bulletScript != null)
            bulletScript.SetDirection(facingRight ? 1f : -1f);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}