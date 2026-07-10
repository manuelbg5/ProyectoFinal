using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string levelScene = "Level1";

    void Update()
    {
        // Presiona Enter o Space para iniciar
        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(levelScene);
    }
}
