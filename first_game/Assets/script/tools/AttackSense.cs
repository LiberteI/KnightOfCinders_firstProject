using UnityEngine;
using System.Collections;

// USAGE: attach this script to camera, and you should set pause time and shake time in playerMovement or wherever you collide with enemy.
// set enemy tag to Enemy;

public class AttackSense : MonoBehaviour
{
    private static AttackSense instance;
    private bool isShaking;
    public static AttackSense Instance{
        get{
            if(instance == null) instance = Transform.FindFirstObjectByType<AttackSense>();
            return instance;
        }
    }   

    public void HitPause(int duration){
        StartCoroutine(Pause(duration));
    }

    // Realise pause frames
    IEnumerator Pause(int duration){
        float pauseTime = duration / 12f;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1;
    }

    public void CameraShake(float duration, float strength){
        if(!isShaking) StartCoroutine(Shake(duration,strength));
    }

    IEnumerator Shake(float duration, float strength){
        float pauseTime = duration / 12f;
        isShaking = true;
        Transform camera = Camera.main.transform;
        Vector3 startPosition = camera.position;

        while(duration > 0){
            camera.position = Random.insideUnitSphere * strength + startPosition;
            duration -= Time.deltaTime;
            yield return null;
        }
        isShaking = false;
    }
}
