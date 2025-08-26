using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
public class UIManager : MonoBehaviour
{   
    /*
        The health bar is used by players and all the enemies.
        Behaviour: The default of the health bar is set to full. if the target takes damage, 
        the health bar would first shrink the red part slowly(not at once), and then stay where it is at,
        leaving a white bar. Then the white bar drops aligning with where the red part is.

        the heath bar would stay on the top of regular enemies, including prefabs that the bosses summon, as well as the player.
        But for the bosses, theirs are long ones sticked on the top of the screen.
        The behaviour follows the same as what has described above.
    */
    [Header("HEALTH BAR")]
    [SerializeField] private Slider redBar;

    [SerializeField] private Slider whiteBar;
    
    private float prevHealth;

    [SerializeField] private float whiteBarDeductSpeed;

    [SerializeField] private float whiteBarIncreaseSpeed;

    private Coroutine redCoroutine;

    private Coroutine whiteCoroutine;

    [SerializeField] private GameObject trackedEntity;

    [SerializeField] private GameObject container;

    [Header("STAMINA BAR")]
    [SerializeField] private Slider staminaBar;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prevHealth = redBar.value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnEnable(){
        // subscribe methods here
        EventManager.OnKnightHealthChanged += UpdateHealthBar;

        EventManager.OnKnightStaminaChanged += UpdateStaminaBar;

        EventManager.OnEnemyHealthChanged += UpdateHealthBar;

        EventManager.OnEnemyDied += DestroyHealthBarAfterDied;
    }
    void OnDisable(){
        // unsubscribe methods here
        EventManager.OnKnightHealthChanged -= UpdateHealthBar;

        EventManager.OnKnightStaminaChanged -= UpdateStaminaBar;

        EventManager.OnEnemyHealthChanged -= UpdateHealthBar;

        EventManager.OnEnemyDied -= DestroyHealthBarAfterDied;
    }

    private void DestroyHealthBarAfterDied(GameObject entity){
        if(entity != trackedEntity){
            return;
        }

        container.transform.Find("HealthBar").gameObject.SetActive(false);
    }
    public void UpdateHealthBar(GameObject entity, float cur, float max){
        if(entity != trackedEntity){
            return;
        } 
        // damage: update red bar first and then wait for a delay, update white bar.
        //heal: updated white bar first and then wait for a delay, update red bar
        float target = cur / max;
        
        // heal
        if(target > prevHealth){
            // stop prev coroutines
            if(redCoroutine != null){
                StopCoroutine(redCoroutine);
            }

            // instant update
            whiteBar.value = target;
            
            // call coroutine
            redCoroutine = StartCoroutine(increaseRedBar(target));
        }
        // damage
        else{
            // stop prev coroutines
            if(whiteCoroutine != null){
                StopCoroutine(whiteCoroutine);
            }

            // instant update
            redBar.value = target;
            
            // call coroutine
            whiteCoroutine = StartCoroutine(decreaseWhiteBar(target));
        }
        prevHealth = target;
    }
    public void UpdateStaminaBar(float cur, float max){
        if(staminaBar == null){
            return;
        }
        // update stamina bar instantly
        staminaBar.value = cur / max;
    }

    private IEnumerator increaseRedBar(float target){
        while(redBar.value < target){
            redBar.value += Time.deltaTime * whiteBarIncreaseSpeed;
            yield return null;
        }

        // avoid value being not target at last
        redBar.value = target;
    }

    private IEnumerator decreaseWhiteBar(float target){
        while(whiteBar.value > target){
            whiteBar.value -= Time.deltaTime * whiteBarDeductSpeed;
            yield return null;
        }
        // avoid value being not target at last
        whiteBar.value = target;
    }
}
