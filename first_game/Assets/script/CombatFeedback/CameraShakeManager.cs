using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;

public class CameraShakeManager : MonoBehaviour
{
    /*      
        this class is for applying camera shake effects. The script is attached to cameras during boss fight.
    */
    [SerializeField] private CinemachineCamera curCamera;

    [SerializeField] private CinemachineBasicMultiChannelPerlin noise;


    void OnEnable(){
        EventManager.OnEnterBossFight += UpdateCurCamera;
    }

    void OnDisable(){
        EventManager.OnEnterBossFight -= UpdateCurCamera;
    }
   

    public void ApplyCameraShake(float amplitude, float frequency, float duration){
        StartCoroutine(ExecuteCameraShake(amplitude, frequency, duration));
    }

    private IEnumerator ExecuteCameraShake(float amplitude, float frequency, float duration){
        noise.AmplitudeGain = amplitude;

        noise.FrequencyGain = frequency;

        yield return new WaitForSecondsRealtime(duration);

        noise.AmplitudeGain = 0;

        noise.FrequencyGain = 0;

    }
    private void UpdateCurCamera(ArenaSetUp setup){
        curCamera = setup.currentCamera;

        noise = curCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ApplyLaserShake(){
        StartCoroutine(ExecuteCameraShake(0.5f, 1f, 0.25f));
    }

}
