using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum StateTypes1{
    Run, Idle, Death, Attack, Walk, Damage, Decide, Mode2, Charge, Vulnerable
}
[Serializable]
public class Parameter1{ // global space: cant be the same as that of Skeleton!
    public Rigidbody2D rb;

    public Animator animator;

    public LayerMask playerLayer;

    public Transform target;

    public int health;

    public int maxHealth;

    public float walkSpeed;

    public float runSpeed;

    public float chargeSpeed;

    public bool getHit;

    public bool isDead;

    public PlayerCombat combatControl;

    public Transform attackCircleCentre;
 
    public float attackCircleRadius;

    public bool isMoving;

    public SpriteRenderer SR;

    public Transform self;

    public int mode;

    public bool isCharging;

    public bool isGoingBerserk;

    public bool hasHit;

    public bool isInPunishment;

}
// the dark wolf is a small boss which has 2 modes. 
// in mode 1, when the wolf is healthy. the wolf has 2 attack types: 
//          1. go transparent and attack player. teleport to player if the player is too far.
//          2. charge to player and attack
// after the health of the wolf falls lower than half, the wolf will go berserk. the attack types will turn violent:
//          1. charge to player remains the same.
//          2. the wolf will charge and hit the player with its body
public class DarkWolf : MonoBehaviour
{
    public Parameter1 parameter;

    public StateTransitionInterface currentState;

    private Dictionary<StateTypes1, StateTransitionInterface> states1 = new Dictionary<StateTypes1, StateTransitionInterface>();
    
    void Start()
    {
        states1.Add(StateTypes1.Idle, new IdleState1(this));
        states1.Add(StateTypes1.Damage, new DamageState1(this));
        states1.Add(StateTypes1.Death, new DeadState1(this));
        states1.Add(StateTypes1.Attack, new AttackState1(this));
        states1.Add(StateTypes1.Walk, new WalkState1(this));
        states1.Add(StateTypes1.Run, new RunState1(this));
        states1.Add(StateTypes1.Decide, new DecideState1(this));
        states1.Add(StateTypes1.Mode2, new Mode2State1(this));
        states1.Add(StateTypes1.Charge, new ChargeState1(this));
        states1.Add(StateTypes1.Vulnerable, new VulnerableState1(this));
        parameter.mode = 1;
        parameter.maxHealth = parameter.health;
        TransitionState(StateTypes1.Idle);
    }

    // Update is called once per frame
    void Update()
    {      
        CheckWolfHealthStatus();
        if(parameter.getHit){
            // recover transparency
            Color c = parameter.SR.color;
            c.a = 1f;
            parameter.SR.color = c;
        }
        if(parameter.isDead){
            return;
        }
        
        if(!parameter.isDead){
            // wolf has died
            if(parameter.health <= 0){
                parameter.isDead = true;
                TransitionState(StateTypes1.Death);
            }
        }
        SetIsMoving();
        currentState.OnUpdate();
        parameter.getHit = false;
    }

    public void CheckWolfHealthStatus(){
        // check health status
        if(parameter.mode == 1){
            // if in mode 1 and health drops below half then go berserk
            if(parameter.health < 0.5f * parameter.maxHealth){
                parameter.mode = 2;
                TransitionState(StateTypes1.Mode2);
            }
        }
    }

    public void TransitionState(StateTypes1 type){
        
        // Exit the current state before transiting to another
        if(currentState != null) currentState.OnExit();
        // Set current state to expected state from states dict
        currentState = states1[type];
        // execute OnEnter
        currentState.OnEnter();
    }

    public void OnDrawGizmos(){ // draw cicle that detects when to attack
        Gizmos.DrawWireSphere(parameter.attackCircleCentre.position, parameter.attackCircleRadius);
    }

    public bool PlayerIsInRange(){
        if(Physics2D.OverlapCircle(parameter.attackCircleCentre.position, parameter.attackCircleRadius, parameter.playerLayer)){
            return true;
        }
        return false;
    }

    public void FlipTo(Transform target){
        if(target != null){
            if(transform.position.x < target.position.x){
                transform.localScale = new Vector3(-6, 6, 6);
            }
            else if(transform.position.x > target.position.x){
                transform.localScale = new Vector3(6, 6, 6);
            }
        }
    }

