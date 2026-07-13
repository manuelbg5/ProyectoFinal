using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Puntos de movimiento")]
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private Transform topPoint;

    [Header("Configuración")]
    [SerializeField] private float speed = 2f;

    private Vector3 targetPosition;

    private void Start()
    {
        // La plataforma comienza moviéndose hacia arriba
        targetPosition = topPoint.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Cuando llega al punto cambia de dirección
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            targetPosition = targetPosition == topPoint.position
                ? bottomPoint.position
                : topPoint.position;
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }*/

    private void OnDrawGizmos()
    {
        if (bottomPoint != null && topPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(bottomPoint.position, topPoint.position);

            Gizmos.DrawWireSphere(bottomPoint.position, 0.1f);
            Gizmos.DrawWireSphere(topPoint.position, 0.1f);
        }
    }
}