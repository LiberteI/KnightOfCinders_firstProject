using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum NSStateType{
    Walk, Hurt, Die, Attack, Defend, Idle, Sneak, Retreat
}

[Serializable]
public class NewSkeletonParameter{
    public Animator animator;

    public Transform target;

    public EnemyMovementManager movementManager;

    public EnemyCombatManager combatManager;

    public NSCombatManager nsCombatManager;

    public GameObject nsHurtBox;

    public EnemyHealthManager healthManager;

    public Rigidbody2D RB;

    public UIManager UImanager;
}
public class NewSkeleton : MonoBehaviour
{
    public NewSkeletonParameter parameter;

    public StateTransitionInterface currentState;

    public Dictionary<NSStateType, StateTransitionInterface> states = new Dictionary<NSStateType, StateTransitionInterface>();

    void Start()
    {
        states.Add(NSStateType.Walk, new NSWalkState(this));
        states.Add(NSStateType.Hurt, new NSHurtState(this));
        states.Add(NSStateType.Die, new NSDieState(this));
        states.Add(NSStateType.Attack, new NSAttackState(this));
       
        states.Add(NSStateType.Defend, new NSDefendState(this));
        states.Add(NSStateType.Idle, new NSIdleState(this));

        states.Add(NSStateType.Sneak, new NSSneakState(this));
        states.Add(NSStateType.Retreat, new NSRetreatState(this));
        TransitionState(NSStateType.Defend);
        
    }

    // Update is called once per frame
    void Update()
    {
        currentState.OnUpdate();
    }
    public void TransitionState(NSStateType type){
        if(currentState != null){
            currentState.OnExit();
        }

        currentState = states[type];

        currentState.OnEnter();
    }

    
}
