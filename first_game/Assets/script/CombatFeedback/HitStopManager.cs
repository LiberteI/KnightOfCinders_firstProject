using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HitStopManager : MonoBehaviour
{
    /*
        this class is a generic class for applying hit stop
    */
   
    public void ApplyHitStop(float stopTime){
        StartCoroutine(ExecuteHitStop(stopTime));
    }

    public void ApplyEnemyFinisher(){
        float stopTime = 0.5f;

        float slowMotionTime = 0.5f;

        StopAllCoroutines();
        StartCoroutine(ExecuteEnemyFinisher(stopTime, slowMotionTime));
        
    }
    private IEnumerator ExecuteHitStop(float stopTime){
        // freeze time once executed
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(stopTime);

        Time.timeScale = 1f;
    }

    private IEnumerator ExecuteEnemyFinisher(float stopTime, float slowMotionTime){
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(stopTime);

        Time.timeScale = 0.3f;

        yield return new WaitForSecondsRealtime(slowMotionTime);

        Time.timeScale = 1f;
    }
}
