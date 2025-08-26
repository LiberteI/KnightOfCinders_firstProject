using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnClickPlay(){
        SceneManager.LoadScene("BeginningCutScene");
    }
    public void OnClickQuit(){
        Application.Quit();
    }
}
