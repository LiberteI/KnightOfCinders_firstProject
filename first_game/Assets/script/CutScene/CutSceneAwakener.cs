using UnityEngine;

public class CutSceneAwakener : MonoBehaviour
{
    [SerializeField] private GameObject winningCutScene;

    [SerializeField] private GameObject defeatCutScene;
    void OnEnable(){
        EventManager.OnWinning += EnableWinningCutScene;

        EventManager.OnDefeat += EnableDefeatCutScene;
    }
    // for testing
    // void Update(){
    //     if(Input.GetKeyDown("g")){
    //         EventManager.RaiseWinning();
    //     }
    //     if(Input.GetKeyDown("f")){
    //         EventManager.RaiseDefeat();
    //     }
    // }
    void OnDisable(){
        EventManager.OnWinning -= EnableWinningCutScene;

        EventManager.OnDefeat -= EnableDefeatCutScene;
    }
    private void EnableWinningCutScene(){
        winningCutScene.SetActive(true);
    }

    private void EnableDefeatCutScene(){
        defeatCutScene.SetActive(true);
    }
}
