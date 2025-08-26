using UnityEngine;
using System.Collections;

public class CombatManager : MonoBehaviour
{   

    /*
        Combat Manager:
        1. Implement 7 attacks: light1, 2, 3. heavy1, 2. RunAttack, JumpAttack
        2. Implement shield attack and defend
        3. implement hurt and die
        4. implement combo: light 1-2-3.
    */

        
        // player combat protocols
    /*. 
        1. Player attacks always interrupt enemy animations *

        2. Combos: *
            - A **combo counter** tracks consecutive light attacks within a time window.
            - Successful chaining allows **combo finishers**, making the player feel agile and rewarding fast input.

        3. Shield Strike: *
            - A low-damage attack that **pushes enemies back**.
            - Has **super armor** and **stuns the enemy on hit**.
            - Designed for **spacing**, not damage.

        4. Player Vulnerability: *
            - Player attacks **can be interrupted**.
            - Players must **rely on defense (blocking)** or **mobility (rolling)** to avoid punishment.

        5. Blocking: *
            - Grants **frontal invincibility**, but leaves the **back vulnerable**.
            - No damage is taken, but **player is knocked back** slightly on block.

        6. Rolling: *
            - Grants **full invincibility**.
            - Ideal for **dodging attacks** or **repositioning**.

        7. Jump & Run Attacks: *
            - Slightly stronger than light attacks.
            - Agile and effective for **engaging enemies** or performing **surprise strikes**.
        

        8. the second combo of heavy hit will have hit stop and camera shake

        9. the third combo of light hit will have hit stop and camera shake
    */
    //  combo system:
        /*
            
                - if player perform an attack during combo timer,
                    jump to the next attack
                - 

            combo timer should be massively extended so player loop through attacks
            
            ---------------- light attack flow -------------
            press j once: swipe up attack. 
                

            press j the 2nd time: swipe down attack
                

            press j the 3nd time: stab
            

            ------------------ heavy attack flow -------------
            press u once: top-down slash

            press u twice: horizontal slash


        */
    
    private float lightAttackTimer = 3f;
    [SerializeField] private float curLightAttackComboTimer;

    private float heavyAttackTimer = 3f;
    [SerializeField] private float curHeavyAttackComboTimer;

    public Knight knight;

    public bool isLightAttacking;

    public bool isHeavyAttacking;

    public bool isDefending;

    public bool isShieldStriking;

    public bool isJumpAttacking;

    public bool isRunAttacking;

    public bool getHit;

    public float damage;

    public float damageMultiplier = 1f;

    public float knockBackForce;

    [Header("Combat Feedback")]

    public FeedbackData sourceFeedback;

    [Header("AttackStepFlag")]
    public string curLightAttack;

    public string curHeavyAttack;
    
    void Update(){
        SetDamage();

        ResetAttackFlags();
    }
    
    void OnEnable(){
        EventManager.OnHitOccured += GetHit;
        EventManager.OnKnightDied += Die;
    }

    void OnDisable(){
        EventManager.OnHitOccured -= GetHit;
        EventManager.OnKnightDied -= Die;
    }

    public void ResetAllFlags(){
        isLightAttacking = false;

        isHeavyAttacking = false;

        isDefending = false;

        isShieldStriking = false;

        isJumpAttacking = false;

        isRunAttacking = false;
        
        getHit = false;
   
    }
    
    public void MultiplyCurDamage(){
        damage *= damageMultiplier;
    }

    public bool IsAttacking(){
        return isLightAttacking || isHeavyAttacking || isShieldStriking || isJumpAttacking || isRunAttacking;
    }

    public void Die(){
        // transition to death
        knight.TransitionState(KnightStateTypes.Die);
        knight.parameter.knightHurtBox.tag = "Untagged";
    }

    private void SetDamage(){
        if(isLightAttacking){
            damage = 20f;
        }
        else if(isHeavyAttacking){
            damage = 40f;
        }
        else if(isShieldStriking){
            damage = 1f;
        }
        else if(isJumpAttacking){
            damage = 50f;
        }
        else if(isRunAttacking){
            damage = 25f;
        }
        else{
            damage = 0;
        }

    }

