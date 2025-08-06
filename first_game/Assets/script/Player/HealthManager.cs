using UnityEngine;
using System.Collections;
public class HealthManager : MonoBehaviour
{
    /*
        duty:
        1. take damage
        2. heal
        3. store health
        4. trigger events
        5. handle death logic
    */ 
    [SerializeField] private float curKnightHealth;

    [SerializeField] private float maxKnightHealth;

    [SerializeField] private Knight knight;

    // in idle 10, idle_2 12, else 6
    public float healthRegenerateRate;

    public float healthRenerateBonus;

    [SerializeField] private float healCoolDownTimer;

    [SerializeField] private float damageDelayTimer;

    [SerializeField] private float curHealCoolDownTimer;

    [SerializeField] private float curDamageDelayTimer;
    
    public bool isDead;

    void Start()
    {   
        // assign initally health and boardcast
        curKnightHealth = maxKnightHealth;
        EventManager.KnightHealthChanged(this.gameObject, curKnightHealth, maxKnightHealth);
    }

    void Update(){
        CheckIsDead();
    }
    

    void FixedUpdate(){
        Heal(healthRegenerateRate + healthRenerateBonus);
        UpdateTimers();
    }
    public void TakeDamage(float damage){
        if(isDead){
            return;
        }
        // update health if valid 
        // die if < 0
        // boardcast
        // set isHit
        if(curKnightHealth <= 0){
            return;
        }
       
        curKnightHealth -= damage;
        curKnightHealth = Mathf.Clamp(curKnightHealth, 0, maxKnightHealth);
        
        // do broadcast here
        EventManager.KnightHealthChanged(this.gameObject, curKnightHealth, maxKnightHealth);
    }

    public void CheckIsDead(){
        // set flag
        if(!isDead){
            if(curKnightHealth <= 0){
                isDead = true;
                
                // do broadcast
                EventManager.RaiseKnightDied();
            }
        }
        
    }

    public void Heal(float rate){
        /*
            heal by frame. 
            delay between each healing
            boardcast
            if just got hit, get a little delay on the next healing
        */
        if(CanHeal()){
            curKnightHealth += rate;
            curKnightHealth = Mathf.Clamp(curKnightHealth, 0, maxKnightHealth);

            // do boardcast here
            EventManager.KnightHealthChanged(this.gameObject, curKnightHealth, maxKnightHealth);

            // reset cool down
            curHealCoolDownTimer = healCoolDownTimer;
        }
        // do boardcast here.
    }
    private void UpdateTimers(){
        if(knight.parameter.combatManager.getHit){
            curDamageDelayTimer = damageDelayTimer;
        }
        if(curDamageDelayTimer > 0f){
            curDamageDelayTimer -= Time.deltaTime;
        }
        if(curHealCoolDownTimer > 0f){
            curHealCoolDownTimer -= Time.deltaTime;
        }
    }
    private bool CanHeal(){
        // if in damage delay timer, cannot heal.
        // if in heal dalay cool down timer, cannot heal.
        // if has died cannt heal
        if(curDamageDelayTimer > 0){
            return false;
        }
        if(curHealCoolDownTimer > 0){
            return false;
        }
        if(curKnightHealth <= 0){
            return false;
        }
        return true;
    }
}
