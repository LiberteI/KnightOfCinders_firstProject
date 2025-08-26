using UnityEngine;
using System.Collections.Generic;
public class PlayerFootStepManager : FootStepManager
{
    [SerializeField] private MovementManager movementManager;

    
    // implement run, walk, roll sounds
    [SerializeField] private AudioSource audioSource;

    [Header("Footstep Clips")]
    [SerializeField] private List<AudioClip> grassSoundClips;

    [SerializeField] private List<AudioClip> stoneSoundClips;

    [SerializeField] private List<AudioClip> waterSoundClips;

    [SerializeField] private AudioClip rollSound;

    private float pitchVariation = 0.1f;

    private bool CanPlaySound(){
        if(movementManager.IsGrounded()){
            return true;
        }
        return false;
    }


    public void PlayStepSound(){
        if(base.curSurface == "Grass"){
            if(CanPlaySound()){
                RandomisePitch();
                PlaySound(DecidedClip(grassSoundClips));
                return;
            }
        }
        if(base.curSurface == "Stone"){
            if(CanPlaySound()){
                RandomisePitch();
                PlaySound(DecidedClip(stoneSoundClips));
            }
        }
        if(base.curSurface == "Water"){
            RandomisePitch();
            PlaySound(DecidedClip(waterSoundClips));
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

    public void PlayRollSound(){
        if(base.curSurface == "Grass"){
            RandomisePitch();
            PlaySound(rollSound);
        }
        if(base.curSurface == "Stone"){
            RandomisePitch();
            PlaySound(DecidedClip(stoneSoundClips));
        }
        
    }
}
