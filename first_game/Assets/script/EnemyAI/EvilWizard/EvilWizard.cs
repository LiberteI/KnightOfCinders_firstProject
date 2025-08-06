using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public enum EvilWizardStateTypes{
    Run, AttackMode1_1, AttackMode1_2, DeathMode1, IdleMode1,
    Decide, Vulnerable
}

[Serializable]
public class EvilWizardParameter{
    public Animator animator;

    public PlayerCombat combatManager;

    public Rigidbody2D RB;

    public int attackPhase;

    public float distance;

    public Transform self;

    public Transform target;

    public int health = 1000;

    public bool isAttacking;

    public float attackCircleRadius;

    public Transform attackCircleCentre;

    public float runSpeed;

    public bool getHit;

    public LayerMask playerLayer;

    public GameObject laser;

    public bool isInPenalty;

    public bool isDead = false;

    public GameObject Phase2Wizard;
}
/*
                            //Design of Evil Wizard ://
    The Wizard mode 1 can Run,
                          Attack Upwards (Attack Mode 1_1),
                          Attack Downwards (Attack Mode 1_2),
                          Idle(IdleMode1)
                          Die (DeathMode1).

    Since the states are not so versatile, the difficulty of the mode 1 boss
    will be relatively low. So:
    mode 1 is "simple", treat it as a teaching phase:
        *Teach players to dodge or time hits

        *Introduce safe attack windows

        *Maybe hint at laser danger in a non-threatening way (e.g., wizard charging staff but not casting yet)

    Reminder, there is no hurt for mode 1 so super armor for mode 1.
    but when being attacked there should be a circle of light around the wizard.

    After the health drops below half, the Wizard mode 1 will Die(DeathMode1), and then
    the soul-like item will withdraw from its dead body, which players will know that 
    that is the mode 2 boss.(The bringer of the death).

    //Make Mode 2 Feel Like a Power Spike

    Mode 2 boss can: Attack(with its sickle),
                     Cast(a top down laser / to summon DarkWolf), 
                     Walk
                     Hurt
                     Die(DeathMode2)
                     Idle(IdleMode2)
    
    ideas: Mode 2 boss is more versatile:
        The boss can perform normal attacks on the player using its sickle,
        and sometimes (when the boss can rise up and cast rhythmed top-down lasers ,
        and the player has to escape from those.) the laser could fill the scene but give player prelude
        to react.

        After the cast for lasers, the boss will be vulnerable for being attacked, when player's damage could double.

        Sometimes, the boss could summon a dark wolf as the other enemy. but there should be and only be 1 wolf in the scene at once.
        The wolf is in its mode 2, namely, can only dash and run to attack.
*/

public class EvilWizard : MonoBehaviour
{   
    public EvilWizardParameter parameter;

    public StateTransitionInterface currentState;

    private Dictionary<EvilWizardStateTypes, StateTransitionInterface> states = new Dictionary<EvilWizardStateTypes, StateTransitionInterface>();

    public static event System.Action<EvilWizard> OnWizardPhaseOneDied;
    
    void Start()
    {   
        states.Add(EvilWizardStateTypes.Run, new EWRunState(this));
        states.Add(EvilWizardStateTypes.AttackMode1_1, new EWAttackMode1_1State(this));
        states.Add(EvilWizardStateTypes.AttackMode1_2, new EWAttackMode1_2State(this));
        states.Add(EvilWizardStateTypes.DeathMode1, new EWDeathMode1State(this));
        states.Add(EvilWizardStateTypes.IdleMode1, new EWIdleMode1State(this));
        states.Add(EvilWizardStateTypes.Decide, new EWDecideState(this));
        states.Add(EvilWizardStateTypes.Vulnerable, new EWVulnerableState(this));
        TransitionState(EvilWizardStateTypes.IdleMode1);
    }

    
    void Update()
    {   
        if(parameter.isDead){
            return;
        }
        if(parameter.health <= 0){
            parameter.isDead = true;
            TransitionState(EvilWizardStateTypes.DeathMode1);
            return;
        }
        currentState.OnUpdate();
    }

    public void TransitionState(EvilWizardStateTypes type){
        Debug.Log($"[Evil Wizard] Transitioning from {currentState?.GetType().Name} to {type}");
        if(currentState != null){
            currentState.OnExit();
        }

        currentState = states[type];

        currentState.OnEnter();
    }

