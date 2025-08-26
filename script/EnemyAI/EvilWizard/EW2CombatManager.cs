using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EW2CombatManager : EnemyCombatManager
{
    public const int IS_HEALTHY = 1;

    public const int IS_WOUNDED = 2;

    public const int IS_UNHEALTHY = 3;

    public float healthThresholdPhase1;

    public float healthThresholdPhase2;

    [Header("Damage")]
    public static float LIGHT_DAMAGE = 15f;

    public static float HEAVY_DAMAGE = 30f;

    public static float LASER_DAMAGE = 10f;

    public static float LASER_WALL_DAMAGE = 20f;

    [Header("Cool downs")]
    public float maxHomingLaserCoolDown = 8f;

    public float hominglaserCoolDown = 8f;

    public float maxWolfCoolDown = 15f;

    public float wolfCoolDown = 15f;

    public float maxLaserWallCoolDown = 15f;

    public float laserWallCoolDown = 15f;

    public float laserWallSpacing;

    [Header("boolens")]
    
    public bool isSummoningLaserWall;

    public bool isSummoningHomingLaser;

    public bool isInCoroutine;

    public bool wolfIsAlive;

    public bool canSummonWolf = true;

    public bool canSummonLaserWall = true;

    public bool canSummonHomingLaser = true;

    public bool getHit;

    public bool isAttackingWithoutEffect;

    public bool isHeavyAttacking;

    private bool hasCleared;

    public bool isVulnerable;

    [Header("Brain")]
    public EvilWizardPhase2 evilWizard;

    [Header("Runtime")]

    public GameObject currentDarkWolf;

    public DarkWolf currentDarkWolfScript;

    public EnemyHealthManager curDarkWolfHealthManager;

    // used for 
    public CombatManager combatManager;

    public GameObject ew2HurtBox;

    private GameObject healthBarObj2;

    [Header("Get hit")]
    private float getHitCount;

    private int randomThreshold;

    [SerializeField] private float curInvulnerableTimer;
    private float invulnerableTimer = 2f; // 5 seconds

    public float knockBackDir;

    private HitData data;

    

    public Knight knight;

    [SerializeField] private GamePlayCoordinator gpCoordinator;

    public AudioFeedbackManager audioFeedbackManager;

    

    [SerializeField] private GameObject ShieldIcon;

    [SerializeField] private GameObject VulnerableIcon;


    void Start(){
        healthThresholdPhase1 = 0.66f * evilWizard.parameter.healthManager.maxHealth;

        healthThresholdPhase2 = 0.33f * evilWizard.parameter.healthManager.maxHealth;
    }
    void Update(){
        CheckHomingLaserEndingTime();

        CheckLaserWallEndingTime();

        CheckWolfDeathTime();

        UpdateCurInvulnerableTimer();

        ChangeTransparencyIfInvincible();

        SetFeedback();

        UpdateInvulnerableState();
        
        UpdateVulnerableState();
    }
    private void UpdateInvulnerableState(){
        if(curInvulnerableTimer > 0){
            // activate shield icon.
            ShieldIcon.SetActive(true);
        }
        else{
            // disable shield icon
            ShieldIcon.SetActive(false);
        }
    }

    private void UpdateVulnerableState(){
        if(isVulnerable){
            VulnerableIcon.SetActive(true);
        }
        else{
            VulnerableIcon.SetActive(false);
        }
    }
    void OnEnable(){
        // Debug.Log($"{name} subscribing to OnEnemyDied");
        EventManager.OnEnemyDied += Die;

        EventManager.OnHitOccured += GetHit;
    }
    void OnDisable(){
        // Debug.Log($"{name} unsubscribing from OnEnemyDied");
        EventManager.OnEnemyDied -= Die;

        EventManager.OnHitOccured -= GetHit;
    }

    private void SetFeedback(){
        if(isAttackingWithoutEffect){
            base.sourceFeedback.stopTime = 0.5f;

            base.sourceFeedback.duration = 0.1f;

            base.sourceFeedback.amplitude = 0.3f;

            base.sourceFeedback.frequency = 0.3f;
        }
        else{
            base.sourceFeedback.stopTime = 0.25f;

            base.sourceFeedback.duration = 0.1f;

            base.sourceFeedback.amplitude = 0.2f;

            base.sourceFeedback.frequency = 0.2f;
        }
    }
    private void UpdateCurInvulnerableTimer(){
        if(curInvulnerableTimer >= 0){
            curInvulnerableTimer -= Time.deltaTime;
        }
        
    }
    // track since when has the wolf died
    public void CheckWolfDeathTime(){
        if(currentDarkWolf == null){
            return;
        }
        if(!wolfIsAlive){
            return;
        }
        if(wolfIsAlive){
            // wolf has just died
            if(currentDarkWolfScript.parameter.healthManager.curHealth <= 0){
                // wait until wolf finishes its animation and then destroy itself
                StartCoroutine(DestroyWolf());
                wolfIsAlive = false;
            
                // start counting down
                StartCoroutine(WolfCoolingDown());
            }  
        }
    }

    public void CheckLaserWallEndingTime(){
        if(!isSummoningLaserWall){
            return;
        }
        if(isSummoningLaserWall){
            // has just exited laserWall coroutine
            if(!isInCoroutine){
                isSummoningLaserWall = false;
                StartCoroutine(SummonLaserWallCoolDown());
            }
        }
    }

    public void CheckHomingLaserEndingTime(){
        if(!isSummoningHomingLaser){
            return;
        }
        if(isSummoningHomingLaser){
            if(!isInCoroutine){
                isSummoningHomingLaser = false;
                StartCoroutine(SummonHomingLaserCoolDown());
            }
        }
    }

    public int checkBossHeathStatus(){
        if(evilWizard.parameter.healthManager.curHealth > healthThresholdPhase1){
            return EW2CombatManager.IS_HEALTHY;
        }
        else if(evilWizard.parameter.healthManager.curHealth < healthThresholdPhase1 && evilWizard.parameter.healthManager.curHealth > healthThresholdPhase2){
            return EW2CombatManager.IS_WOUNDED;
        }
        else{
            return EW2CombatManager.IS_UNHEALTHY;
        }
    }
    private void Die(GameObject evilWizard){
        // filter
        // Debug.Log(evilWizard);
        if(evilWizard != this.evilWizard.gameObject){
            return;
        }
        ew2HurtBox.tag = "Untagged";
        // transition to death

        this.evilWizard.TransitionState(Phase2StatesTypes.Phase2Death);

    }
    public override void GetHit(HitData data){
        // does not get hit if dead
        if(evilWizard.parameter.healthManager.isDead){
            return;
        }
        if(data == null){
            return;
        }
        // does not get hit if is invincible
        if(IsInvincible()){
            return;
        }
        // cache data
        this.data = data;
        
        // filter: only respond if this object gets hit:
        if(data.targetHurtBox != ew2HurtBox){
            return;
        }
        if(isVulnerable){
            //!!!!!!!!!!!!!!!!!!
            // take damage and play animation
            evilWizard.parameter.healthManager.TakeDamage(data.damage * 2f);
            // play idle to be overidden
            evilWizard.parameter.animator.Play("Idle");
            evilWizard.parameter.animator.Play("Hurt");
            return;
        }
        // always responds to knock back  
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

        // check if dark wolf has been spammed
        if(!HaveSuperArmor()){
            evilWizard.parameter.healthManager.TakeDamage(data.damage);
            // if not yet, dark wolf still responds.
            evilWizard.TransitionState(Phase2StatesTypes.Hurt);

            // increment get hit count
            getHitCount ++;
        }
        else{
            // set invulnerability timer and reset get hit count
            curInvulnerableTimer = invulnerableTimer;
            getHitCount = 0;
        }
    }
    public IEnumerator ExecuteGetHit(){
        // set flag
        getHit = true;
        
        // get knocked back

        // play animation
        evilWizard.parameter.animator.Play("Hurt");

        yield return base.AnimationFinishes("Hurt", evilWizard.parameter.animator);

        getHit = false;
    }
    private void GetKnockedBack(float incomingDir, float force){
        float dir = 0;
        if(incomingDir > 0){
            dir = 1;
        }
        else{
            dir = -1;
        }

        evilWizard.parameter.RB.AddForce(new Vector2(force * dir, 0), ForceMode2D.Impulse);
    }
    private bool HaveSuperArmor(){
        
        if(getHitCount >= randomThreshold){
            return true;
        }
        return false;
    } 

    private void ChangeTransparencyIfInvincible(){
        if(IsInvincible()){
            Color color = evilWizard.parameter.SR.color;

            // set transparency to 0.2
            color.a = 0.2f;

            evilWizard.parameter.SR.color = color;
        }
        else{
            // recover transparency
            Color color = evilWizard.parameter.SR.color;

            color.a = 1f;

            evilWizard.parameter.SR.color = color;
        }
    }
    private bool IsInvincible(){
        if(wolfIsAlive){
            return true;
        }
        if(isSummoningLaserWall){
            return true;
        }
        return false;
    }
    private IEnumerator DestroyWolf(){
        yield return new WaitForSeconds(1f);
        // Destroy the wolf
        Destroy(currentDarkWolf);
        Destroy(healthBarObj2);
    }
    private IEnumerator WolfCoolingDown(){
        while(wolfCoolDown > 0){
            wolfCoolDown -= Time.deltaTime;
            yield return null;
        }
        if(wolfCoolDown <= 0){
            canSummonWolf = true;
            wolfCoolDown = maxWolfCoolDown;
        }
    }

    private IEnumerator SummonLaserWallCoolDown(){
        while(laserWallCoolDown > 0){
            laserWallCoolDown -= Time.deltaTime;
            yield return null;
        }
        if(laserWallCoolDown <= 0){
            canSummonLaserWall = true;
            laserWallCoolDown = maxLaserWallCoolDown;
        }
    }

    private IEnumerator SummonHomingLaserCoolDown(){
        while(hominglaserCoolDown > 0){
            hominglaserCoolDown -= Time.deltaTime;
            yield return null;
        }
        if(hominglaserCoolDown <= 0){
            canSummonHomingLaser = true;
            hominglaserCoolDown = maxHomingLaserCoolDown;
        }
    }

    public IEnumerator RiseFromDeathAndIdle(){
        isInCoroutine = true;

        Vector3 phase1DeathSite = evilWizard.parameter.wizardPhase1.position;
        
        transform.position = phase1DeathSite;

        evilWizard.parameter.animator.Play("RiseFromDeath");
        while(!evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("RiseFromDeath")){
            yield return null;
        }

        AnimatorStateInfo info = evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0);
        while(true){
            info = evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime >= 0.99f){
                break;
            }
            yield return null;
        }

        evilWizard.parameter.animator.Play("Idle");

        yield return new WaitForSeconds(1f);

        isInCoroutine = false;
    }
    /*
        wizard first cast a spell and then summon 7 lasers homing the player.
    */
    public IEnumerator SummonLasers(){
        base.damage = EW2CombatManager.LASER_DAMAGE;

        isInCoroutine = true;
        canSummonHomingLaser = false;
        isSummoningHomingLaser = true;
        int laserNum = 0;
        // if there is a wolf, nerf homing lasers
        if(wolfIsAlive){
            laserNum = 2;
        }
        else{
            laserNum = 5;
        }

        List<GameObject> laserPool = new List<GameObject>();

        for(int i = 0; i < laserNum; i++){
            // cache lasers
            GameObject laser = Instantiate(evilWizard.parameter.laserPrefab);
            
            laserPool.Add(laser);
        }
        
        evilWizard.parameter.animator.Play("Cast");

        // make sure to enter cast animation
        while(!evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Cast")){
            yield return null;
        }
        AnimatorStateInfo info = evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0);
        // make sure animation is played thoroughly
        while(true){
            info = evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime >= 0.99f){
                break;
            }
            yield return null;
        }

        // wizard is free to go
        isInCoroutine = false;
        
        
        Vector3 playerPos = evilWizard.parameter.target.position;

        // use temPos to avoid situation where player get striken by lasers when staying static
        Vector3 temPos = transform.position;
        
        for(int i = 0; i < laserNum; i++){
            // set laser position
            playerPos.x = evilWizard.parameter.target.position.x;
            playerPos.y = evilWizard.parameter.target.position.y + 5f;
            
            float randomNum = UnityEngine.Random.Range(-5f, 5f);
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
            // set runtime laser data
            HitBoxManager tempHitBox = laser.GetComponent<HitBoxManager>();
            tempHitBox.SetInitiator(ew2HurtBox);
            tempHitBox.SetCombatManager(combatManager);

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
        wizard first cast a spell and summon a portalPrefab in front of player.
        Then teleport wizard to player
    */
    public IEnumerator TeleportAndAttack(){
        base.damage = EW2CombatManager.HEAVY_DAMAGE;
        isHeavyAttacking = true;
        isInCoroutine = true;

        Transform portalPrefabTransform = evilWizard.parameter.portalPrefab.transform;

        Animator portalPrefabAnimator = evilWizard.parameter.portalPrefab.GetComponent<Animator>();
        evilWizard.parameter.animator.Play("Cast");

        // reset speed
        evilWizard.parameter.RB.linearVelocity = Vector2.zero;

        // make sure to enter cast animation
        while(!evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Cast")){
            yield return null;
        }
        AnimatorStateInfo info = evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0);
        // make sure animation is played thoroughly
        while(true){
            info = evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime >= 0.99f){
                break;
            }
            yield return null;
        }

        // teleport portalPrefab to player first
        Vector3 newPos = evilWizard.parameter.target.position;
        if(evilWizard.parameter.target.localScale.x > 0){
            newPos.x = evilWizard.parameter.target.position.x + 2f * attackCircleRadius;
            newPos.y = transform.position.y + 5f;
        }
        else{
            newPos.x = evilWizard.parameter.target.position.x - 2f * attackCircleRadius;
            newPos.y = transform.position.y + 5f;
        }
        portalPrefabTransform.position = newPos;
        
        evilWizard.parameter.portalPrefab.SetActive(true);
        portalPrefabAnimator.Play("Spell-NoEffect");
        yield return new WaitForSeconds(0.5f);

        // teleport wizard
        newPos.y -= 5f;
        

        transform.position = newPos;

        // disable portalPrefab
        evilWizard.parameter.portalPrefab.SetActive(false);
        evilWizard.parameter.movementManager.FlipTo(evilWizard.parameter.target);

        // execute attack
        evilWizard.parameter.animator.Play("Attack");

        // make sure to enter Attack animation
        while(!evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
            yield return null;
        }
        info = evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0);
        // make sure animation is played thoroughly
        while(true){
            info = evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime >= 0.99f){
                break;
            }
            yield return null;
        }
        isInCoroutine = false;
        isHeavyAttacking = false;
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
        isInCoroutine = true;
        canSummonWolf = false;
        
        /*
            there will be a spell above the wolf before wolf respawns

            wolf rises from death and then start striking

            wolf is respawned to enter mode 2
        */
        
        // cache portalPrefab
        Transform portalPrefabTransform = evilWizard.parameter.portalPrefab.transform;
        Animator portalPrefabAnimator = evilWizard.parameter.portalPrefab.GetComponent<Animator>();

        // cache darkWolf
        GameObject dwContainerObj = Instantiate(evilWizard.parameter.darkWolfContainerPrefab);

        Transform dwContainerTransform = dwContainerObj.transform;

        Transform dwTransform = dwContainerTransform.Find("DarkWolf");

        GameObject darkWolf = dwTransform.gameObject;

        // cache animator and scripts
        Animator darkWolfAnimator = darkWolf.GetComponent<Animator>();

        DarkWolf darkWolfScript = darkWolf.GetComponent<DarkWolf>();

        EnemyHealthManager dwHealthManagerScript = darkWolf.GetComponent<EnemyHealthManager>();

        
        // assign fields with what has cached
        currentDarkWolf = darkWolf;

        currentDarkWolfScript = darkWolfScript;

        curDarkWolfHealthManager = dwHealthManagerScript;
        
        // cast a spell 
        evilWizard.parameter.animator.Play("Cast");

        // determine portalPrefab and wolf's spawn point(next to the player)
        float spawnPoint = evilWizard.parameter.target.position.x;

        Vector3 randomSpawnPoint = new Vector3(spawnPoint, evilWizard.parameter.target.position.y + 4f, evilWizard.parameter.target.position.z);

        // make sure to enter cast animation
        while(!evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Cast")){
            yield return null;
        }

        AnimatorStateInfo info = evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0);
        // make sure animation is played thoroughly
        while(true){
            info = evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime >= 0.99f){
                break;
            }
            yield return null;
        }
        
        // summon portalPrefab
        Vector3 newportalPrefabScale = new Vector3(2f, 2f, 1f);
        portalPrefabTransform.localScale = newportalPrefabScale;
        portalPrefabTransform.position = randomSpawnPoint;
        evilWizard.parameter.portalPrefab.SetActive(true);
        portalPrefabAnimator.Play("Spell-NoEffect");

        yield return new WaitForSeconds(0.2f);
        // wizard can recover its action from now
        isInCoroutine = false;

        // summon wolf

        dwTransform.position = randomSpawnPoint;
        dwContainerObj.SetActive(true);

        wolfIsAlive = true;
        
        // set boss mode health bar (health bar) to false. 
        
        Transform healthBar1 = dwContainerObj.transform.Find("HealthBar");

        GameObject healthBarObj = healthBar1.gameObject;

        healthBarObj.SetActive(false);

        // set second boss health bar to true(health bar 2)
        Transform healthBar2 = dwContainerObj.transform.Find("HealthBar2");

        healthBarObj2 = healthBar2.gameObject;

        healthBarObj2.SetActive(true);

        // disable portalPrefab
        evilWizard.parameter.portalPrefab.SetActive(false);

        // return a frame to enter berserk mode
        yield return null; 
        // set wolf's mode to mode 2 and start its FSM from mode 2

        darkWolfScript.parameter.dwCombatManager.curMode = 2;
        darkWolfScript.TransitionState(DarkWolfStateType.Berserk);
        this.curDarkWolfHealthManager.isDead = false;
        // nerf the max health to a fair value. 
        this.curDarkWolfHealthManager.maxHealth = 500f;
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
        base.damage = EW2CombatManager.LASER_WALL_DAMAGE;

        isSummoningLaserWall = true;
        isInCoroutine = true;
        canSummonLaserWall = false;
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
                
                GameObject laser = Instantiate(evilWizard.parameter.laserPrefab);
                
                laserPool.Add(laser);
            }   

            // wizard casts
            evilWizard.parameter.animator.Play("Idle", 0, 0);  // leave current state
            yield return null; 
            evilWizard.parameter.animator.Play("Cast", 0, 0);
            
            // make sure to enter cast animation
            while(!evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Cast")){
                yield return null;
            }
            AnimatorStateInfo info = evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0);
            // make sure animation is played thoroughly
            while(true){
                info = evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0);
                if(info.normalizedTime >= 0.99f){
                    break;
                }
                yield return null;
            }
            evilWizard.parameter.animator.Play("Idle");

            // set central laser position
            if(round == 1){
                centralLaserLocation = evilWizard.parameter.target.position;
                centralLaserLocation.y = evilWizard.parameter.target.position.y + 5f;
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
                    pos.x = (i - centralIndex) * laserWallSpacing + firstTimeLocation.x;
                }
                
                else{
                    pos.x = (i - centralIndex) * laserWallSpacing + firstTimeLocation.x + 0.5f * laserWallSpacing;
                }
                
                laserPool[i].transform.position = pos;
            }

            // summon lasers

            for(int i = 0; i < laserNum; i++){
                GameObject laser = laserPool[i];

                Animator laserAnimator = laser.GetComponent<Animator>();
                // set runtime laser data
                HitBoxManager tempHitBox = laser.GetComponent<HitBoxManager>();
                tempHitBox.SetInitiator(ew2HurtBox);
                tempHitBox.SetCombatManager(combatManager);

                laser.SetActive(true);
                laserAnimator.Play("Spell");
                yield return null;
                
                // call a coroutine to destroy lasers after finishing its animation.
                StartCoroutine(DestroyWhenAnimationEnds(laser, "Spell"));
            }
            
            
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);
        isInCoroutine = false;
    }

    
}
