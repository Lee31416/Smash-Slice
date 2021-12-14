using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MainMenu : MonoBehaviour
    {
        public void StartMulti()
        {
            SceneManager.LoadScene("Lobby");
        }

        public void StartSingle()
        {
            SceneManager.LoadScene("SinglePlayer");
        }
        
        public void QuitGame() 
        {
            Application.Quit();    
        }

        public void BackToMainMenu()
        {
            SceneManager.LoadScene(0);
        } 
    }
}