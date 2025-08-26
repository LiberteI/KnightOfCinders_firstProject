using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CutSceneAmbienceManager : MonoBehaviour
{   
    [Header("opening")]
    public AudioClip screamingAudio;

    public AudioClip breathingAudio;

    public AudioClip swordSound;

    public AudioClip equiptSound;

    public AudioClip defeatSound;

    [Header("winning")]

    public AudioClip knightBreath;

    public AudioClip knightAnger;

    public AudioClip witchBreath;

    public AudioClip magicSound;

    public AudioClip peaceful;

    public AudioClip fireSound;

    public AudioClip glassSound;

    public AudioClip swordSound2;

    public AudioClip sliceSound;

    public AudioSource ambienceSource;

    [SerializeField] private AudioSource bgMusicSource;

    [SerializeField] private AudioClip bgm;
     
    public void PlayLoop(AudioClip newClip, float fadeTime){
        StartCoroutine(ExecuteNewLoopWithFade(newClip, fadeTime));
    }

    public void Start(){
        bgMusicSource.loop = true;

        bgMusicSource.clip = bgm;
        StartCoroutine(FadeInAndPlay());
        
    }

    private IEnumerator FadeInAndPlay(){
        // fade in 
        bgMusicSource.Play();
        for(float t = 0; t < 1f; t += Time.deltaTime){
            bgMusicSource.volume = t / 1f;

            yield return null;
        }

    }
    private IEnumerator ExecuteNewLoopWithFade(AudioClip newClip, float fadeTime){
        // fade out
        for(float t = 0; t < fadeTime; t += Time.deltaTime){
            ambienceSource.volume = 1 - t / fadeTime;

            yield return null;
        }

        // play new clip
        ambienceSource.Stop();

        ambienceSource.clip = newClip;

        ambienceSource.Play();

        // fade in 

        for(float t = 0; t < fadeTime; t += Time.deltaTime){
            ambienceSource.volume = t / fadeTime;

            yield return null;
        }

    }
}
