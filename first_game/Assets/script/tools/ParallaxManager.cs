using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ParallaxManager : MonoBehaviour
{   
    [SerializeField]
    public float lastPos;

    [SerializeField]
    public float parallaxSpeed;

    [SerializeField]
    private GameObject camera;
    

    void Start(){
        lastPos = transform.position.x;
    }
    
    void Update(){
        // decide how much the bgImage would move related to the camera
        // speed = 1: stay put related to camera. Looks infinitely far to player
        // speed = 0: move fast related to camera. Looks infinitely near from player
        // speed = 0.5 move at half speed. Looks mid-far.
        float relativeDistance = camera.transform.position.x * parallaxSpeed;

        transform.position = new Vector3(lastPos + relativeDistance, transform.position.y, transform.position.z);
    }

}


