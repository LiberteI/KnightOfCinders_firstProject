using UnityEngine;
using System.Collections.Generic;
public class WizardSoundManager : BehaviourSoundManager
{
    // wizard grunts when attack, grunts when being attack. *
    // angry when doing heavy attack *
    // is painful when vulnerable *
    // scream when die and have to let out the evil spirit *
    // play evil magic sound when cast a spell.
    [SerializeField] private List<AudioClip> angrySound;

    [SerializeField] private List<AudioClip> painfulSound;

    [SerializeField] private AudioClip screamSound;

    [SerializeField] private AudioClip evilSound;

    [SerializeField] private AudioClip setOutPhase2Sound;

    public void PlayPainfulSound(){
        base.PlaySound(base.DecidedClip(painfulSound));
    }

    public void PlayAngrySound(){
        base.PlaySound(base.DecidedClip(angrySound));
    }

    public void PlayScreamSound(){
        base.PlaySound(screamSound);
    }

    public void PlayEvilSpellSound(){
        base.PlaySound(evilSound);
    }

    public void PlaySetOutPhase2Sound(){
        base.PlaySound(setOutPhase2Sound);
    }
}