    private void ResetAttackFlags(){
        // reset combo step to null if combo timer get deducted to below 0 
        curLightAttackComboTimer -= Time.deltaTime;
        curHeavyAttackComboTimer -= Time.deltaTime;

        if(curLightAttackComboTimer <= 0){
            curLightAttack = null;
        }

        if(curHeavyAttackComboTimer <= 0){
            curHeavyAttack = null;
        }
    }

    
    // reuse this coroutine if combo needed.
    public IEnumerator LightAttack(string curLightAttack){
        
        /*
            1. set flag
            2. play animation and make sure it finishes.
            3. Drain stamina
            4. disable prev speed, assign a new one
        */

        // if it current combo is the last one, apply hit feel
        if(curLightAttack == "Attack3"){
            sourceFeedback.stopTime = 0.1f;

            sourceFeedback.duration = 0.1f;

            sourceFeedback.amplitude = 0.1f;

            sourceFeedback.frequency = 0.1f;
        }
        else{
            sourceFeedback.stopTime = 0.05f;

            sourceFeedback.duration = 0.1f;

            sourceFeedback.amplitude = 0.1f;

            sourceFeedback.frequency = 0.1f;
        }
        isLightAttacking = true;

        // set flag to record combo step
        this.curLightAttack = curLightAttack;

        // set timer
        curLightAttackComboTimer = lightAttackTimer;

        knight.parameter.animator.Play(curLightAttack);

        yield return AnimationFinishes(curLightAttack, knight.parameter.movementManager.lightAttackSpeed);

        isLightAttacking = false;
    }

    public IEnumerator PowerAttack(string curHeavyAttack){
        /*
            1. set flag
            2. play animation and make sure it finishes.
            3. Drain stamina
            4. disable prev speed, assign a new one
        */
        // if the combo is the second step, apply hit feedback
        if(curHeavyAttack == "PowerAttack1"){
            sourceFeedback.stopTime = 0.2f;

            sourceFeedback.duration = 0.1f;

            sourceFeedback.amplitude = 0.15f;

            sourceFeedback.frequency = 0.15f;
        }
        else{
            sourceFeedback.stopTime = 0.1f;

            sourceFeedback.duration = 0.1f;

            sourceFeedback.amplitude = 0.1f;

            sourceFeedback.frequency = 0.1f;
        }
        isHeavyAttacking = true;
        
        // set flag to record heavy attack step
        this.curHeavyAttack = curHeavyAttack;

        // set timer
        curHeavyAttackComboTimer = heavyAttackTimer;

        knight.parameter.animator.Play(curHeavyAttack);
        
        yield return AnimationFinishes(curHeavyAttack, knight.parameter.movementManager.heavyAttackSpeed);

        isHeavyAttacking = false;
    }

    public IEnumerator ShieldStrike(){
        /*
            1. Perform ShieldStrike
            2. play animation and make sure it finishes.
            3. apply forces to hit items, to knock them back.(little damage)
            4. drain stamina
            
        */
        sourceFeedback.stopTime = 0.1f;

        sourceFeedback.duration = 0.1f;

        sourceFeedback.amplitude = 0.1f;

        sourceFeedback.frequency = 0.1f;

        isShieldStriking = true;

        knight.parameter.animator.Play("ShieldStrike");

        
        yield return AnimationFinishes("ShieldStrike", knight.parameter.movementManager.shieldStrikeSpeed);

        isShieldStriking = false;
    }

    public IEnumerator JumpAttack(){
        /*
            1. Perform JumpAttack when jumping
            2. play animation and make sure it finishes.
            3. drain stamina
        */
        isJumpAttacking = true;

        knight.parameter.animator.Play("JumpAttack");

        yield return AnimationFinishes("JumpAttack", Mathf.Abs(knight.parameter.RB.linearVelocity.x));

        // do stamina here.
        
        isJumpAttacking = false;
    }

