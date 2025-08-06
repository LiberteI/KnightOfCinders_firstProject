using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum KnightStateTypes{
    Walk, Run, Idle, Hurt, Die, Defend, Jump, RunAttack, Attack1, Attack2, Attack3,
    Roll, HeavyAttack1, HeavyAttack2, ShieldStrike, JumpAttack, Invulnerable
}
[Serializable]
public class KnightParameter{
    public Animator animator;

    public Rigidbody2D RB;

    public MovementManager movementManager;

    public CombatManager combatManager;

    public StaminaManager staminaManager;

    public HealthManager healthManager;

    public GameObject knightHurtBox;
}

public class Knight : MonoBehaviour
{
    public KnightParameter parameter;

    public PlayerStateInterface currentState;

    public Dictionary<KnightStateTypes, PlayerStateInterface> states = new Dictionary<KnightStateTypes, PlayerStateInterface>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        states.Add(KnightStateTypes.Walk , new KWalkState(this));
        states.Add(KnightStateTypes.Run , new KRunState(this));
        states.Add(KnightStateTypes.Idle , new KIdleState(this));
        states.Add(KnightStateTypes.Hurt , new KHurtState(this));
        states.Add(KnightStateTypes.Die , new KDeathState(this));
        states.Add(KnightStateTypes.Defend , new KDefendState(this));
        states.Add(KnightStateTypes.Jump , new KJumpState(this));
        states.Add(KnightStateTypes.RunAttack , new KRunAttackState(this));
        states.Add(KnightStateTypes.Attack1 , new KAttack1State(this));
        states.Add(KnightStateTypes.Attack2 , new KAttack2State(this));
        states.Add(KnightStateTypes.Attack3 , new KAttack3State(this));
        states.Add(KnightStateTypes.Roll , new KRollState(this));
        states.Add(KnightStateTypes.HeavyAttack1, new KHeavyAttack1State(this));
        states.Add(KnightStateTypes.HeavyAttack2, new KHeavyAttack2State(this));
        states.Add(KnightStateTypes.ShieldStrike, new KShieldStrikeState(this));
        states.Add(KnightStateTypes.JumpAttack, new kJumpAttackState(this));
        states.Add(KnightStateTypes.Invulnerable, new KInvulnerableState(this));

        TransitionState(KnightStateTypes.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.OnUpdate();
        currentState.HandleInput();
        
    }
    void FixedUpdate(){
        currentState.OnFixedUpdate();
    }
    
    public void TransitionState(KnightStateTypes type){
        // Debug.Log($"[Knight] Transitioning from {currentState?.GetType().Name} to {type}");
        if(currentState != null){
            currentState.OnExit();
        }

        currentState = states[type];

        currentState.OnEnter();
    }


}
