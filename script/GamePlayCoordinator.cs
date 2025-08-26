using UnityEngine;
using System;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
[Serializable]
public class ArenaSetUp{
    public GameObject currentLeftBarrier;

    public GameObject currentRightBarrier;

    public CinemachineCamera currentCamera;
    
    public GameObject currentBoss;
}
public class GamePlayCoordinator : MonoBehaviour
{
    /*
        - The purpose of this script is to coordinate each part of the game.
            For example, when player walks into the first hidden enemy in the dungeon, the camera is fixed in the fight arena.
            Two slices of transparent map barriers are also created to prevent player from going through.
            Only when the enemy is eliminated can the player go to the next part of the map, when the barrier is eliminated as well.

        - The boss fights will be triggered when player walks into specific area. So this script can be a child game object of the player.

        - The game is seperated into 5 parts. The player is spawned in natural but desolate environment, where player is taught how to perform basic movement.
            Then the player will have to search the front gate of the castle, but cannot go through. So the player has to go through the sewer.
            The sewer connects to the dungeon, where the player encounters the first enemy. After the enemy is invoked, the camera is fixed, and barriers are set.
            This process is treated as a training level. Where the player is taught how to dodge, light attack, heavy attack, defend, etc...

        - Then the player goes up and will encounter the first boss: dark wolf. Still the same process with accessing next level

        - As the player is approaching the key kulprit, a skeleton squat game object will be activated, and it will instantly summon skeletons consists of 7.

        - Beating those skeletons, the player could face the final boss. The evil wizard.
    */

    [Header("Dungeon - Trainer Skeleton")]
    
    public ArenaSetUp dungeonSetUp;


    [Header("Castle Playground - Dark Wolf")]
    
    public ArenaSetUp darkWolfSetUp;
    

    [Header("Entrance to Main Hall - Skeleton Squat")]
    
    public ArenaSetUp SkeletonSquatSetUp;


    [Header("Main Hall - Evil Wizard")]
    
    public ArenaSetUp evilWizardSetUp;

    
    [Header("constant cameras")]
    [SerializeField] private CinemachineCamera exploringCamera1;

    [SerializeField] private CinemachineCamera exploringCamera2;

    [SerializeField] private CinemachineCamera sewerCamera;

    [SerializeField] private CinemachineCamera dungeonCamera;

    [SerializeField] private CinemachineCamera finalBossCamera;

    public ArenaSetUp curArena;

    // attributes that make sure to trigger scenes once.
    private bool dungeonTriggered;

    private bool darkWolfTriggered;

    private bool squatTriggered;

    private bool wizardTriggered;

    public bool isInBossFight;

    public string currentScene = "outside";

    [SerializeField] private Transform sceneCheckCentre;

    [SerializeField] private float sceneCheckRadius;

    [SerializeField] private LayerMask outside;

    [SerializeField] private LayerMask finalRoom;

    [SerializeField] private LayerMask sewer;

    [SerializeField] private LayerMask dungeon;

    [SerializeField] private LayerMask rainScene;

    [SerializeField] private GameObject rain;

    [SerializeField] private GameObject bound;

    [SerializeField] private AmbienceManager ambienceManager;

    

    void OnEnable(){
        EventManager.OnEnterBossFight += IsInBossFight;

        EventManager.OnExitBossFight += CleanUpArena;

        EventManager.OnExitBossFight += HasExitedBossFight;
    }
    void OnDisable(){
        EventManager.OnEnterBossFight -= IsInBossFight;

        EventManager.OnExitBossFight -= CleanUpArena;

        EventManager.OnExitBossFight -= HasExitedBossFight;
    }
    void Start(){
        exploringCamera1.Priority = 10;
    }
    void Update(){
        DetermineScene();

        CheckQuit();
    }

