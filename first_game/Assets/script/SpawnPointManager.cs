using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GameObject spawnPoint;
    public Transform spPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = GameObject.Find("SpawnPoint");
        spPoint = spawnPoint.transform;
    }

    // Update is called once per frame
    void Update()
    {   
        checkBound();
        
    }
    
    private void checkBound(){
        if(playerMovement.transform.position.y < -5f){
            Debug.Log("You fall to the void space!!!");
            playerMovement.transform.position = spPoint.position;
            // spPoint is a Transform object instead of a position
        }
    }
}
