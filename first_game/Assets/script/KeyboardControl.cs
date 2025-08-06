using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardControl : MonoBehaviour
{
    public GameObject panel;
    public bool isPaused = false;
    public PlayerMovement playerMovement;

    void Update(){
        //reset button
        reset();
        pullMenu();

    }

    public void pullMenu(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isPaused){
                gameResume();
            }
            else{
                gamePause();
                //freeze time and pull out the menu window
            }
            
           
        }
    }
    
    public void reset(){
        if(Input.GetKeyDown("r")){
            SceneManager.LoadScene("level1");
        }
    }

    public void gamePause(){
        Time.timeScale = 0;
        isPaused = true;
        panel.SetActive(true);
        
    }
    public void gameResume(){
        Time.timeScale = 1;
        isPaused = false;
        panel.SetActive(false);
    }
}