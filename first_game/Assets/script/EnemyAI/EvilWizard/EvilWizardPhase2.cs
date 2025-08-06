using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public enum Phase2StatesTypes{
    Walk, AttackWithEffect, AttackWithoutEffect, HomingLaser, SummonWolf,
    LaserWall, Hurt, Phase2Death,
    Start, Phase2Decide, Phase2Vulnerable
}

[Serializable]
public class EvilWizardPhase2Parameter{
    public const float HEALTH_THRESHOLD_PHASE1 = 0.8f * MAX_HEALTH;

    public const float HEALTH_THRESHOLD_PHASE2 = 0.4f * MAX_HEALTH;

    public Animator animator;

    public const int IS_HEALTH = 1;

    public const int IS_WOUNDED = 2;

    public const int IS_UNHEALTHY = 3;

    public float health;

    public const float MAX_HEALTH = 1000f;

    public float attackCircleRadius;

    public Transform attackCircleCentre;

    public float walkSpeed;

    public float flipOffSet;

    public Transform target;

    public Transform self;

    public LayerMask playerLayer;

    public GameObject portal;

    public GameObject laser;

    public Rigidbody2D RB;

    public GameObject darkWolfPrefab;

    public GameObject currentDarkWolf;

    public DarkWolf currentDarkWolfScript;

    public Transform wizardPhase1;

    [Header("Cool downs")]
    public float maxHomingLaserCoolDown = 8f;

    public float hominglaserCoolDown = 8f;

    public float maxWolfCoolDown = 15f;

    public float wolfCoolDown = 15f;

    public float maxLaserWallCoolDown = 15f;

    public float laserWallCoolDown = 15f;

    [Header("boolens")]
    public float laserWallSpacing;

    public bool isSummoningLaserWall;

    public bool isSummoningHomingLaser;

    public bool isInCoroutine;

    public bool wolfIsAlive;

    public bool canSummonWolf = true;

    public bool canSummonLaserWall = true;

    public bool canSummonHomingLaser = true;

    public bool isDead = false;

    public bool isFacingRight = false;
}

public class EvilWizardPhase2 : MonoBehaviour
{   
    public EvilWizardPhase2Parameter parameter;

    public StateTransitionInterface currentState;

    private Dictionary<Phase2StatesTypes, StateTransitionInterface> states = new Dictionary<Phase2StatesTypes, StateTransitionInterface>();
    
    void Start()
    {   
        parameter.health = EvilWizardPhase2Parameter.MAX_HEALTH;

        states.Add(Phase2StatesTypes.Start, new EWStartState(this));
        states.Add(Phase2StatesTypes.Phase2Decide, new EWPhase2DecideState(this));
        states.Add(Phase2StatesTypes.Walk, new EWWalkState(this));
        states.Add(Phase2StatesTypes.AttackWithEffect, new EWAttackWithEffectState(this));
        states.Add(Phase2StatesTypes.AttackWithoutEffect, new EWAttackWithoutEffectState(this));
        states.Add(Phase2StatesTypes.HomingLaser, new EWHomingLaserState(this));
        states.Add(Phase2StatesTypes.SummonWolf, new EWSummonWolfState(this));
        states.Add(Phase2StatesTypes.LaserWall, new EWLaserWallState(this));
        states.Add(Phase2StatesTypes.Hurt, new EWHurtState(this));
        states.Add(Phase2StatesTypes.Phase2Vulnerable, new EWPhase2VulnerableState(this));
        states.Add(Phase2StatesTypes.Phase2Death, new Phase2DeathState(this));
        
        TransitionState(Phase2StatesTypes.Start);
    }

    
    void Update()
    {   
        if(parameter.health <= 0){
            Die();
            return;
        }
        
        CheckHomingLaserEndingTime();
        CheckLaserWallEndingTime();
        CheckWolfDeathTime();
        currentState.OnUpdate();
        
    }

    

    
    public void TransitionState(Phase2StatesTypes type){
        Debug.Log($"[EW] Transitioning {currentState} to {type}");
        if(currentState != null){
            currentState.OnExit();
        }

        currentState = states[type];

        currentState.OnEnter();
    }

