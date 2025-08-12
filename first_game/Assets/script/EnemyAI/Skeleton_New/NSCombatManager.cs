using UnityEngine;
using System.Collections;
public enum NSAttackType{
    Attack1 = 1, Attack2 = 2, Attack3 = 3
}


public class NSCombatManager : EnemyCombatManager
{

    public NewSkeleton newSkeleton;

    public Knight knight;

    [Header("Getting Hit")]
    private HitData data;

    public bool getHit;

    public int getHitCount;

    private int randomThreshold;
    
    [SerializeField] private float curInvulnerableTimer;
    private float invulnerableTimer = 5f; // 5 seconds

    

    /*
        1. if a skeleton gets attacked, after the animation fully played,  *
            start a timer that within which, the skeleton does not transition to hurt state again but still take damage if hit
        
        psudo-code: *
            On hit(): transition to hurt state and increment hurt counts.
            if hurt counts is above 2-5(random threshold): 
            1. reset hurt counts
            2. start a timer within which skeleton will not be transitioned to hurt state.
    */
    [Header("Skeleton Squat")]
    public SkeletonRole currentRole;

    [Header("Flanker")]
    [SerializeField] private float decideTimer;

    [SerializeField] private float curDecideTimer;

    public float curDistance;

    [SerializeField] private float desiredDistance;

    [SerializeField] private float distanceMargin;
    
    // around 3s?
    [SerializeField] private float flankerAttackTimer;

    [SerializeField] private float curFlankerAttackTimer;


    /*
        2. 1 flanker will sneak around and hunt player with a swifter speed but less HP.
            - flanker will keep distance with the player.(sneaky state). 
              In sneaky state, the flanker maintains mid-range distance. It will decide how to move every 0.5s.
              if the player is chasing the flanker, the flanker will retreat but not instantly. 
              if get hit, it will retreat instantly.
              if the player is too far away, the flanker will approach the player.

              psudo-code:
              private float decideTimer = 0.5f;
              private float curDecideTimer;

              private float curDistance; // current distance
              private float desiredDistance; // the range that skeleton wants to keep from the player
              private float distanceMargin; // a margin value that keeps skeleton from jittering
               


              Update():
              FlipTo(player);
              CalculateDistance();
              if(curDecideTimer >= 0) keep deducting.

              
              if(getHit) RetreatImmediately();
              every 0.5s: DecideMove();
              
              void DecideMove():
               // too close: desiredDistance - margin ==> run away
               // too far: desiredDistance + margin ==> sneak on player
               // just right: [desiredDistance - margin, desiredDistance + margin] ==> idle
              
    */
    void Start(){
        curFlankerAttackTimer = flankerAttackTimer;
    }
    void Update(){
        UpdateCurInvulnerableTimer();
        
        UpdateCurDecideTimer();

        UpdateCurFlankerAttackTimer();
    }

    public void CalculateDistance(){
        curDistance = Vector2.Distance(transform.position, knight.transform.position);
    }

    private void UpdateCurFlankerAttackTimer(){
        if(curFlankerAttackTimer > 0){
            curFlankerAttackTimer -= Time.deltaTime;
        }
    }

    private void UpdateCurDecideTimer(){
        if(curDecideTimer > 0){
            curDecideTimer -= Time.deltaTime;
        }
    }
    
    public bool IsInFrontOfKnight(){
        // skeleton is to the left: Ps - Pk < 0 && Lk < 0
        // skeleton is to the right: Ps - Pk > 0 && Lk > 0

        if(transform.position.x - knight.transform.position.x < 0 && knight.transform.localScale.x < 0){
            return true;
        } 
        if(transform.position.x - knight.transform.position.x > 0 && knight.transform.localScale.x > 0){
            return true;
        }
        return false;

    }
    public bool FlankerShouldAttack(){
        if(curFlankerAttackTimer > 0){
            return false;
        }
        else{
            return true;
        }
        
    }
    
    public void DecideMove(){
        if(curDecideTimer > 0){
            return;
        }

        if(curDistance > desiredDistance + distanceMargin){
            // skeleton is too far from the player
            
            // play walk animation
            newSkeleton.parameter.animator.Play("Walk");
            // face the player
            newSkeleton.parameter.movementManager.FlipTo(knight.transform);
            // move towards player.
            newSkeleton.parameter.movementManager.WalkTowardsPlayer();

        }
        else if(curDistance < desiredDistance - distanceMargin){
            // skeleton is too close from the player

            // play walk animation
            newSkeleton.parameter.animator.Play("Walk");

            // look away
            newSkeleton.parameter.movementManager.LookAwayFromPlayer(knight.transform);
            
            // flee from the player
            newSkeleton.parameter.movementManager.FleeAwayFromPlayer();
            
        }
        else{
            // position is just right.

            // disable velocity
            newSkeleton.parameter.movementManager.DisableVelocity();
            // idle
            newSkeleton.parameter.animator.Play("Idle");
        }

        // reset the timer
        curDecideTimer = decideTimer;
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
        // does not get hit if is back-uper
        if(currentRole == SkeletonRole.Backuper){
            return;
        }
        if(newSkeleton.parameter.healthManager.isDead){
            return;
        }
        if(data == null){
            return;
        }
        // pass data
        this.data = data;

        // only respond if this object gets hit.
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

        base.damage = 7f; // hardcoded damage
        
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

        base.damage = 7f; // hardcoded damage

        newSkeleton.parameter.animator.Play("Attack2");

         // disable velocity
        newSkeleton.parameter.movementManager.DisableVelocity();

        yield return base.AnimationFinishes("Attack2", newSkeleton.parameter.animator);

        // reset timer
        if(currentRole == SkeletonRole.Flanker){
            GenerateRandomAndResetTimer();
        }
        base.isAttacking = false;
    }

    public IEnumerator Attack3(){
        // set flag
        // play combo animation
        base.isAttacking = true;

        base.damage = 7f; // hardcoded damage

        // disable velocity
        newSkeleton.parameter.movementManager.DisableVelocity();
        newSkeleton.parameter.animator.Play("Attack1");

        yield return base.AnimationFinishes("Attack1", newSkeleton.parameter.animator);

        yield return null;

        newSkeleton.parameter.animator.Play("Attack2");

        yield return base.AnimationFinishes("Attack2", newSkeleton.parameter.animator);

        // reset timer
        if(currentRole == SkeletonRole.Flanker){
            GenerateRandomAndResetTimer();
        }
        base.isAttacking = false;
    }
    private void GenerateRandomAndResetTimer(){
        int randomAttackInterval = UnityEngine.Random.Range(3, 6); // [3, 5)
        flankerAttackTimer = randomAttackInterval;
        curFlankerAttackTimer = flankerAttackTimer;
    }
}
