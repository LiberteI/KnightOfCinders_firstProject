using UnityEngine;
using System.Collections.Generic;
public class BehaviourSoundManager : MonoBehaviour
{
    /*
        - Define: attack, hurt, jump sounds.
    */

    [SerializeField] protected List<AudioClip> attackGruntSounds;

    [SerializeField] protected List<AudioClip> hurtGruntSounds;

    [SerializeField] protected AudioSource audioSource;

    public virtual void PlayAttackSound(){
        PlaySound(DecidedClip(attackGruntSounds));
    }

    public virtual void PlayHurtSound(){
        PlaySound(DecidedClip(hurtGruntSounds));
    }

    public AudioClip DecidedClip(List<AudioClip> clips){
        int randomIdx = UnityEngine.Random.Range(0, clips.Count);

        return clips[randomIdx];
    }

    public virtual void PlaySound(AudioClip clip){
        audioSource.PlayOneShot(clip);
    }

    void OnEnable(){
        EventManager.OnWinning += DisableAllSounds;

        EventManager.OnDefeat += DisableAllSounds;

        EventManager.OnWinning += DisableThisScript;

        EventManager.OnDefeat += DisableThisScript;
    }

    void OnDisable(){
        EventManager.OnWinning -= DisableAllSounds;

        EventManager.OnDefeat -= DisableAllSounds;

        EventManager.OnWinning -= DisableThisScript;

        EventManager.OnDefeat -= DisableThisScript;
    }

    private void DisableThisScript(){
        this.enabled = false;
    }

    private void DisableAllSounds(){
        audioSource.Stop();
    }
}