    private void CheckQuit(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            SceneManager.LoadScene("MainMenu");
        }
    }
    private void OnTriggerStay2D(Collider2D other){
        if(other.CompareTag("SewerRange")){
            sewerCamera.Priority = 20;
            exploringCamera1.Priority = 0;
            exploringCamera2.Priority = 10;
            return;
        }

        if(other.CompareTag("DungeonRange")){
            dungeonCamera.Priority = 20;
            return;
        }

        if(other.CompareTag("FinalRoom")){
            finalBossCamera.Priority = 20;
            return;
        }
        
    }
    // trigger entry -- fight setup -- boss death -- arena clean up
    private void OnTriggerEnter2D(Collider2D other){
        
        if(other.CompareTag("DungeonTrigger")){
            TriggerArena(ref dungeonTriggered, dungeonSetUp);
        }

        if(other.CompareTag("WolfTrigger")){
            TriggerArena(ref darkWolfTriggered, darkWolfSetUp);
        }

        if(other.CompareTag("SSTrigger")){
            TriggerArena(ref squatTriggered, SkeletonSquatSetUp);
        }

        if(other.CompareTag("FinalBossTrigger")){
            TriggerArena(ref wizardTriggered, evilWizardSetUp);
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        
        if(other.CompareTag("SewerRange")){
            sewerCamera.Priority = 0;
            return;
        }
        if(other.CompareTag("DungeonRange")){
            dungeonCamera.Priority = 0;
            return;
        }
        if(other.CompareTag("FinalRoom")){
            finalBossCamera.Priority = 0;
            return;
        }
    }
    
    private void TriggerArena(ref bool hasTriggered, ArenaSetUp currentArena){
        // when entering the arena, set flag, activate boss and switch camera.

        // defensive coding
        if(currentArena == null){
            return;
        }
        if(hasTriggered){
            return;
        }
        // set flag to true
        hasTriggered = true;

        // switch camera
        currentArena.currentCamera.Priority = 15;

        // set boundaries
        currentArena.currentLeftBarrier.SetActive(true);
        currentArena.currentRightBarrier.SetActive(true);


        // activate boss
        currentArena.currentBoss.SetActive(true);

        // broadcast
        EventManager.RaiseEnterBossFight(currentArena);

        // update curArena 
        curArena = currentArena;
    }

    public void CleanUpArena(ArenaSetUp currentArena){
        // when the boss fight is done, disable barrier, switch back to exploring camera.

        currentArena.currentLeftBarrier.SetActive(false);
        currentArena.currentRightBarrier.SetActive(false);

        currentArena.currentCamera.Priority = 0;
    }


    public void IsInBossFight(ArenaSetUp currentArena){
        isInBossFight = true;
    }

    public void HasExitedBossFight(ArenaSetUp currentArena){
        isInBossFight = false;
    }

    public void DetermineScene(){
        string curScene = "";
        // Debug.Log(curScene);
        if(Physics2D.OverlapCircle(sceneCheckCentre.position, sceneCheckRadius, dungeon)){
            curScene = "dungeon";
        }
        else if(Physics2D.OverlapCircle(sceneCheckCentre.position, sceneCheckRadius, finalRoom)){
            curScene = "finalRoom";
        }
        else if(Physics2D.OverlapCircle(sceneCheckCentre.position, sceneCheckRadius, sewer)){
            curScene = "sewer";
        }
        
        else if(Physics2D.OverlapCircle(sceneCheckCentre.position, sceneCheckRadius, outside)){
            curScene = "outside";
        }
        else if(Physics2D.OverlapCircle(sceneCheckCentre.position, sceneCheckRadius, rainScene)){
            curScene = "rainScene";
            rain.SetActive(true);
            bound.SetActive(true);
        }
        // Debug.Log(curScene);
        if(curScene != "" && curScene != currentScene){
            EventManager.RaiseSceneChanged(curScene);
            currentScene = curScene;
        }
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(sceneCheckCentre.position, sceneCheckRadius);
    }
}
