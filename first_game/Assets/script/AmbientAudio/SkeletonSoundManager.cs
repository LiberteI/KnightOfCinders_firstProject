using UnityEngine;
using System.Collections.Generic;
public class SkeletonSoundManager : BehaviourSoundManager
{
    // skeleton growl once a while,(aborted) grunts when attack and being attacked.
    [SerializeField] List<AudioClip> growlSound;
    
    private void PlayGrowlSound(){
        base.PlaySound(base.DecidedClip(growlSound));
    }
}
