using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionWindowObject;
    
    public void onClickPlay(){
        SceneManager.LoadScene("level1");
    }
    public void onClickQuit(){
        Application.Quit();
    }
    public void onClickOption(){
        optionWindowObject.SetActive(true);
    }
    public void onClickExit(){
        optionWindowObject.SetActive(false);
    }
}