    public void Die(){
        if(!parameter.isDead){
            parameter.animator.Play("Death");
            parameter.isDead = true;
        }
    }
    // track since when has the wolf died

    public void CheckWolfDeathTime(){
        if(parameter.currentDarkWolf == null){
            return;
        }
        if(!parameter.wolfIsAlive){
            return;
        }
        if(parameter.wolfIsAlive){
            // wolf has just died
            if(parameter.currentDarkWolfScript.parameter.health <= 0){
                // wait until wolf finishes its animation and then destroy itself
                StartCoroutine(DestroyWolf());
                parameter.wolfIsAlive = false;
            
                // start counting down
                StartCoroutine(WolfCoolingDown());
            }  
        }
    }

    public void CheckLaserWallEndingTime(){
        if(!parameter.isSummoningLaserWall){
            return;
        }
        if(parameter.isSummoningLaserWall){
            // has just exited laserWall coroutine
            if(!parameter.isInCoroutine){
                parameter.isSummoningLaserWall = false;
                StartCoroutine(SummonLaserWallCoolDown());
            }
        }
    }

    public void CheckHomingLaserEndingTime(){
        if(!parameter.isSummoningHomingLaser){
            return;
        }
        if(parameter.isSummoningHomingLaser){
            if(!parameter.isInCoroutine){
                parameter.isSummoningHomingLaser = false;
                StartCoroutine(SummonHomingLaserCoolDown());
            }
        }
    }

    public int checkBossHeathStatus(){
        if(parameter.health > EvilWizardPhase2Parameter.HEALTH_THRESHOLD_PHASE1){
            return EvilWizardPhase2Parameter.IS_HEALTH;
        }
        else if(parameter.health < EvilWizardPhase2Parameter.HEALTH_THRESHOLD_PHASE1 && parameter.health > EvilWizardPhase2Parameter.HEALTH_THRESHOLD_PHASE2){
            return EvilWizardPhase2Parameter.IS_WOUNDED;
        }
        else{
            return EvilWizardPhase2Parameter.IS_UNHEALTHY;
        }
    }

    public bool PlayerIsInRange(){
        return Physics2D.OverlapCircle(parameter.attackCircleCentre.position, parameter.attackCircleRadius, parameter.playerLayer);
    }
    public void OnDrawGizmos(){
        Gizmos.DrawWireSphere(parameter.attackCircleCentre.position, parameter.attackCircleRadius);
    }
    
    /*
        We have to adjust FlipTo method because of the offset in the wizard's sprite
    */
    public void FlipTo(Transform target)
    {
        if (target == null) return;
        float realPivot = 0;
        // define a adjusted pivot
        if(parameter.isFacingRight){
            realPivot = transform.position.x - parameter.flipOffSet;
        }
        else{
            realPivot = transform.position.x + parameter.flipOffSet;
        }
        
        bool shouldFaceRight = realPivot < target.position.x;

        // use a field parameter to prevent jittering
        if (shouldFaceRight != parameter.isFacingRight)
        {
            // Flip scale
            if(shouldFaceRight){
                transform.localScale = new Vector3(-6, 6, 6);
            }
            else{
                transform.localScale = new Vector3(6, 6, 6);
            }

            // Adjust position once on flip manually
            Vector3 newPos = transform.position;
            if(shouldFaceRight){
                newPos.x += parameter.flipOffSet;
            }
            else{
                newPos.x -= parameter.flipOffSet;
            }
            transform.position = newPos;

            // update isFacingRight
            parameter.isFacingRight = shouldFaceRight;
        }
    }

    public void WalkTowardsPlayer(){
        if(parameter.target == null){
            return;
        }

        float direction = parameter.target.position.x - transform.position.x;

        if(direction > 0){
            parameter.RB.linearVelocity = new Vector2(1 * parameter.walkSpeed, parameter.RB.linearVelocity.y);
        }
        else{
            parameter.RB.linearVelocity = new Vector2(-1 * parameter.walkSpeed, parameter.RB.linearVelocity.y);
        }
    }

