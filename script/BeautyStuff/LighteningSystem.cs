using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Rendering.Universal;
public class LighteningSystem : MonoBehaviour
{
    // this script is for manipulating lightening system during skeleton squat fight scene

    [SerializeField] private List<GameObject> lighteningPoints;

    private float timer;

    private float curTimer;

    private float maxIntensity = 10f;

    void Update(){
        UpdateCurTimer();

        PlayLightening();
    }
    private void UpdateCurTimer(){
        if(curTimer > 0){
            curTimer -= Time.deltaTime;
        }
    }

    private void PlayLightening(){
        if(curTimer > 0){
            return;
        }

        // excute lightening.
        StartCoroutine(LightningStrike(GetRandomLighteningPoint()));

        // update curTimer
        timer = UnityEngine.Random.Range(3f, 6f);
        curTimer = timer;
    }
    private GameObject GetRandomLighteningPoint(){
        int randomIdx = UnityEngine.Random.Range(0, lighteningPoints.Count);

        return lighteningPoints[randomIdx];
    }

    private IEnumerator LitPointWithFade(GameObject point){
        // sudden lightening
        Light2D light2D = point.GetComponent<Light2D>();

        light2D.intensity = maxIntensity;


        // lightening fade-out
        float elapsed = 0;

        float duration = UnityEngine.Random.Range(0.2f, 1f);

        while(elapsed < duration){
            elapsed += Time.deltaTime;
            
            float factor = elapsed / duration;

            light2D.intensity = Mathf.Lerp(maxIntensity, 0, factor);

            yield return null;
        }
        light2D.intensity = 0;
    }

    // a point could flash more than once within a short time
    private IEnumerator LightningStrike(GameObject point)
    {
        int flashes = UnityEngine.Random.Range(1, 3);

        for (int i = 0; i < flashes; i++){
        
            yield return LitPointWithFade(point);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.2f));
        }
    }
}
