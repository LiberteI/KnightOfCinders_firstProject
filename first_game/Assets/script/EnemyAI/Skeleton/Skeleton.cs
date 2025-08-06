using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum StateTypes{
    Idle, Patrol, Chase, React, Attack, Hit, Dead
}
[Serializable]
public class Parameter{ 
    public int health = 300;

    public float moveSpeed;

    public float chaseSpeed;

    public float idleTime;

    public Transform[] patrolPoints;

    public Transform[] chasePoints;

    public Animator animator;

    public Transform target; // Player target

    public Transform attackCircleCentre;

    public float attackCircleRadius;

    public LayerMask targetLayer; // Player target

    public bool getHit;

    public PlayerCombat combatControl;

    public Rigidbody2D rb;

    public bool isDead = false;
}


public class Skeleton : MonoBehaviour
{
    public Parameter parameter;

    private IState currentState;

    private Dictionary<StateTypes, IState> states = new Dictionary<StateTypes, IState>();
    void Start()
    {
        states.Add(StateTypes.Idle, new IdleState(this));
        states.Add(StateTypes.Patrol, new PatrolState(this));
        states.Add(StateTypes.Chase, new ChaseState(this));
        states.Add(StateTypes.React, new ReactState(this));
        states.Add(StateTypes.Attack, new AttackState(this));
        states.Add(StateTypes.Hit, new HitState(this));
        states.Add(StateTypes.Dead, new DeadState(this));

        TransitionState(StateTypes.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if(parameter.health <= 0){
            parameter.isDead = true;
        }
        currentState.OnUpdate();
    }

    public void TransitionState(StateTypes type){
        if(currentState != null) currentState.OnExit();
        currentState = states[type];
        currentState.OnEnter();
    }

    public void FlipTo(Transform target){
        if(target != null){
            if(transform.position.x > target.position.x){
                transform.localScale = new Vector3(-8, 8, 8);
            }
            else if(transform.position.x < target.position.x){
                transform.localScale = new Vector3(8, 8, 8);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player")){
            parameter.target = other.transform;
        }
    }

    // private void OnTriggerExit2D(Collider2D other){
    //     if(other.CompareTag("Player")){
    //         parameter.target = null;
    //     }
    // }


    private void OnDrawGizmos(){
        Gizmos.DrawWireSphere(parameter.attackCircleCentre.position,parameter.attackCircleRadius);
    }

    public void KnockedBack(){
        float knockbackForce;
        if(parameter.target == null) return;
        if(parameter.combatControl.attackType == "Heavy") knockbackForce = 3f;
        else knockbackForce = 1f;
        float knockBackDirection = (parameter.target.localScale.x >= 0) ? 1 : -1;
        parameter.rb.AddForce(new Vector2(knockBackDirection * knockbackForce, 1f), ForceMode2D.Impulse);
    }

    
}
