using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    [SerializeField] private GameObject cam;

    [SerializeField] private float parallaxSpeed; 

    [SerializeField] private float graphStartPos;

    [SerializeField] private float camStartPos;

    [SerializeField] private float smoothness; // higher the snappier

    void Start(){
        camStartPos = cam.transform.position.x;

        graphStartPos = camStartPos;

        transform.position = new Vector3(graphStartPos, transform.position.y, transform.position.z);
    }

    void LateUpdate(){
        // update cam pos using mouse
        Vector3 mousePos = Input.mousePosition;
        
        float deltaCamPos = mousePos.x - camStartPos;

        // Debug.Log($"deltaCamPos: {deltaCamPos}");
        float graphDisplacement = deltaCamPos * parallaxSpeed * 0.01f;

        // Debug.Log($"graphDisplacement: {graphDisplacement}");
        float targetPox = graphStartPos + graphDisplacement;

        float newX = Mathf.Lerp(transform.position.x, targetPox, smoothness * Time.deltaTime);
        Vector3 newPos = new Vector3(newX, transform.position.y, transform.position.z);

        transform.position = newPos;
    }


}
