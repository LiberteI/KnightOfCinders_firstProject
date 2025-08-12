using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
    refactor to-do list:

    1. encapsulate everything following the SOLID

    2. improve hurt logic and Die logic.
*/
public enum DarkWolfStateType{
    Run, Idle, Die, Attack, Walk, Hurt, Decide, Berserk, Charge, Vulnerable
}
[Serializable]
public class DarkWolfParameter{ // global space: cant be the same as that of Skeleton!
    public Rigidbody2D rb;

    public Animator animator;

    public Transform target;

    public SpriteRenderer SR;

    public EnemyHealthManager healthManager;

    public DWCombatManager dwCombatManager;

    public DWMovementManager dwMovementManager;

    public Knight knight;

    public GameObject dwHurtBox;

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
    public DarkWolfParameter parameter;

    public StateTransitionInterface currentState;

    private Dictionary<DarkWolfStateType, StateTransitionInterface> dwStates = new Dictionary<DarkWolfStateType, StateTransitionInterface>();
    
    void Start()
    {
        dwStates.Add(DarkWolfStateType.Idle, new DWIdleState(this));
        dwStates.Add(DarkWolfStateType.Hurt, new DWHurtState(this));
        dwStates.Add(DarkWolfStateType.Die, new DWDeadState(this));
        dwStates.Add(DarkWolfStateType.Attack, new DWAttackState(this));
        dwStates.Add(DarkWolfStateType.Walk, new DWWalkState(this));
        dwStates.Add(DarkWolfStateType.Run, new DWRunState(this));
        dwStates.Add(DarkWolfStateType.Decide, new DWDecideState(this));
        dwStates.Add(DarkWolfStateType.Berserk, new DWBerserkState(this));
        dwStates.Add(DarkWolfStateType.Charge, new DWChargeState(this));
        dwStates.Add(DarkWolfStateType.Vulnerable, new DWVulnerableState(this));
        parameter.dwCombatManager.curMode = 1;
        parameter.healthManager.curHealth = parameter.healthManager.maxHealth;
        TransitionState(DarkWolfStateType.Idle);
    }

    public void TransitionState(DarkWolfStateType type){
        // Debug.Log($"Transistion from {currentState}, to {type}");
        // Exit the current state before transiting to another
        if(currentState != null){
            currentState.OnExit();
        } 
        // Set current state to expected state from states dict
        currentState = dwStates[type];
        // execute OnEnter
        currentState.OnEnter();
    }

    // Update is called once per frame
    void Update()
    {      
        currentState.OnUpdate();
    }

    
    

    

    
}
