using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public static class StaminaCostTable{
    /*
        light attack costs a little 3-5% per use
        heavy attack costs more than a little 5 - 8% per use
        run costs a little 3-5% / use
        blocked attack costs huge 30% / occurance
        roll costs huge 10% / use
        shield strike costs 5% / use
    */

    public static readonly Dictionary<StaminaCostTypes, float> costs = new() {
        {StaminaCostTypes.LightAttack, 5f},
        {StaminaCostTypes.HeavyAttack, 8f},
        {StaminaCostTypes.Run, 0.5f}, 
        {StaminaCostTypes.BlockedAttack, 30f},
        {StaminaCostTypes.Roll, 10f},
        {StaminaCostTypes.ShieldStrike, 5f},
        {StaminaCostTypes.RunAttack, 8f},
        {StaminaCostTypes.JumpAttack, 6f},
    };
}
// define what behaviour would cost stamina.
public enum StaminaCostTypes{
    LightAttack, HeavyAttack, Run, BlockedAttack, Roll, ShieldStrike, RunAttack, JumpAttack
}
public class StaminaManager : MonoBehaviour
{
    
    [SerializeField] private float maxStamina; // 100

    public float curStamina;

    [SerializeField] private float depletionTimer;

    [SerializeField] private float currentDepletionTimer;

    public float reproduceRate; // 6 by default

    public float idleBonus; // + 3 , +4

    public float walkBonus; // + 2

    private bool isDepleted;
    

    [SerializeField] private Knight knight;
    void Start()
    {
        curStamina = maxStamina;
    }

    
    void FixedUpdate(){
        // keep regenerating
        RegenerateStamina();
    }

    /*
        protocols:
        1. there is a global stamina bar. !!
        2. stamina regenerates every frame.
        3. running, heavy attacks, blocking while being attack, shieldStrick will reduce stamina.
        4. if stamina drops below 0, reset to 0 but it takes 1 extra second to restart generating, because of depletion
        5. clamp stamina so that it will never go above max or below 0. 
        6. if stamina is not enough for next move: handle gracefully.
        7. if stamina is full, the next attack will double damage.
    */
    private void RegenerateStamina(){
        if(isDepleted){
            currentDepletionTimer -= Time.deltaTime;
            if(currentDepletionTimer > 0){
                return; // do not regenerate yet.
            }
            else{
                // reset depletion data
                isDepleted = false;
            }
        }
        // reproduce stamina in a fixed percentage of the max amount by time.
        
        if(knight.currentState is KIdleState){
            curStamina += (reproduceRate + idleBonus) * Time.deltaTime;
        }
        else if(knight.currentState is KWalkState){
            curStamina += (reproduceRate + walkBonus) * Time.deltaTime;
        }
        else{
            curStamina += reproduceRate * Time.deltaTime;
        }
        // clamp stamina
        ClampStamina();
        // boardcast
        EventManager.KnightStaminaChanged(curStamina, maxStamina);

        
    }
    private void ClampStamina(){
        curStamina = Mathf.Clamp(curStamina, 0, maxStamina);
    }

    public bool DeductStamina(StaminaCostTypes type, bool force){
        // search dictionary.
        // if key is found then return true and store value in out
        if(StaminaCostTable.costs.TryGetValue(type, out float cost)){
            // if cur stamina is sufficient or force enter
            if(curStamina >= cost || force){
                curStamina = Mathf.Max(0, curStamina - cost);
                // clamp stamina
                ClampStamina();

                // boardcast
                EventManager.KnightStaminaChanged(curStamina, maxStamina);

                // depletion boardcast and start regeneration delay
                if(curStamina <= 1f && !isDepleted){
                    isDepleted = true;
                    currentDepletionTimer = depletionTimer;

                    EventManager.KnightStaminaDepleted();
                }
                return true;
            }
        }
        return false;
    }


}
