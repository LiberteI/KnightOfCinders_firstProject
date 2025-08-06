using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuWindow : MonoBehaviour
{

    private KeyboardControl keyboardControler;
    public GameObject Panel;


    public void Start(){
        keyboardControler = FindFirstObjectByType<KeyboardControl>(); 
        // Find the PlayerMovement script in the scene
        if (keyboardControler == null)
        {
            // Debug.LogError("keyboardControler script not found!");
        }
        // pauseMenuUI = GameObject.Find("Panel");
    }
    
    
    public void onClickExit(){
        keyboardControler.gameResume();
        SceneManager.LoadScene("MainMenu");
    }



    public void onClickResume(){
        keyboardControler.gameResume();
    
    }
    
}
