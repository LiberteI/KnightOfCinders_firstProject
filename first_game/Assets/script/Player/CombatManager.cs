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

    [SerializeField] private Knight knight;

    public bool isLightAttacking;

    public bool isHeavyAttacking;

    public bool isDefending;

    public bool isShieldStriking;

    public bool isJumpAttacking;

    public bool isRunAttacking;

    public bool getHit;

    public float damage;

    public float knockBackForce;

    [Header("AttackStepFlag")]
    public string curLightAttack;

    public string curHeavyAttack;
    void Start(){
    }
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

    public bool IsAttacking(){
        return isLightAttacking || isHeavyAttacking || isShieldStriking || isJumpAttacking || isRunAttacking;
    }

    public void Die(){
        // transition to death
        knight.TransitionState(KnightStateTypes.Die);
    }

    private void SetDamage(){
        if(isLightAttacking){
            damage = 20f;
        }
        else if(isHeavyAttacking){
            damage = 30f;
        }
        else if(isShieldStriking){
            damage = 1f;
        }
        else if(isJumpAttacking){
            damage = 25f;
        }
        else if(isRunAttacking){
            damage = 20f;
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

        // do stamina here.
        
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
                        GetKnockedBack(data.knockBackDir, 0.5f);
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
        // set flag
        getHit = true;

        // get damage early
        knight.parameter.healthManager.TakeDamage(data.damage);

        // play animation
        knight.parameter.animator.Play("hurt");

        // make sure to finish animation and disable speed.
        yield return AnimationFinishes("hurt", 0);

        

        getHit = false;
    }

    // determine when animation can be broken
    private bool IsInvincible(){
        // prob disable hurt box collider
        if(knight.parameter.movementManager.isRolling){
            return true;
        }
        return false;
    }

    
    
    private void GetKnockedBack(float incomingDir, float damage){
        // scale force by damage:
        float dir = 0;
        if(incomingDir > 0){
            dir = 1;
        }
        else if(incomingDir < 0){
            dir = -1;
        }
        float force = damage * 10f; 
        knight.parameter.RB.AddForce(new Vector2(force * dir, 0), ForceMode2D.Impulse);
    }
    
}