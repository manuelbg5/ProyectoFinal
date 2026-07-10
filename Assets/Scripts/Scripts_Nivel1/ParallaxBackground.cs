using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Vector2 parallaxEffectMultiplier; // Ej. (0.1, 0.1) para muy lento

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate() // Se ejecuta después de que el jugador se mueva
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Mueve el fondo una fracción del movimiento de la cámara
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);

        lastCameraPosition = cameraTransform.position;
    }
}