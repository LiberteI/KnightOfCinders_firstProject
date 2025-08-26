using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private GameObject cam;

    // 0 = move with camera
    // 1 = do not move at all
    [SerializeField] private float parallaxSpeed; 

    [SerializeField] private float graphStartPos;

    [SerializeField] private float camStartPos;

    [SerializeField] private float length;

    [SerializeField] private float cameraRelativeDisplacement;

    void Start(){
        camStartPos = cam.transform.position.x;

        graphStartPos = camStartPos;

        transform.position = new Vector3(graphStartPos, transform.position.y, transform.position.z);
        // get background image width
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    void FixedUpdate(){
        float deltaCamPos = cam.transform.position.x - camStartPos;

        // Debug.Log($"deltaCamPos: {deltaCamPos}");
        float graphDisplacement = deltaCamPos * parallaxSpeed;

        // Debug.Log($"graphDisplacement: {graphDisplacement}");

        Vector3 newPos = new Vector3(graphStartPos + graphDisplacement, transform.position.y, transform.position.z);

        transform.position = newPos;

        // relative difference (how far camera moved compared to background)
        cameraRelativeDisplacement = deltaCamPos - graphDisplacement;
        // Debug.Log($"cameraRelativeDisplacement: {cameraRelativeDisplacement}");
        // infinite scrolling
        if(cameraRelativeDisplacement > length){
            // swap the parent to the middle again
            graphStartPos += length;

            camStartPos += length;

            transform.position = newPos;
        }
        else if(cameraRelativeDisplacement < -length){
            // swap the parent to the middle again
            graphStartPos -= length;

            camStartPos -= length;
            
            transform.position = newPos;
        }
    }
}
