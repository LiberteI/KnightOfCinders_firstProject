using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EW1CombatManager : EnemyCombatManager
{
    public int attackPhase;

    public float distance;

    public bool getHit;

    public bool isInPenalty;

    public EvilWizard evilWizard;

    private HitData data;

    private float getHitCount;

    private int randomThreshold;

    public bool isCasting;

    public bool isHeavyAttacking;
    [SerializeField] private float curInvulnerableTimer;
    private float invulnerableTimer = 2f; // 5 seconds
    
    [SerializeField] private Knight knight;

    public static float HEAVY_DAMAGE = 30f;

    public static float LIGHT_DAMAGE = 15f;

    public static float LASER_DAMAGE = 20f;

    [SerializeField] private GameObject ShieldIcon; 

    [SerializeField] private GameObject VulnerableIcon;
    private void SetFeedback(){
        if(isHeavyAttacking){
            base.sourceFeedback.stopTime = 0.5f;

            base.sourceFeedback.duration = 0.1f;

            base.sourceFeedback.amplitude = 0.4f;

            base.sourceFeedback.frequency = 0.4f;
        }
        else{
            base.sourceFeedback.stopTime = 0.25f;

            base.sourceFeedback.duration = 0.1f;

            base.sourceFeedback.amplitude = 0.2f;

            base.sourceFeedback.frequency = 0.2f;
        }
    }

    void OnEnable(){
        EventManager.OnEnemyDied += Die;

        EventManager.OnHitOccured += GetHit;
    }
    void OnDisable(){
        EventManager.OnEnemyDied -= Die;

        EventManager.OnHitOccured -= GetHit;
    }

    void Update(){
        UpdateCurInvulnerableTimer();

        SetFeedback();

        UpdateInvulnerableState();

        UpdateVulnerableState();
    }

    private void UpdateInvulnerableState(){
        if(curInvulnerableTimer >= 0){
            // activate shield icon.
            ShieldIcon.SetActive(true);
        }
        else{
            // disable shield icon
            ShieldIcon.SetActive(false);
        }
    }
    private void UpdateVulnerableState(){
        if(isInPenalty){
            VulnerableIcon.SetActive(true);
        }
        else{
            VulnerableIcon.SetActive(false);
        }
    }
    private void UpdateCurInvulnerableTimer(){
        if(curInvulnerableTimer >= 0){
            curInvulnerableTimer -= Time.deltaTime;
        }
        
    }
    public void CalculateDistance(){
        // player position
        Vector2 playerPos = evilWizard.parameter.target.position;
        
        // access tranform directly
        distance = Vector2.Distance(playerPos, transform.position);
    }
    // Update: instead of setting to true, instantiate temperary laser.
    public IEnumerator SummonALaser(){
        base.isAttacking = true;
        isCasting = true;

        // cache player position
        Vector3 playerPos = evilWizard.parameter.target.position;

        playerPos.y += 5f;

        // instantiate laser
        GameObject laser = Instantiate(evilWizard.parameter.laserPrefab, playerPos, Quaternion.identity);
        
        // cache laser animator

        Animator laserAnimator = laser.GetComponent<Animator>();

        // cache HitBox Script
        HitBoxManager laserHitBox = laser.GetComponent<HitBoxManager>();

        laserHitBox.SetInitiator(evilWizard.parameter.ew1HurtBox);

        laserHitBox.SetCombatManager(evilWizard.parameter.knight.parameter.combatManager);

        // activate laser
        laser.SetActive(true);

        // play animation
        laserAnimator.Play("Spell");

        
        
        while(true){
            AnimatorStateInfo info = laserAnimator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime > 0.99f){
                break;
            }
            
            yield return null;
        }
        // destroy laser after being summoned.
        Destroy(laser);
        base.isAttacking = false;
        isCasting = false;
    }

    public IEnumerator StutterRunAndAttack(){
        base.isAttacking = true;
        isHeavyAttacking = true;

        evilWizard.parameter.animator.Play("Cast");

        evilWizard.parameter.soundManager.PlayAngrySound();
        // Wait for animation to start playing
        while (!evilWizard.parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Cast")) {
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
        float leftTeleportRange = evilWizard.parameter.target.position.x - 20f;

        float rightTeleportRange = evilWizard.parameter.target.position.x + 20f;

        Vector3 newPos = transform.position;

        float randomPoint = UnityEngine.Random.Range(leftTeleportRange, rightTeleportRange);

        int randomTimes = UnityEngine.Random.Range(2 , 6);  // 2,3,4,5

        int hasTeleportedTimes = 0;

        while(hasTeleportedTimes < randomTimes){
            randomPoint = UnityEngine.Random.Range(leftTeleportRange, rightTeleportRange);

            newPos.x = randomPoint;

            transform.position = newPos;

            evilWizard.parameter.movementManager.FlipTo(evilWizard.parameter.target);

            yield return new WaitForSeconds(0.5f); // teleport every 0.3 second

            hasTeleportedTimes ++;
        }

        int randomOneOrTwo = UnityEngine.Random.Range(1, 3); // 1, 2

        // make sure to be exactly behind of in front of player before striking
        if(randomOneOrTwo == 1){ 
            newPos.x = evilWizard.parameter.target.position.x + 5f;
        }
        else{
            newPos.x = evilWizard.parameter.target.position.x - 5f;
        }
        
        transform.position = newPos;
        evilWizard.parameter.movementManager.FlipTo(evilWizard.parameter.target);

        // dodge time for player before initialising attack
        yield return new WaitForSeconds(0.6f);
        base.isAttacking = false;
    }

    public IEnumerator Penalty(){
        isInPenalty = true;

        evilWizard.parameter.animator.Play("Hurt");

        yield return new WaitForSeconds(1.5f);

        isInPenalty = false;
    }

    public IEnumerator PlayDeathAndSetOutPhase2(){

        evilWizard.TransitionState(EvilWizardStateTypes.DeathMode1);
        evilWizard.parameter.soundManager.PlayScreamSound();

        yield return new WaitForSeconds(1f);
        evilWizard.parameter.soundManager.PlaySetOutPhase2Sound();
        evilWizard.parameter.Phase2Wizard.SetActive(true);
        
    }

    public override void GetHit(HitData data){
        // do not get hit if dead
        if(evilWizard.parameter.healthManager.isDead){
            return;
        }
        if(data == null){
            return;
        }

        // cache data
        this.data = data;

        // filter:
        if(data.targetHurtBox != evilWizard.parameter.ew1HurtBox){
            return;
        }
        if(isHeavyAttacking || isCasting){
            // only take damage, action is not breakable
            evilWizard.parameter.healthManager.TakeDamage(data.damage);
            return;
        }

        // always respond to knock back
        
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
            randomThreshold = Random.Range(3, 5); // 3-4
        }

        // when attack can be broken
        if(!HaveSuperArmor()){
            evilWizard.parameter.healthManager.TakeDamage(data.damage);
            if(isInPenalty){
                // take double damage
                evilWizard.parameter.healthManager.TakeDamage(data.damage);
            }
            // if not yet, still responds.
            evilWizard.TransitionState(EvilWizardStateTypes.Hurt);
            if(knight.parameter.combatManager.ShouldKnockback()){
                GetKnockedBack(data.knockBackDir, 10f);
            }
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
        
        // to be overidden
        evilWizard.parameter.animator.Play("Idle");

        // play animation
        evilWizard.parameter.animator.Play("Hurt");

        yield return base.AnimationFinishes("Hurt", evilWizard.parameter.animator);

        getHit = false;
    }

    public void Die(GameObject evilWizard){
        // filter:
        if(evilWizard != this.evilWizard.gameObject){
            return;
        }
        this.evilWizard.parameter.ew1HurtBox.tag = "Untagged";
        // disable all coroutines
        StopAllCoroutines();
        ShieldIcon.SetActive(false);
        VulnerableIcon.SetActive(false);
        StartCoroutine(PlayDeathAndSetOutPhase2());
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
}
