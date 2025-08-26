using UnityEngine;
using System.Collections.Generic;
public class PlayerSoundManager : BehaviourSoundManager
{
    [SerializeField] protected List<AudioClip> jumpSound;

    public void PlayJumpSound(){
        base.PlaySound(base.DecidedClip(jumpSound));
    }

}
