using UnityEngine;

using System.Collections;

using System.Collections.Generic;
public class BackGroundMusicManager : MonoBehaviour
{   
    [SerializeField] private AudioClip nonFightBGM;

    [SerializeField] private List<AudioClip> fightBGM;

    [SerializeField] private AudioSource bgmSource;

    void Start(){
        bgmSource.loop = true;
        
        bgmSource.clip = nonFightBGM;

        bgmSource.Play();
    }

    void OnEnable(){
        EventManager.OnEnterBossFight += PlayBossFightMusic;

        EventManager.OnExitBossFight += PlayNonFightMusic;

        EventManager.OnWinning += DisableAllSounds;

        EventManager.OnDefeat += DisableAllSounds;
    }

    void OnDisable(){
        EventManager.OnEnterBossFight -= PlayBossFightMusic;

        EventManager.OnExitBossFight -= PlayNonFightMusic;

        EventManager.OnWinning -= DisableAllSounds;

        EventManager.OnDefeat -= DisableAllSounds;
    }

    private void PlayBossFightMusic(ArenaSetUp curArena){
        if(curArena.currentBoss.name == "SkeletonForTraining"){
            StartCoroutine(ExecuteNewMusicWithFade(fightBGM[0], 1f));
        }
        else if(curArena.currentBoss.name == "DarkWolfContainer (1)"){
            StartCoroutine(ExecuteNewMusicWithFade(fightBGM[1], 1f));
        }
        else if(curArena.currentBoss.name == "SkeletonSquat"){
            StartCoroutine(ExecuteNewMusicWithFade(fightBGM[2], 1f));
        }
        else if(curArena.currentBoss.name == "EvilWizardContainer"){
            StartCoroutine(ExecuteNewMusicWithFade(fightBGM[3], 1f));
        }
        else{
            Debug.Log("no boss found");
        }
        
    }
    private void PlayNonFightMusic(ArenaSetUp curArena){
        StartCoroutine(ExecuteNewMusicWithFade(nonFightBGM, 1f));
    }
    // play non fightBGM when not in a boss fight
    private IEnumerator ExecuteNewMusicWithFade(AudioClip newClip, float fadeTime){
        // fade out
        for(float t = 0; t < fadeTime; t += Time.deltaTime){
            bgmSource.volume = 1 - t / fadeTime;

            yield return null;
        }

        bgmSource.Stop();

        bgmSource.clip = newClip;

        bgmSource.Play();

        // fade in
        for(float t = 0; t < fadeTime; t += Time.deltaTime){
            bgmSource.volume = t / fadeTime;

            yield return null;
        }
    }

    private void DisableAllSounds(){
        bgmSource.Stop();
    }
}
