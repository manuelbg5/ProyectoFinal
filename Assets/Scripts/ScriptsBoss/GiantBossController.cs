using UnityEngine;

public class GiantBossController : MonoBehaviour
{
    [Header("Invocación de Robots Medianos")]
    [SerializeField] private GameObject mediumRobotPrefab; 
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float summonInterval = 20f;
    [SerializeField] private int maxMediumRobotsAlive = 2; 

    private float nextSummonTime;
    
    // NUEVO: Referencia al Animator
    private Animator anim; 

    private void Start()
    {
        nextSummonTime = Time.time + summonInterval;
        
        // NUEVO: Buscamos el Animator en el hijo (igual que en GiantBossHealth)
        anim = GetComponentInChildren<Animator>(); 
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

        BossController[] todosLosRobots = FindObjectsByType<BossController>(FindObjectsSortMode.None);
        int robotsVivos = 0;

        foreach (BossController robot in todosLosRobots)
        {
            BossHealth saludRobot = robot.GetComponent<BossHealth>();
            if (saludRobot != null && !saludRobot.IsDead)
            {
                robotsVivos++;
            }
        }

        if (robotsVivos < maxMediumRobotsAlive)
        {
            Instantiate(mediumRobotPrefab, spawnPoint.position, Quaternion.identity);
            
            // NUEVO: ¡Disparamos la animación de invocar!
            if (anim != null)
            {
                anim.SetTrigger("Summon");
            }
            
            Debug.Log("Jefe Gigante invocó. Robots vivos actualmente: " + robotsVivos);
        }
    }

    public void CambiarIntervaloDeInvocacion(float nuevoIntervalo, int nuevoMaximo)
    {
        summonInterval = nuevoIntervalo;
        nextSummonTime = Time.time; 
        Debug.Log("¡Fase 2 Activa! Nuevo intervalo: " + summonInterval);
    }
}
