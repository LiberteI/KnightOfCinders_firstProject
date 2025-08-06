using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float smoothTime = 0.5f;
    
    public Vector3 offset = new Vector3(0,5.5f, -10);

    private Vector3 velocity = Vector3.zero;


    void FixedUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        desiredPosition.z = offset.z;//z got overriden in 2d 
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        transform.position = smoothedPosition;
    }
}
