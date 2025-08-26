using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ParallaxManager : MonoBehaviour
{   
    public float graphStartPos;

    public float graphDeltaPos;

    public float parallaxSpeed;

    [SerializeField]
    private GameObject camera;
    
    public float cameraDeltaPos;

    private float camStartPos;

    void Start(){
        graphStartPos = transform.position.x;

        camStartPos = camera.transform.position.x;
    }
    
    void FixedUpdate(){
        // decide how much the bgImage would move related to the camera
        // speed = 1: stay put related to camera. Looks infinitely far to player
        // speed = 0: move fast related to camera. Looks infinitely near from player
        // speed = 0.5 move at half speed. Looks mid-far.
        cameraDeltaPos = camera.transform.position.x - camStartPos;

        graphDeltaPos = transform.position.x - graphStartPos;
        
        float relativeDistance = cameraDeltaPos * parallaxSpeed;

        Vector3 newPos = new Vector3(graphStartPos + relativeDistance, transform.position.y, transform.position.z);

        transform.position = newPos;
    }

}


