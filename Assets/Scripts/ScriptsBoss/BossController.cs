using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Movimiento (Patrullaje Automático de Izquierda a Derecha)")]
    [SerializeField] private float movementSpeed = 2.5f;
    [SerializeField] private float patrolDuration = 3.0f; // Cada cuántos segundos cambia de dirección

    [Header("Invocación de robots pequeños")]
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private Transform spawnPointLeft;
    [SerializeField] private Transform spawnPointRight;
    [SerializeField] private float summonInterval = 20f;
    [SerializeField] private int maxRobotsAlive = 4;

    private Rigidbody2D rb;
    private bool isDead;
    private float nextSummonTime;

    // Variables internas de control de patrullaje
    private float patrolTimer;
    private float directionX = -1f; // Comienza caminando a la izquierda

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        nextSummonTime = Time.time + summonInterval;
        patrolTimer = patrolDuration; // Inicializamos el temporizador de patrulla
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            StopMoving();
            return;
        }

        // Lógica de invocación constante
        if (Time.time >= nextSummonTime)
        {
            SummonRobots();
            nextSummonTime = Time.time + summonInterval;
        }

        // ====================================================================
        // NUEVO: Lógica de Patrullaje de izquierda a derecha por tiempo
        // ====================================================================
        patrolTimer -= Time.fixedDeltaTime;
        if (patrolTimer <= 0f)
        {
            directionX *= -1f;           // Invertimos la dirección (-1 a 1, o 1 a -1)
            patrolTimer = patrolDuration; // Reiniciamos el reloj
        }

        // Aplicamos velocidad horizontal constante
        rb.linearVelocity = new Vector2(directionX * movementSpeed, rb.linearVelocity.y);

        // Volteamos el sprite según la dirección en la que camina
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (directionX < 0f); // Si va a la izquierda (negativo), flipX es true
        }

        if (animator != null)
            animator.SetFloat(SpeedHash, 1f);
    }

    private void StopMoving()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (animator != null)
            animator.SetFloat(SpeedHash, 0f);
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;
        StopMoving();

        if (animator != null)
            animator.SetTrigger(DieHash);
    }

    private void SummonRobots()
    {
        if (isDead || robotPrefab == null)
            return;

        RobotEspadaController[] robotsAlive =
            FindObjectsByType<RobotEspadaController>(FindObjectsSortMode.None);

        if (robotsAlive.Length >= maxRobotsAlive)
            return;

        if (spawnPointLeft != null)
        {
            Instantiate(robotPrefab, spawnPointLeft.position, Quaternion.identity);
        }

        if (spawnPointRight != null && robotsAlive.Length + 1 < maxRobotsAlive)
        {
            Instantiate(robotPrefab, spawnPointRight.position, Quaternion.identity);
        }
    }
}