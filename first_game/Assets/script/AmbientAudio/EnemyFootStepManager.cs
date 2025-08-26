using UnityEngine;
using System.Collections.Generic;
public class EnemyFootStepManager : FootStepManager
{
    [Header("Footstep Clips")]
    [SerializeField] private List<AudioClip> grassSoundClips;

    [SerializeField] private List<AudioClip> stoneSoundClips;

    [SerializeField] private AudioSource audioSource;

    private float pitchVariation = 0.1f;

    public void PlayStepSound(){
        if(base.curSurface == "Grass"){
            RandomisePitch();
            PlaySound(DecidedClip(grassSoundClips));
            return;
        }
        if(base.curSurface == "Stone"){
            RandomisePitch();
            PlaySound(DecidedClip(stoneSoundClips));
        }
    }
    private AudioClip DecidedClip(List<AudioClip> clips){
        int randomIdx = UnityEngine.Random.Range(0, clips.Count);

        return clips[randomIdx];
    }

    private void RandomisePitch(){
        audioSource.pitch = UnityEngine.Random.Range(1f - pitchVariation, 1f + pitchVariation);
    }
    private void PlaySound(AudioClip clip){
        audioSource.PlayOneShot(clip);
    }
}
