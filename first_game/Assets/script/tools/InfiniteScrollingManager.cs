using UnityEngine;

public class InfiniteScrollingManager : MonoBehaviour
{
    [SerializeField]
    private GameObject camera;

    [SerializeField]
    private float length;
    
    [SerializeField]
    private ParallaxManager parallaxManager;

    void Start()
    {   
        // get background image width
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        // the movement of camera related to the background
        float backgroundMovement = camera.transform.position.x * (1 - parallaxManager.parallaxSpeed);
        
        // if the length of the movement of camera related to the background has added up to a width of the image:
        if(backgroundMovement - parallaxManager.lastPos > length){
            // swap the parent to the middle again
            parallaxManager.lastPos += length;
            transform.position = new Vector3(parallaxManager.lastPos, transform.position.y, transform.position.z);
        }
        else if(backgroundMovement + parallaxManager.lastPos < -length){
            parallaxManager.lastPos -= length;
            transform.position = new Vector3(parallaxManager.lastPos, transform.position.y, transform.position.z);
        }
        
    }
}