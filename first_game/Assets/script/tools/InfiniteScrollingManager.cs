using UnityEngine;

public class InfiniteScrollingManager : MonoBehaviour
{
    [SerializeField] private float length;
    
    [SerializeField]
    private ParallaxManager parallaxManager;

    void Start()
    {   
        
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // the movement of camera related to the background
        float backgroundMovement = parallaxManager.cameraDeltaPos;
        
        // if the length of the movement of camera related to the background has added up to a width of the image:
        if(backgroundMovement - parallaxManager.graphDeltaPos > length){
            
            parallaxManager.graphDeltaPos += length;
            transform.position = new Vector3(parallaxManager.graphDeltaPos, transform.position.y, transform.position.z);
        }
        else if(backgroundMovement + parallaxManager.graphDeltaPos < -length){
            parallaxManager.graphDeltaPos -= length;
            transform.position = new Vector3(parallaxManager.graphDeltaPos, transform.position.y, transform.position.z);
        }
        
    }
}