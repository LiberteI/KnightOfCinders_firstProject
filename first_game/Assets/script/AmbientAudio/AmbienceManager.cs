using UnityEngine;
using System.Collections;

using System.Collections.Generic;
public class AmbienceManager : MonoBehaviour
{
    // this script will control the ambience sound based on where the player is at

    [Header("Forest // Castle PlayGround")]

    [SerializeField] private AudioClip birdSound;

    [SerializeField] private AudioClip crowSound;

    [SerializeField] private AudioClip wolfHowlSound;

    [SerializeField] private AudioClip windSound;

    [SerializeField] private AudioClip rainSound;

    // do a thunder here

    [SerializeField] private AudioClip thunderSound;

    [Header("Sewer And Dungeon")]
    [SerializeField] private AudioClip underWater;

    [SerializeField] private AudioClip frogSound;

    [SerializeField] private AudioClip Cave_WaterDroplets;

    [Header("Final Room")]

    [SerializeField] private AudioClip fireSound;

    // Loops → wind, water, constant insects (played on loop with fade-in/out).

    // Random One-Shots → birds, wolves, frogs (triggered every X–Y seconds with randomness).

    [SerializeField] private GamePlayCoordinator coordinator;

    [SerializeField] private AudioSource ambienceSource;

    [SerializeField] private AudioSource oneShotSource;

    private Coroutine oneShotCoroutine;

    private bool wolfHasTriggered = false;


    void OnEnable(){
        EventManager.OnSceneChanged += PlayOneShotLoop;

        EventManager.OnSceneChanged += PlayAmbience;

        EventManager.OnWinning += DisableAllSounds;

        EventManager.OnDefeat += DisableAllSounds;

        EventManager.OnWinning += DisableThisScript;

        EventManager.OnDefeat += DisableThisScript;
    }

    void OnDisable(){
        EventManager.OnSceneChanged -= PlayOneShotLoop;

        EventManager.OnSceneChanged -= PlayAmbience;

        EventManager.OnWinning -= DisableAllSounds;

        EventManager.OnDefeat -= DisableAllSounds;

        EventManager.OnWinning -= DisableThisScript;

        EventManager.OnDefeat -= DisableThisScript;
    }
    void Start(){
        ambienceSource.loop = true;
        oneShotCoroutine = StartCoroutine(LoopOneShots("outside"));
    }   
    private void DisableThisScript(){
        this.enabled = false;
    }

    private void DisableAllSounds(){
        ambienceSource.Stop();
        oneShotSource.Stop();
    }
    private void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("howlTrigger")){
            TriggerWolfHowling();
        }
    }

    private void TriggerWolfHowling(){
        if(wolfHasTriggered){
            return;
        }
        PlaySound(wolfHowlSound);
        wolfHasTriggered = true;
    }
    
    private bool shouldPlayAmbience(){
        if(coordinator.isInBossFight){
            return false;
        }
        return true;
    }
    
    public void PlayOneShotLoop(string curScene){
        if(oneShotCoroutine != null){
            StopCoroutine(oneShotCoroutine);
            oneShotCoroutine = StartCoroutine(LoopOneShots(curScene));
        }
    }

    public void PlayAmbience(string curScene){
        if(curScene == "sewer"){
            PlayUnderWaterLoop();
        }
        else if(curScene == "dungeon"){
            PlayDungeonLoop();
        }
        else if(curScene == "outside"){
            PlayOutsideLoop();
        }
        else if(curScene == "finalRoom"){
            PlayRoomLoop();
        }
        else if(curScene == "rainScene"){
            PlayRainLoop();
        }
    }


    public IEnumerator LoopOneShots(string currentScene){
        while(true){
            // is out
            if(currentScene == "outside"){
                // every 5 - 10 seconds
                float time = UnityEngine.Random.Range(5f, 10f);

                int randomHalf = UnityEngine.Random.Range(0, 2);

                yield return new WaitForSeconds(time);
                if(randomHalf == 0){
                    if(shouldPlayAmbience()){
                        PlaySound(birdSound);
                    }
                    
                }
                else{
                    if(shouldPlayAmbience()){
                        PlaySound(crowSound);
                    }
                    
                }
                

            }
            else if(currentScene == "sewer" || currentScene == "dungeon"){
                // every 4-8 seconds

                float time = UnityEngine.Random.Range(4f, 8f);

                yield return new WaitForSeconds(time);
                if(shouldPlayAmbience()){
                    PlaySound(frogSound);
                }
                
            }
            
            else{
                yield return null;
            }
            
        }
        
    }
    public void PlayRainLoop(){
        PlayLoop(rainSound, 1f);
        oneShotSource.loop = true;
        oneShotSource.clip = thunderSound;
        oneShotSource.volume = 0.6f;
        oneShotSource.Play();
    }
    public void PlayUnderWaterLoop(){
        PlayLoop(underWater, 1f);
    }
    public void PlayRoomLoop(){
        oneShotSource.loop = false;
        oneShotSource.Stop();
        PlayLoop(fireSound, 1f);
    }
    public void PlayDungeonLoop(){
        PlayLoop(Cave_WaterDroplets, 1f);
    }

    public void PlayOutsideLoop(){
        PlayLoop(windSound, 1f);
    }

    private void PlaySound(AudioClip clip){
        oneShotSource.PlayOneShot(clip);
    }


    private void PlayLoop(AudioClip newClip, float fadeTime){
        StartCoroutine(ExecuteNewLoopWithFade(newClip, fadeTime));
    }

    private IEnumerator ExecuteNewLoopWithFade(AudioClip newClip, float fadeTime){
        // fade out
        for(float t = 0; t < fadeTime; t += Time.deltaTime){
            ambienceSource.volume = 1 - t / fadeTime;

            yield return null;
        }

        // play new clip
        ambienceSource.Stop();

        ambienceSource.clip = newClip;

        ambienceSource.Play();

        // fade in 

        for(float t = 0; t < fadeTime; t += Time.deltaTime){
            ambienceSource.volume = t / fadeTime;

            yield return null;
        }

    }
    /*
        Background Layer (constant loops)

            Wind through trees (gentle whoosh, with random gusts)

            Distant birds (crows, owls, woodpeckers, depending on day/night)

            Insects (crickets, cicadas, flies, depending on region & time)

            Rustling leaves (soft background rustle)

        Foreground / Randomized One-Shots

            Bird calls nearby (sparrows, crows, owls at night)

            Twigs snapping (small animals moving)

            Bush rustle (squirrel or deer moving away)

            Distant wolf howl (rare, for tension)

            Falling branch or leaves hitting the ground
    */
}
