using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class DWCombatManager : EnemyCombatManager
{   
    public bool getHit;

    public bool isDead;

    public int curMode;

    public bool isCharging;

    public bool isGoingBerserk;

    public bool hasHit;

    public bool isInPenalty;

    [SerializeField] private DarkWolf darkWolf;

    public static float NORMAL_DAMAGE = 15f;

    public static float CHARGE_DAMAGE = 30f;

    private float getHitCount;

    private int randomThreshold;

    [SerializeField] private float curInvulnerableTimer;
    private float invulnerableTimer = 5f; // 5 seconds
    
    [SerializeField] private Knight knight;

    public float knockBackDir;

    private HitData data;

    public bool isVulnerable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckWolfHealthStatus();
        UpdateCurInvulnerableTimer();
    }
    private void UpdateCurInvulnerableTimer(){
        if(curInvulnerableTimer >= 0){
            curInvulnerableTimer -= Time.deltaTime;
        }
        
    }

    public void CheckWolfHealthStatus(){
        // check health status
        if(curMode == 1){
            // if in mode 1 and health drops below half then go berserk
            if(darkWolf.parameter.healthManager.curHealth < 0.5f * darkWolf.parameter.healthManager.maxHealth){
                curMode = 2;
                darkWolf.TransitionState(DarkWolfStateType.Berserk);
            }
        }
    }

    public IEnumerator FadeOut(){
        float duration = 1f;
        float elapsed = 0f;

        while(elapsed < duration){
            // Mathf.Lerp(a , b , t): approach b from a based on t. t is a decimal (like a loading bar)
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            Color c = darkWolf.parameter.SR.color;
            c.a = alpha;
            darkWolf.parameter.SR.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public float CalculateDistance(){
        return Mathf.Abs(darkWolf.parameter.target.position.x - transform.position.x);
    }

    public IEnumerator WindUpThenCharge(){
        // prioritize death. !!!!!!!!!!!!!!!!!
        // Debug.Log("Charge");
        isCharging = true;
        // charge-up before charging

        // play the 5th frame of the animation
        float frame = 5f;
        float totalFrame = 60f;
        float normalizedTime = frame / totalFrame;
        darkWolf.parameter.animator.Play("RunWithoutCollider", 0, normalizedTime);

        // freeze time
        darkWolf.parameter.animator.speed = 0f;
        // charge-up for 1.5s
        yield return new WaitForSeconds(1f);

        // commit to direction
        Vector3 playerPos = darkWolf.parameter.target.position;
        Vector3 startPos = transform.position;
        Vector3 direction = playerPos - startPos;
        
    
        int dir = 0;
        if(direction.x > 0){
            // commit knock back direction now
            dir = 1;
            knockBackDir = 1f;
        }
        else{
            dir = -1;
            knockBackDir = -1f;
        }
        


        yield return new WaitForSeconds(0.5f);
        
        // normalise darkWolf.parameter.animator
        darkWolf.parameter.animator.speed = 1f;
        darkWolf.parameter.animator.Play("DarkWolf_2d_Run Animation");
        // start charging
        // stop charging if charged for enough distance
        darkWolf.parameter.dwMovementManager.FlipTo(darkWolf.parameter.target);

        
        
        float distance = Mathf.Abs((playerPos - startPos).x);
    
        damage = DWCombatManager.CHARGE_DAMAGE;
        while(Vector3.Distance(transform.position, startPos) <= distance){
            darkWolf.parameter.rb.linearVelocity = new Vector2(dir * darkWolf.parameter.dwMovementManager.chargeSpeed, darkWolf.parameter.rb.linearVelocity.y);
            
            if(knight.parameter.combatManager.getHit){
                hasHit = true;
            }
            
            yield return null;
        }
        
        darkWolf.parameter.rb.linearVelocity = Vector2.zero;
    
        isCharging = false;
    }

    public IEnumerator GoingBerserk(){
        isGoingBerserk = true;

        float frame = 10f;
        float totalFrame = 60f;
        float normalizedTime = frame / totalFrame;
        darkWolf.parameter.animator.Play("DarkWolf_2d_Damage Animation", 0, normalizedTime);

        darkWolf.parameter.animator.speed = 0f;

        yield return new WaitForSeconds(1f);

        darkWolf.parameter.animator.speed = 1f;
        isGoingBerserk = false;
    }

    public IEnumerator RaisePenalty(){
        isInPenalty = true;

        
        darkWolf.parameter.animator.Play("DarkWolf_2d_Damage Animation");

        yield return new WaitForSeconds(1.5f);

        isInPenalty = false;
    }
    
    void OnEnable(){
        EventManager.OnEnemyDied += Die;

        EventManager.OnHitOccured += GetHit;
    }
    void OnDisable(){
        EventManager.OnEnemyDied -= Die;

        EventManager.OnHitOccured -= GetHit;
    }

    private void Die(GameObject darkWolf){
        // filter
        if(darkWolf != this.darkWolf.gameObject){
            return;
        }

        // transition to death

        this.darkWolf.TransitionState(DarkWolfStateType.Die);

    }

    public override void GetHit(HitData data){
        // does not get hit if dead
        if(darkWolf.parameter.healthManager.isDead){
            return;
        }
        if(data == null){
            return;
        }
        // cache data
        this.data = data;
        
        // filter: only respond if this object gets hit:
        if(data.targetHurtBox != darkWolf.parameter.dwHurtBox){
            return;
        }
        if(isVulnerable){
            //!!!!!!!!!!!!!!!!!!
            // take damage and play animation
            darkWolf.parameter.healthManager.TakeDamage(data.damage);
            darkWolf.parameter.animator.Play("DarkWolf_2d_Damage Animation");
            return;
        }
        // always responds to knock back and take damage    
        darkWolf.parameter.healthManager.TakeDamage(data.damage);
        // recover transparency
        Color c = darkWolf.parameter.SR.color;
        c.a = 1f;
        darkWolf.parameter.SR.color = c;

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
            // if not yet, dark wolf still responds.
            darkWolf.TransitionState(DarkWolfStateType.Hurt);
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
        darkWolf.parameter.animator.Play("DarkWolf_2d_Damage Animation");

        yield return base.AnimationFinishes("DarkWolf_2d_Damage Animation", darkWolf.parameter.animator);

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

        darkWolf.parameter.rb.AddForce(new Vector2(force * dir, 0), ForceMode2D.Impulse);
    }
    private bool HaveSuperArmor(){
        if(isCharging){
            return true;
        }
        if(getHitCount >= randomThreshold){
            return true;
        }
        return false;
    }
}