    private IEnumerator DestroyWolf(){
        yield return new WaitForSeconds(1f);
        // Destroy the wolf
        Destroy(parameter.currentDarkWolf);
    }
    private IEnumerator WolfCoolingDown(){
        while(parameter.wolfCoolDown > 0){
            parameter.wolfCoolDown -= Time.deltaTime;
            yield return null;
        }
        if(parameter.wolfCoolDown <= 0){
            parameter.canSummonWolf = true;
            parameter.wolfCoolDown = parameter.maxWolfCoolDown;
        }
    }

    private IEnumerator SummonLaserWallCoolDown(){
        while(parameter.laserWallCoolDown > 0){
            parameter.laserWallCoolDown -= Time.deltaTime;
            yield return null;
        }
        if(parameter.laserWallCoolDown <= 0){
            parameter.canSummonLaserWall = true;
            parameter.laserWallCoolDown = parameter.maxLaserWallCoolDown;
        }
    }

    private IEnumerator SummonHomingLaserCoolDown(){
        while(parameter.hominglaserCoolDown > 0){
            parameter.hominglaserCoolDown -= Time.deltaTime;
            yield return null;
        }
        if(parameter.hominglaserCoolDown <= 0){
            parameter.canSummonHomingLaser = true;
            parameter.hominglaserCoolDown = parameter.maxHomingLaserCoolDown;
        }
    }

    public IEnumerator RiseFromDeathAndIdle(){
        parameter.isInCoroutine = true;

        Vector3 phase1DeathSite = parameter.wizardPhase1.position;
        
        parameter.self.position = phase1DeathSite;

        parameter.animator.Play("RiseFromDeath");
        while(!parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("RiseFromDeath")){
            yield return null;
        }

        AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        while(true){
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime >= 0.99f){
                break;
            }
            yield return null;
        }

        parameter.animator.Play("Idle");

        yield return new WaitForSeconds(1f);

