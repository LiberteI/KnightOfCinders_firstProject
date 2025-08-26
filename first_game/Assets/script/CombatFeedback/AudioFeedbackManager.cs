using UnityEngine;

public class AudioFeedbackManager : MonoBehaviour
{   
    /*
        - Player:
            1. When an object is not hit, play swing sound.

            2. When skeleton is hit, play blade sound

            3. When other obj gets hit, except for skeleton, play slice sound.

            4. Randomise sound pich.

        - Skeleton
            1. When player is not hit, play swing sound.

            2. When player gets hit, play blade sound.

            3. when player is defending and get hit, play blade sound.

    */
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip swingSound;

    [SerializeField] private AudioClip sliceSound;

    
    [SerializeField] private AudioClip bladeSound;

    [SerializeField] private AudioClip wolfChargeSound;

    [SerializeField] private AudioClip wolfChargeHitSound;

    [SerializeField] private AudioClip wolfHowlingSound;


    [Header("Wizard")]

    [SerializeField] private AudioClip wizardP1CastSound;
    
    [SerializeField] private AudioClip wizardP2CastSound;

    [SerializeField] private AudioClip RodSound;

    

    [SerializeField] private AudioClip LaserMissSound;

    [SerializeField] private AudioClip MagicHitSound;

    public bool wolfIsCharging;

    public bool shouldPlayBladeSound;

    

    [SerializeField] private float pitchVariation = 0.1f;

    private void OnEnable()
    {
        EventManager.OnHitOccured += HandleHitFeedback;
    }

    private void OnDisable()
    {
        EventManager.OnHitOccured -= HandleHitFeedback;
    }

    private void PlaySound(AudioClip sound){
        if(sound == null){
            return;
        }
        if(audioSource == null){
            return;
        }
        RandomisePitch();
        audioSource.PlayOneShot(sound);
    }

    // if attack lands
    private void HandleHitFeedback(HitData data){

        
        if(data.initiator.CompareTag("Player")){
            // if it is skeleton that gets hit
            if(data.targetHurtBox.name == "HurtBox"){
                // play deflecting sound
                // Debug.Log("play blade sound");
                audioSource.Stop();
                PlaySound(bladeSound);
                
            }
            else{
                audioSource.Stop();
                PlaySound(sliceSound);
                
            }
            
        }
        if(data.initiator.CompareTag("Enemy")){
            // skeleton hit player
            if(data.initiator.name == "HurtBox"){
                audioSource.Stop();
                PlaySound(bladeSound);
                
            }
            // wolf hit player
            if(data.initiator.name == "DarkWolfDamageBox"){
                audioSource.Stop();
                // Debug.Log(wolfIsCharging);
                if(wolfIsCharging){
                    
                    PlaySound(wolfChargeHitSound);
                    
                }
                else{
                    PlaySound(bladeSound);
                    
                }
            }

            // wizard phase 1
            if(data.initiator.name == "DamageHitBox"){

                PlaySound(MagicHitSound);
                
            }

            // wizard phase 2
            if(data.initiator.name == "EvilWizardDamageHitBox"){
                // Debug.Log("Reached");
                // Debug.Log(shouldPlayBladeSound);
                if(shouldPlayBladeSound){
                    // Debug.Log("played blade sound");
                    audioSource.Stop();
                    PlaySound(bladeSound);
                    
                }
                else{
                    PlaySound(MagicHitSound);
                    
                }
                
            }
        }
    }


    public void PlayPlayerSwing(){
        // play swing clip here.
        // Debug.Log("Player swing sound");

       
        PlaySound(swingSound);
    }

    public void PlayWolfChargeSound(){
        PlaySound(wolfChargeSound);
    }

    private void RandomisePitch(){
        audioSource.pitch = UnityEngine.Random.Range(1f - pitchVariation, 1f + pitchVariation);
    }

    public void SetWolfIsCharging(bool value){
        wolfIsCharging = value;
    }

    public void PlayWolfHowlingSound(){
        PlaySound(wolfHowlingSound);
    }

    public void PlayWizardCastSound(){
        PlaySound(wizardP1CastSound);
    }

    public void PlayLaserSound(){
        PlaySound(LaserMissSound);
    }

    public void PlayRodSound(){
        PlaySound(RodSound);
    }

    public void PlayWizardP2CastSound(){
        PlaySound(wizardP2CastSound);
    }

    
}
