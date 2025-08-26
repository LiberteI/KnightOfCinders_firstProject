using UnityEngine;

public class GameObjectDisabler : MonoBehaviour
{
    void OnEnable(){
        EventManager.OnWinning += DisableAllGameObject;

        EventManager.OnDefeat += DisableAllGameObject;
    }
    void OnDisable(){
        EventManager.OnWinning -= DisableAllGameObject;

        EventManager.OnDefeat -= DisableAllGameObject;
    }

    private void DisableAllGameObject(){
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("ShouldDisable");

        for(int i = 0; i < enemies.Length; i ++){
            enemies[i].SetActive(false);
        }
    }
}
