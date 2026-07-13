using UnityEngine;

public class GiantBossController : MonoBehaviour
{
    [Header("Invocación de Robots Medianos")]
    [SerializeField] private GameObject mediumRobotPrefab; // Arrastra tu prefab del BossController aquí
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float summonInterval = 20f;
    [SerializeField] private int maxMediumRobotsAlive = 2; // Para no llenar la pantalla

    private float nextSummonTime;

    private void Start()
    {
        nextSummonTime = Time.time + summonInterval;
    }

    private void Update()
    {
        if (Time.time >= nextSummonTime)
        {
            SummonMediumRobot();
            nextSummonTime = Time.time + summonInterval;
        }
    }

    private void SummonMediumRobot()
    {
        if (mediumRobotPrefab == null) return;

        // Obtenemos todos los robots medianos en la escena
        BossController[] todosLosRobots = FindObjectsByType<BossController>(FindObjectsSortMode.None);
        int robotsVivos = 0;

        // Filtramos y contamos SOLO a los que NO están muertos
        foreach (BossController robot in todosLosRobots)
        {
            BossHealth saludRobot = robot.GetComponent<BossHealth>();
            if (saludRobot != null && !saludRobot.IsDead)
            {
                robotsVivos++;
            }
        }

        // Si los vivos son menos que el límite, invocamos
        if (robotsVivos < maxMediumRobotsAlive)
        {
            Instantiate(mediumRobotPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log("Jefe Gigante invocó. Robots vivos actualmente: " + robotsVivos);
        }
    }

    public void CambiarIntervaloDeInvocacion(float nuevoIntervalo, int nuevoMaximo)
    {
        summonInterval = nuevoIntervalo;
        maxRobotsAlive = nuevoMaximo;
        
        // MAGIA AQUÍ: Reiniciamos el contador de tiempo para que invoque 
        // INMEDIATAMENTE al entrar en la Fase 2, sin tener que esperar.
        nextSummonTime = Time.time; 
        
        Debug.Log("¡Fase 2 Activa! Nuevo intervalo: " + summonInterval);
    }
}
