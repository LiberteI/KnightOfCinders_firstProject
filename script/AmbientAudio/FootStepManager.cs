using UnityEngine;

public class FootStepManager : MonoBehaviour
{   
    protected string curSurface;

    public void OnTriggerStay2D(Collider2D other){
        if(other.CompareTag("Grass")){
            // if grounded set current surface to grass
            curSurface = "Grass";
        }
        
        if(other.CompareTag("Stone")){
            // if grounded set current surface to stone
            curSurface = "Stone";
        }

        if(other.CompareTag("SewerRange")){
            // if inside sewer, set current surface to water
            curSurface = "Water";
        }
    }
}