    public void CalculateDistance(){
        Vector2 playerPos = parameter.target.position;
        Vector2 selfPos = parameter.self.position;

        parameter.distance = Vector2.Distance(playerPos, selfPos);
    }

    public void PhaseOneDie(){
        if(OnWizardPhaseOneDied != null){
            OnWizardPhaseOneDied.Invoke(this);
        }
    }

    
    public bool PlayerIsInRange(){
        return Physics2D.OverlapCircle(parameter.attackCircleCentre.position, parameter.attackCircleRadius, parameter.playerLayer);
    }
    
    public void OnDrawGizmos(){
        Gizmos.DrawWireSphere(parameter.attackCircleCentre.position, parameter.attackCircleRadius);
    }

    public void FlipTo(Transform target){
        if(target != null){
            if(transform.position.x > target.position.x){
                transform.localScale = new Vector3(-6, 6, 6);
            }
            else if(transform.position.x < target.position.x){
                transform.localScale = new Vector3(6, 6, 6);
            }
        }
    }

    public void RunTowardsPlayer(){
        if(parameter.target == null){
            return;
        }

        float direction = parameter.target.position.x - transform.position.x;

        if(direction > 0){
            parameter.RB.linearVelocity = new Vector2(1 * parameter.runSpeed, parameter.RB.linearVelocity.y);
        }
        else{
            parameter.RB.linearVelocity = new Vector2(-1 * parameter.runSpeed, parameter.RB.linearVelocity.y);
        }
    }
    
    public IEnumerator SummonALaser(){
        parameter.isAttacking = true;

        parameter.laser.SetActive(true);

        // Cache components 

        Transform laserTransform = parameter.laser.transform;
        Animator laserAnimator = parameter.laser.GetComponent<Animator>();

        Vector3 playerPos = parameter.target.position;

        float x = playerPos.x;

        float y = playerPos.y + 2.7f;

        float z = playerPos.z;

        laserTransform.position = new Vector3(x, y, z);
        
        laserAnimator.Play("Spell");


        
        while(true){
            AnimatorStateInfo info = laserAnimator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime >= 0.98f){
                parameter.laser.SetActive(false);
                
                break;
            }
            
            yield return null;
        }

        parameter.isAttacking = false;
    }

    public IEnumerator StutterRunAndAttack(){
        parameter.isAttacking = true;

        parameter.animator.Play("Attack2");
        // Wait for animation to start playing
        while (!parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2")) {
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
        float leftTeleportRange = parameter.target.position.x - 20f;

        float rightTeleportRange = parameter.target.position.x + 20f;

        Vector3 newPos = parameter.self.position;

        float randomPoint = UnityEngine.Random.Range(leftTeleportRange, rightTeleportRange);

        int randomTimes = UnityEngine.Random.Range(2 , 6);  // 2,3,4,5

        int hasTeleportedTimes = 0;

        while(hasTeleportedTimes < randomTimes){
            randomPoint = UnityEngine.Random.Range(leftTeleportRange, rightTeleportRange);

            newPos.x = randomPoint;

            parameter.self.position = newPos;

            FlipTo(parameter.target);

            yield return new WaitForSeconds(0.5f); // teleport every 0.3 second

            hasTeleportedTimes ++;
        }

        int randomOneOrTwo = UnityEngine.Random.Range(1, 3); // 1, 2

        // make sure to be exactly behind of in front of player before striking
        if(randomOneOrTwo == 1){ 
            newPos.x = parameter.target.position.x + 5f;
        }
        else{
            newPos.x = parameter.target.position.x - 5f;
        }
        
        parameter.self.position = newPos;
        FlipTo(parameter.target);

        yield return new WaitForSeconds(0.6f);
        parameter.isAttacking = false;
    }

    public IEnumerator Penalty(){
        parameter.isInPenalty = true;

        parameter.animator.Play("Idle");

        yield return new WaitForSeconds(1.5f);

        parameter.isInPenalty = false;
    }

    public IEnumerator PlayDeathAndSetOutPhase2(){

        parameter.animator.Play("Death");

        // make sure to enter animation first
        while(!parameter.animator.GetCurrentAnimatorStateInfo(0).IsName("Death")){
            yield return null;
        }

        // play animation thoroughly
        AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        while(true){
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if(info.normalizedTime >= 0.99f){
                break;
            }
            yield return null;
        }

        parameter.Phase2Wizard.SetActive(true);
        
    }
}