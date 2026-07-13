using UnityEngine;
using UnityEngine.SceneManagement; // Requerido para cambiar o reiniciar escenas

public class ControladorMenuGameOver : MonoBehaviour
{
    public void ReiniciarNivel()
    {
        // Si pausaste el tiempo con Time.timeScale = 0f al morir, despaúsalo aquí:
        Time.timeScale = 1f; 

        // Recarga la escena en la que estás actualmente jugando
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        // Cambia "MenuPrincipal" por el nombre exacto de tu escena de menú
        SceneManager.LoadScene("MainMenu"); 
    }
}
