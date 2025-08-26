using UnityEngine;

public class WolfSoundManager : BehaviourSoundManager
{
    // wolf barks once a while(Aborted), growls when attack, pre dash and grunts when get hit.
    // play vulnerable indicator sound when in vulnerable state
    [SerializeField] private AudioClip preDashSound;

    [SerializeField] private AudioClip vulnerable;

    public void PlayPreDashSound(){
        base.PlaySound(preDashSound);
    }
    public void PlayVulnerableSound(){
        base.PlaySound(vulnerable);
    }
}
