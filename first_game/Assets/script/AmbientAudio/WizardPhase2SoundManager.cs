using UnityEngine;
using System.Collections.Generic;
public class WizardPhase2SoundManager : BehaviourSoundManager
{
    // play dead sound if dies

    // play golem sound every random seconds *

    // play vulnerable indicator sound *

    // play hurt sound *
    [SerializeField] private AudioClip vulnerable;

    [SerializeField] private AudioClip deadSound;

    [SerializeField] private List<AudioClip> golemSound;

    [SerializeField] private EvilWizardPhase2 evilWizard;

    private float timer;

    private float curTimer;

    void Start(){
        timer = 1f;
        curTimer = timer;
    }
    void Update(){
        PlayGolemSound();
    }
    private void PlayGolemSound(){
        if(!CanPlayGolemSound()){
            return;
        }
        if(curTimer > 0){
            curTimer -= Time.deltaTime;
            return;
        }

        base.PlaySound(base.DecidedClip(golemSound));
        timer = UnityEngine.Random.Range(4f, 10f);
        curTimer = timer;
    }
    private bool CanPlayGolemSound(){
        if(evilWizard.parameter.healthManager.isDead){
            return false;
        }
        return true;
    }

    public void PlayVulnerableSound(){
        base.PlaySound(vulnerable);
    }

    public void PlayDeathSound(){
        base.PlaySound(deadSound);
    }
}
