using UnityEngine;
using System.Collections;
public enum NSAttackType{
    Attack1 = 1, Attack2 = 2, Attack3 = 3
}


public class NSCombatManager : EnemyCombatManager
{

    [SerializeField] private NewSkeleton newSkeleton;

    [SerializeField] private Knight knight;

    private HitData data;

    public bool getHit;

    public int getHitCount;

    private int randomThreshold;
    
    [SerializeField] private float curInvulnerableTimer;
    private float invulnerableTimer = 5f; // 5 seconds
    /*
        Scenario: a group of skeletons will cooperate to attack the player.
        the total number could be around 5. with 1 flanker, 1 frontliner, and 3 back-uper.


        1. 2 frontliner will hunt the player

        2. flank will join the fight if player is defending for some time

        3. 2 back-uper will join the fight if either of their ally falls. they replace the position that has fallen.
            when they are watching in the back, they play defend animation and attack if player comes around.

        4. Threat Level Shifting (Agro Switch)
            Skeletons re-evaluate target every few seconds.

            If player rolls too much or backs off, flanker becomes aggressive.

            If frontliner is low HP, back-uppers advance even without deaths.

            Purpose: Keeps combat dynamic, avoids player cheesing by backing off.

        5. if a skeleton gets attacked, after the animation fully played, 
            start a timer that within which, the skeleton does not transition to hurt state again but still take damage if hit
        
        psudo-code: *
            On hit(): transition to hurt state and increment hurt counts.
            if hurt counts is above 2-5(random threshold): 
            1. reset hurt counts
            2. start a timer within which skeleton will not be transitioned to hurt state.
    */  

    void Update(){
        UpdateCurInvulnerableTimer();
    }
    private bool HaveSuperArmor(){
        return getHitCount >= randomThreshold;
    }

    private void UpdateCurInvulnerableTimer(){
        if(curInvulnerableTimer >= 0){
            curInvulnerableTimer -= Time.deltaTime;
        }
        
    }

    void OnEnable(){
        EventManager.OnHitOccured += GetHit;
        EventManager.OnEnemyDied += Die;
    }
    void OnDisable(){
        EventManager.OnHitOccured -= GetHit;
        EventManager.OnEnemyDied -= Die;
    }

    private void Die(GameObject newSkeleton){
        // filter
        if(newSkeleton != this.newSkeleton.gameObject){
            return;
        }

        // transition to death;
        this.newSkeleton.TransitionState(NSStateType.Die);

    }
    public override void GetHit(HitData data){
        if(newSkeleton.parameter.healthManager.isDead){
            return;
        }
        if(data == null){
            return;
        }
        // pass data
        this.data = data;

        // only respond if this object get hit.
        if(data.targetHurtBox != newSkeleton.parameter.nsHurtBox){
            return;
        }
        
        // skeleton is sure to take damage
        newSkeleton.parameter.healthManager.TakeDamage(data.damage);

        // do attack sense and special hurt feedback here.

        if(knight.parameter.combatManager.isShieldStriking){
            GetKnockedBack(data.knockBackDir, 10f);
        }

        // if is in invulnerable state
        if(curInvulnerableTimer > 0){
            // do blocked effect here!!!
            return;
        }

        // cache get hit threshold 
        if(getHitCount == 0){
            randomThreshold = Random.Range(2, 5); // [2,5)
        }

        // check if skeleton has been spammed
        if(!HaveSuperArmor()){
            // if not yet, skeleton still responds.
            newSkeleton.TransitionState(NSStateType.Hurt);
            // increment get hit count
            getHitCount ++;
        }
        else{
            // set invulnerability timer and reset get hit count
            curInvulnerableTimer = invulnerableTimer;
            getHitCount = 0;
        }
        
    }
    private void GetKnockedBack(float incomingDir, float force){
        float dir = 0;
        if(incomingDir > 0){
            dir = 1;
        }
        else{
            dir = -1;
        }

        newSkeleton.parameter.RB.AddForce(new Vector2(force * dir, 0), ForceMode2D.Impulse);
    }
    public IEnumerator ExecuteGetHit(){
        // set flag
        getHit = true;

        // get knocked back
        GetKnockedBack(data.knockBackDir, 5f);

        // play animation
        newSkeleton.parameter.animator.Play("Hurt");

        yield return base.AnimationFinishes("Hurt", newSkeleton.parameter.animator);

        getHit = false;
    }

    public NSAttackType DecideAttack(){
        // P(attack1) = p(attack2) = 40%
        // P(combo) = 20%
        int randomValue = UnityEngine.Random.Range(0, 100);
        if(randomValue >= 0 && randomValue < 40){
            return NSAttackType.Attack1;
        }
        else if(randomValue >= 40 && randomValue < 80){
            return NSAttackType.Attack2;
        }
        else{
            return NSAttackType.Attack3;
        }
    }

    public IEnumerator Attack1(){
        // set flag
        // play animation
        // set damage
        base.isAttacking = true;

        base.damage = 15f; // hardcoded damage
        
        newSkeleton.parameter.animator.Play("Attack1");

        // disable velocity
        newSkeleton.parameter.movementManager.DisableVelocity();

        yield return base.AnimationFinishes("Attack1", newSkeleton.parameter.animator);

        base.isAttacking = false;
    }

    public IEnumerator Attack2(){
        // set flag
        // play animation
        base.isAttacking = true;

        base.damage = 15f; // hardcoded damage

        newSkeleton.parameter.animator.Play("Attack2");

         // disable velocity
        newSkeleton.parameter.movementManager.DisableVelocity();

        yield return base.AnimationFinishes("Attack2", newSkeleton.parameter.animator);

        base.isAttacking = false;
    }

    public IEnumerator Attack3(){
        // set flag
        // play combo animation
        base.isAttacking = true;

        base.damage = 15f; // hardcoded damage

        // disable velocity
        newSkeleton.parameter.movementManager.DisableVelocity();
        newSkeleton.parameter.animator.Play("Attack1");

        yield return base.AnimationFinishes("Attack1", newSkeleton.parameter.animator);

        yield return null;

        newSkeleton.parameter.animator.Play("Attack2");

        yield return base.AnimationFinishes("Attack2", newSkeleton.parameter.animator);

        base.isAttacking = false;
    }

    
    
}