        parameter.isInCoroutine = false;
    }
    /*
        wizard first cast a spell and then summon 7 lasers homing the player.
    */
    public IEnumerator SummonLasers(){
        parameter.isInCoroutine = true;
        parameter.canSummonHomingLaser = false;
        parameter.isSummoningHomingLaser = true;
        int laserNum = 0;
        // if there is a wolf, nerf homing lasers
        if(parameter.wolfIsAlive){
            laserNum = 5;
        }
        else{
            laserNum = 7;
        }

        List<GameObject> laserPool = new List<GameObject>();

        for(int i = 0; i < laserNum; i++){
            // cache lasers
            GameObject laser = Instantiate(parameter.laser);
            laser.transform.localScale = new Vector3(10, 10, 10);
            laserPool.Add(laser);
        }
        
        parameter.animator.Play("Cast");

        // make sure to enter cast animation
        while(!parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Cast")){
            yield return null;
        }
        AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        // make sure animation is played thoroughly
        while(true){
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime >= 0.99f){
                break;
            }
            yield return null;
        }

        // wizard is free to go
        parameter.isInCoroutine = false;
        
        
        Vector3 playerPos = parameter.target.position;

        // use temPos to avoid situation where player get striken by lasers when staying static
        Vector3 temPos = parameter.self.position;
        
        for(int i = 0; i < laserNum; i++){
            // set laser position
            playerPos.x = parameter.target.position.x;
            playerPos.y = parameter.target.position.y + 2.7f;
            
            float randomNum = UnityEngine.Random.Range(-3f, 3f);
            GameObject laser = laserPool[i];

            // if the player is static, prevent lasering the same spot every time.
            if(playerPos.x == temPos.x){
                temPos = playerPos;
                playerPos.x += randomNum;
                laser.transform.position = playerPos;
            }
            else{
                laser.transform.position = playerPos;
                temPos = playerPos;
            }
            

            // summon laser
            laser.SetActive(true);
            laser.GetComponent<Animator>().Play("Spell");

            // call coroutine to disable laser in time
            StartCoroutine(DestroyWhenAnimationEnds(laser, "Spell"));
            
            // laser intervals
            yield return new WaitForSeconds(0.22f);
        }

        
    }

    private IEnumerator DestroyWhenAnimationEnds(GameObject gameObject, string animationName){
        Animator animator = gameObject.GetComponent<Animator>();

        // wait until animation is being played
        while(!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName)){
            yield return null;
        }
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1){
            yield return null;
        }
        Destroy(gameObject);
    }

    private IEnumerator DisableWhenAnimationEnds(GameObject gameObject, string animationName){
        Animator animator = gameObject.GetComponent<Animator>();

        // wait until animation is being played
        while(!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName)){
            yield return null;
        }
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1){
            yield return null;
        }
        gameObject.SetActive(false);
    }
    /*
        wizard first cast a spell and summon a portal in front of player.
        Then teleport wizard to player
    */
    public IEnumerator TeleportAndAttack(){
        parameter.isInCoroutine = true;

        Transform portalTransform = parameter.portal.transform;

        Animator portalAnimator = parameter.portal.GetComponent<Animator>();
        parameter.animator.Play("Cast");

        // reset speed
        parameter.RB.linearVelocity = Vector2.zero;

        // make sure to enter cast animation
        while(!parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Cast")){
            yield return null;
        }
        AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        // make sure animation is played thoroughly
        while(true){
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime >= 0.99f){
                break;
            }
            yield return null;
        }

        // teleport portal to player first
        Vector3 newPos = parameter.target.position;
        if(parameter.target.localScale.x > 0){
            newPos.x = parameter.target.position.x + parameter.attackCircleRadius;
            newPos.y = parameter.self.position.y;
        }
        else{
            newPos.x = parameter.target.position.x - parameter.attackCircleRadius;
            newPos.y = parameter.self.position.y;
        }
        portalTransform.position = newPos;
        
        parameter.portal.SetActive(true);


        portalAnimator.Play("Spell-NoEffect");

        yield return new WaitForSeconds(0.5f);
        // teleport wizard
        parameter.self.position = newPos;

        // disable portal
        parameter.portal.SetActive(false);
        FlipTo(parameter.target);

        // execute attack
        parameter.animator.Play("Attack");

        // make sure to enter Attack animation
        while(!parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
            yield return null;
        }
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        // make sure animation is played thoroughly
        while(true){
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime >= 0.99f){
                break;
            }
            yield return null;
        }
        parameter.isInCoroutine = false;
        
    }
    
    /*
        wizard will summon a wolf and then transition to summon-laser state.
        wizard will be invincible by disabling its box collider, and keep casting every 4 seconds after finishing a cast
        
        we will set a flag stating there is and only one wolf in the scene. 
        if there is a wolf then the wizard will stay in laser state.
        if the wolf has died, the wizard will have a cool down for such skill for 20 seconds, when it 
        only can execute healthy state phases of attack.
    */
    public IEnumerator SummonAWolf(){
        parameter.isInCoroutine = true;
        parameter.canSummonWolf = false;
        
        /*
            there will be a spell above the wolf before wolf respawns

            wolf rises from death and then start striking

            wolf is respawned to enter mode 2
        */
        
        // cache portal
        Transform portalTransform = parameter.portal.transform;
        Animator portalAnimator = parameter.portal.GetComponent<Animator>();

        // cache darkWolf
        GameObject darkWolf = Instantiate(parameter.darkWolfPrefab);
        Transform darkWolfTransform = darkWolf.transform;
        Animator darkWolfAnimator = darkWolf.GetComponent<Animator>();
        DarkWolf darkWolfScript = darkWolf.GetComponent<DarkWolf>();
        parameter.currentDarkWolf = darkWolf;
        parameter.currentDarkWolfScript = darkWolfScript;

        // cast a spell 
        parameter.animator.Play("Cast");

        // determine portal and wolf's spawn point
        float randomSpawnX = UnityEngine.Random.Range(parameter.target.position.x - 20f, parameter.target.position.x + 20f);

        Vector3 randomSpawnPoint = new Vector3(randomSpawnX, parameter.target.position.y + 4f, parameter.target.position.z);

        // make sure to enter cast animation
        while(!parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Cast")){
            yield return null;
        }

        

        AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        // make sure animation is played thoroughly
        while(true){
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime >= 0.99f){
                break;
            }
            yield return null;
        }
        
        // summon portal
        Vector3 newPortalScale = new Vector3(2f, 2f, 1f);
        portalTransform.localScale = newPortalScale;
        portalTransform.position = randomSpawnPoint;
        parameter.portal.SetActive(true);
        portalAnimator.Play("Spell-NoEffect");

        yield return new WaitForSeconds(0.2f);
        // wizard can recover its action from now
        parameter.isInCoroutine = false;

        // summon wolf

        darkWolfTransform.position = randomSpawnPoint;
        darkWolf.SetActive(true);

        parameter.wolfIsAlive = true;
        
        
        // disable portal
        parameter.portal.SetActive(false);

        // set wolf's mode to mode 2 and start its FSM from mode 2
        
        darkWolfScript.parameter.mode = 2;
        darkWolfScript.TransitionState(StateTypes1.Mode2);
    }

    /*
        Wizard will summon a laser wall consisting of 11 top-down lasers evenly spaced around the player.

        - Initially, the central laser will be placed directly on the player's current position.
        - After the first batch of laser animations completes, the central laser and all others will shift to new positions with some offset.
        - This shifting process represents one cycle of the laser wall attack.
        - The laser wall cycle will repeat two more times (for a total of three cycles).
    
    After all cycles are complete, the wizard will exit the SummonLaserWall state and transition to the Vulnerable state.
    */

    public IEnumerator SummonLaserWall(){
        parameter.isSummoningLaserWall = true;
        parameter.isInCoroutine = true;
        parameter.canSummonLaserWall = false;
        int laserNum = 11;
        
        // initialise and cache player's first time location
        Vector3 firstTimeLocation = new Vector3(1, 1, 1);
        Vector3 centralLaserLocation = new Vector3(1, 1, 1);

        int totalRounds = 6;
        for(int round = 1; round <= totalRounds; round++){
            List<GameObject> laserPool = new List<GameObject>();
            // instantiate new lasers every time
            for(int i = 0; i < laserNum; i++){
                // cache lasers
                
                GameObject laser = Instantiate(parameter.laser);
                laser.transform.localScale = new Vector3(10, 10, 10);
                laserPool.Add(laser);
            }   

            // wizard casts
            parameter.animator.Play("Idle", 0, 0);  // leave current state
            yield return null; 
            parameter.animator.Play("Cast", 0, 0);
            
            // make sure to enter cast animation
            while(!parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Cast")){
                yield return null;
            }
            AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            // make sure animation is played thoroughly
            while(true){
                info = parameter.animator.GetCurrentAnimatorStateInfo(0);
                if(info.normalizedTime >= 0.99f){
                    break;
                }
                yield return null;
            }
            parameter.animator.Play("Idle");

            // set central laser position
            if(round == 1){
                centralLaserLocation = parameter.target.position;
                centralLaserLocation.y = parameter.target.position.y + 2.7f;
                firstTimeLocation = centralLaserLocation;
            }
            else{
                centralLaserLocation = firstTimeLocation;
            }

            int centralIndex = laserNum / 2;
            
            // set the lasers' position
            for(int i = 0; i < laserNum; i++){
                Vector3 pos = centralLaserLocation;
                if(round % 2 == 1){
                    pos.x = (i - centralIndex) * parameter.laserWallSpacing + firstTimeLocation.x;
                }
                
                else{
                    pos.x = (i - centralIndex) * parameter.laserWallSpacing + firstTimeLocation.x + 0.5f * parameter.laserWallSpacing;
                }
                
                laserPool[i].transform.position = pos;
            }

            // summon lasers

            for(int i = 0; i < laserNum; i++){
                GameObject laser = laserPool[i];

                Animator laserAnimator = laser.GetComponent<Animator>();

                laser.SetActive(true);
                laserAnimator.Play("Spell");
                yield return null;
                
                // call a coroutine to destroy lasers after finishing its animation.
                StartCoroutine(DestroyWhenAnimationEnds(laser, "Spell"));
            }
            
            
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);
        parameter.isInCoroutine = false;
    }


}