    public void SetIsMoving(){
        if(parameter.rb.linearVelocity.sqrMagnitude > 0.01f){
            parameter.isMoving = true;
        }
        else{
            parameter.isMoving = false;
        }
    }

    public void RunTowardsPlayer(){
        if(parameter.target == null){
            return;
        }

        float direction = parameter.target.position.x - transform.position.x;

        if(direction > 0){
            parameter.rb.linearVelocity = new Vector2(1 * parameter.runSpeed, parameter.rb.linearVelocity.y);
        }
        else{
            parameter.rb.linearVelocity = new Vector2(-1 * parameter.runSpeed, parameter.rb.linearVelocity.y);
        }
    }

    public void WalkTowardsPlayer(){
        if(parameter.target == null){
            return;
        }

        float direction = parameter.target.position.x - transform.position.x;

        if(direction > 0){
            parameter.rb.linearVelocity = new Vector2(1 * parameter.walkSpeed, parameter.rb.linearVelocity.y);
        }
        else{
            parameter.rb.linearVelocity = new Vector2(-1 * parameter.walkSpeed, parameter.rb.linearVelocity.y);
        }
    }

    public IEnumerator FadeOut(){
        float duration = 1f;
        float elapsed = 0f;

        while(elapsed < duration){
            // Mathf.Lerp(a , b , t): approach b from a based on t. t is a decimal (like a loading bar)
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            Color c = parameter.SR.color;
            c.a = alpha;
            parameter.SR.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public float CalculateDistance(){
        return Mathf.Abs(parameter.target.position.x - transform.position.x);
    }

    public IEnumerator WindUpThenCharge(){
        Debug.Log("Charge");
        parameter.isCharging = true;
        // charge-up before charging

        // play the 5th frame of the animation
        float frame = 5f;
        float totalFrame = 60f;
        float normalizedTime = frame / totalFrame;
        parameter.animator.Play("DarkWolf_2d_Run Animation", 0, normalizedTime);

        // freeze time
        parameter.animator.speed = 0f;
        // charge-up for 1.5s
        yield return new WaitForSeconds(1f);

        // commit to direction
        Vector3 playerPos = parameter.target.position;
        Vector3 startPos = parameter.self.position;
        Vector3 direction = playerPos - startPos;
        int dir = 0;
        if(direction.x > 0){
            dir = 1;
        }
        else{
            dir = -1;
        }
        


        yield return new WaitForSeconds(0.5f);

        // normalise animator
        parameter.animator.speed = 1f;

        // start charging
        // stop charging if charged for enough distance
        FlipTo(parameter.target);
        float distance = Mathf.Abs((playerPos - startPos).x);
        
        while(Vector3.Distance(parameter.self.position, startPos) <= distance){
            parameter.rb.linearVelocity = new Vector2(dir * parameter.chargeSpeed, parameter.rb.linearVelocity.y);
            
            if(parameter.combatControl.isHit){
                parameter.hasHit = true;
            }
            // if(combatControl.isHit){}
            yield return null;
        }
        
        parameter.rb.linearVelocity = Vector2.zero;
    
        parameter.isCharging = false;
    }

    public IEnumerator GoingBerserk(){
        parameter.isGoingBerserk = true;

        float frame = 10f;
        float totalFrame = 60f;
        float normalizedTime = frame / totalFrame;
        parameter.animator.Play("DarkWolf_2d_Damage Animation", 0, normalizedTime);

        parameter.animator.speed = 0f;

        yield return new WaitForSeconds(1f);

        parameter.animator.speed = 1f;
        parameter.isGoingBerserk = false;
    }

    public IEnumerator Punishment(){
        parameter.isInPunishment = true;

        float frame = 3f;
        float totalFrame = 60f;
        float normalizedTime = frame / totalFrame;
        parameter.animator.Play("DarkWolf_2d_Damage Animation", 0, normalizedTime);

        parameter.animator.speed = 0f;

        yield return new WaitForSeconds(1.5f);

        parameter.animator.speed = 1f;

        parameter.isInPunishment = false;
    }
}