    public IEnumerator RunAttack(){
        /*
            1. Perform JumpAttack when jumping
            2. play animation and make sure it finishes.
            3. drain stamina
            4. disable prev speed, assign a new one and slowly stop
        */
        isRunAttacking = true;

        knight.parameter.animator.Play("RunAttack");

        yield return AnimationFinishes("RunAttack", Mathf.Abs(knight.parameter.RB.linearVelocity.x));

        
        isRunAttacking = false;
    }
    // make sure animation finishes and reset speed during animation
    private IEnumerator AnimationFinishes(string animationName, float newSpeed){
        // wait to get into animation
        AnimatorStateInfo info = knight.parameter.animator.GetCurrentAnimatorStateInfo(0);
        while (!info.IsName(animationName)){
            yield return null;
            info = knight.parameter.animator.GetCurrentAnimatorStateInfo(0);
        }
        while(info.IsName(animationName)){
            knight.parameter.RB.linearVelocity = new Vector2(newSpeed * transform.localScale.x, knight.parameter.RB.linearVelocity.y);
            if(info.normalizedTime > 0.99f){
                break;
            }
            info = knight.parameter.animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
    }

/*
    Combat Response Rules:

    1. Light Attack: \
        - When hit, attack is interrupted.
        - Take full damage.
        - Transition to Hurt state, play Hurt animation.

    2. Power (Heavy) Attack: \
        - If stamina > 50%, super armor active: attack continues but take full damage.
        - Otherwise, attack is interrupted.
        - Transition to Hurt state, play Hurt animation.

    3. Defending: \
        - If hit from front:
        - If stamina >= cost, take 0 damage, knockback only.
        - If stamina < cost, take half damage, transition to Hurt state.
        - If hit from behind: take full damage, transition to Hurt state.

    4. Rolling: \
        - Invincible (no damage, no reaction).

    5. Consecutive Hits:
        - If hit 4 times within 5 seconds, enter temporary invulnerable state.

    6. Idle / Run / Walk / Jump / Jump Attack: \
        - When hit, attack is interrupted.
        - Take full damage.
        - Transition to Hurt state, play Hurt animation.

    7. Super Armor: \
        - Active during Shield Strike.
        - Also active during Run Attack if stamina > 70%.
*/

            // Phase 1. normally, transition to hurt state and play animation and take damage
            

            // phase 2. Player is defending: block attack initiated in the front only if stamina is enough.
            //          Defence will be broken if get hit from the back, taking full damage.
            //          Defence will be broken if get hit in the front with stamina being not sufficient to take one damage.
            
            // phase 3. Player is invincible: ie: rolling. return GetHit method.

    // DamageIsFromBack
    private bool DamageIsFromBack(HitData data, float playerFacingDir){
        // check if knock back direction is equal to where player is facing
        float attackIncomingDir = data.initiator.transform.position.x - transform.position.x;
        // Debug.Log($"attack Incoming dir : {attackIncomingDir}, player facing dir: {playerFacingDir}");
        int dataDir = 0;
        int playerDir = 0;
        if(attackIncomingDir > 0){
            dataDir = 1;
        }
        else if(attackIncomingDir < 0){
            dataDir = -1;
        }

        if(playerFacingDir > 0){
            playerDir = 1;
        }
        else if(playerFacingDir < 0){
            playerDir = -1;
        }

        if(dataDir == playerDir){
            // damage is not from back if the two dirs are the same
            return false;
        }
        else{
            // otherwise is 
            return true;
        }
    }
    
    private void GetHit(HitData data){
        if(knight.parameter.healthManager.isDead){
            return;
        }
        if(data == null){
            return;
        }


        // phase 3
        if(IsInvincible()){
            // Debug.Log("dodged an attack");
            return;
        }
        // Debug.Log("1");
        // Debug.Log($"data hurtbox: {data.targetHurtBox}");
        // Debug.Log($"knight hurtbox: {knight.parameter.knightHurtBox}");
        if(knight.parameter.knightHurtBox == data.targetHurtBox){
            
            // phase 2
            if(isDefending){
                // if damage is not from back
                if(!DamageIsFromBack(data, transform.localScale.x)){
                    // successfully defend. ie: stamina is sufficient
                    if(knight.parameter.staminaManager.DeductStamina(StaminaCostTypes.BlockedAttack, false)){
                        // transition to knock back state where velocity is not overridden

                        // get knock back a little.
                        // Debug.Log(data.knockBackDir);
                        GetKnockedBack(data.knockBackDir, 5f);
                        // take 0 damage
                        
                        return;
                    }
                    // fail to defend
                    else{
                        // empty stamina
                        
                        knight.parameter.staminaManager.DeductStamina(StaminaCostTypes.BlockedAttack, true);

                        // break defence and take half damage.
                        data.damage = 0.5f * data.damage;

                        knight.TransitionState(KnightStateTypes.Hurt);
                        knight.currentState.OnGetHit(data);
                        
                        return;
                    }

                }

                // if damage is from back
                // cannot defend: do phase 1
            }


            // phase 1
            knight.TransitionState(KnightStateTypes.Hurt);

            
            // pass incoming hit data
            // execute OnGetHit method in states that concern
            knight.currentState.OnGetHit(data);
        }
        
    }
    public IEnumerator ExecuteGetHit(HitData data){

        if(data.knockBackDir > 0){
            data.knockBackDir = 1;
        }
        else{
            data.knockBackDir = -1;
        }
        GameObject curInitiator = data.initiator;
        // set flag
        getHit = true;

        // get damage early
        knight.parameter.healthManager.TakeDamage(data.damage);

        // play animation
        knight.parameter.animator.Play("hurt");

        // if get crashed by a charging wolf:
        var dwCombat = data.initiator.GetComponentInParent<DWCombatManager>();
        if(dwCombat != null && dwCombat.isCharging){
            
            // Debug.Log(dwCombat.knockBackDir);
            int curFacing = 0;

            // cache player facing direction
            if(transform.localScale.x > 0){
                curFacing = 1;
            }
            else{
                curFacing = -1;
            }

            // use committed knock back direction
            if(curFacing != dwCombat.knockBackDir){
                data.knockBackDir = -1;
            }
            else{
                data.knockBackDir = 1;
            }
            GetKnockedBack(data.knockBackDir, 50f);
        }
        
        // if get heavy attacked by the wizard
        var ewCombat1 = data.initiator.GetComponentInParent<EW1CombatManager>();
        var ewCombat2 = data.initiator.GetComponentInParent<EW2CombatManager>();

        if(ewCombat1 != null && ewCombat1.isHeavyAttacking){
            // cache player facing direction

            int curFacing = 0;

            if(transform.localScale.x > 0){
                curFacing = 1;
            }
            else{
                curFacing = -1;
            }

            // Debug.Log($"Incoming dir :{data.knockBackDir}");
            if(curFacing != data.knockBackDir){
                data.knockBackDir = -1;
            }
            else{
                data.knockBackDir = 1;
            }

            
            // Debug.Log($"Modified dir :{data.knockBackDir}");
                                

            GetKnockedBack(data.knockBackDir, 50f);
        }
        if(ewCombat2 != null && ewCombat2.isHeavyAttacking){
            // cache player facing direction

            int curFacing = 0;

            if(transform.localScale.x > 0){
                curFacing = 1;
            }
            else{
                curFacing = -1;
            }

            
            if(curFacing != data.knockBackDir){
                data.knockBackDir = -1;
            }
            else{
                data.knockBackDir = 1;
            }
            GetKnockedBack(data.knockBackDir, 50f);
        }
        // make sure to finish animation and disable speed.
        yield return AnimationFinishes("hurt", knight.parameter.RB.linearVelocity.x);

        

        getHit = false;
    }

    // determine when animation can be broken
    public bool IsInvincible(){
        // prob disable hurt box collider
        if(knight.parameter.movementManager.isRolling){
            return true;
        }
        return false;
    }

    
    
    public void GetKnockedBack(float incomingDir, float damage){

        float dir = 0;
        // dir is related to x-axis
        if(incomingDir > 0){
            dir = 1;
        }
        else if(incomingDir < 0){
            dir = -1;
        }
        float force = damage; 
        knight.parameter.RB.AddForce(new Vector2(force * dir, 0), ForceMode2D.Impulse);
    }
    
    public bool ShouldKnockback(){
        if(curLightAttack == "Attack3"){
            return true;
        }
        if(curHeavyAttack == "PowerAttack1"){
            return true;
        }
        return false;
    }
}