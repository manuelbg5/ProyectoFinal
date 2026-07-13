using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Movimiento")]
    [SerializeField] private float movementSpeed = 2.5f;
    [SerializeField] private float stoppingDistance = 4f;
    [Header("Disparo")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 1.5f;
    [SerializeField] private float bulletSpeed = 8f;

    [Header("Invocación de robots")]
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private Transform spawnPointLeft;
    [SerializeField] private Transform spawnPointRight;
    [SerializeField] private float summonInterval = 20f;
    [SerializeField] private int maxRobotsAlive = 4;

    private Rigidbody2D rb;
    private bool isDead;
    private float nextFireTime;
    private float nextSummonTime;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
                
        }
        nextSummonTime = Time.time + summonInterval;
    }

    private void FixedUpdate()
    {
        if (isDead || player == null)
        {
            StopMoving();
            return;
        }
        if (Time.time >= nextSummonTime)
        {
            SummonRobots();
            nextSummonTime = Time.time + summonInterval;
        }

        float differenceX = player.position.x - transform.position.x;
        float horizontalDistance = Mathf.Abs(differenceX);

        FacePlayer();

        if (horizontalDistance <= stoppingDistance)
        {
            StopMoving();

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireInterval;
            }

            return;
        }

        float direction = Mathf.Sign(differenceX);

        rb.linearVelocity = new Vector2(
            direction * movementSpeed,
            rb.linearVelocity.y
        );

        if (animator != null)
            animator.SetFloat(SpeedHash, 1f);
    }


    private void MoveTowardsPlayer()
    {
        float direction =
            Mathf.Sign(player.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(
            direction * movementSpeed,
            rb.linearVelocity.y
        );

        if (animator != null)
            animator.SetFloat(SpeedHash, 1f);
    }

    private void StopMoving()
    {
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        if (animator != null)
            animator.SetFloat(SpeedHash, 0f);
    }

    private void FacePlayer()
    {
        if (spriteRenderer == null || player == null)
            return;

        bool playerIsLeft =
            player.position.x < transform.position.x;

        spriteRenderer.flipX = playerIsLeft;
    }

    public void PlayShootAnimation()
    {
        if (!isDead && animator != null)
            animator.SetTrigger(ShootHash);
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

    private void Shoot()
    {
        if (isDead || bulletPrefab == null || firePoint == null || player == null)
            return;

        if (animator != null)
            animator.SetTrigger(ShootHash);

        float direction = player.position.x < transform.position.x ? -1f : 1f;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb != null)
        {
            bulletRb.linearVelocity = new Vector2(
                direction * bulletSpeed,
                0f
            );
        }

        SpriteRenderer bulletSprite = bullet.GetComponent<SpriteRenderer>();

        if (bulletSprite != null)
            bulletSprite.flipX = direction < 0f;
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
            Instantiate(
                robotPrefab,
                spawnPointLeft.position,
                Quaternion.identity
            );
        }

        if (spawnPointRight != null &&
            robotsAlive.Length + 1 < maxRobotsAlive)
        {
            Instantiate(
                robotPrefab,
                spawnPointRight.position,
                Quaternion.identity
            );
        }
    }




}