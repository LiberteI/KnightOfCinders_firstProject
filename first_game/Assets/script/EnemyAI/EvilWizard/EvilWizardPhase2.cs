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
    
    public Animator animator;

    public Transform target;

    public LayerMask playerLayer;

    public GameObject portalPrefab;

    public GameObject laserPrefab;

    public Rigidbody2D RB;

    public GameObject darkWolfContainerPrefab;

    public Transform wizardPhase1;

    public SpriteRenderer SR;

    [Header("Managers")]

    public EW2CombatManager combatManager;

    public EW2MovementManager movementManager;

    public UIManager uiManager;

    public EnemyHealthManager healthManager;

    [Header("Brain")]

    public Knight knight;

    public WizardPhase2SoundManager soundManager;
}

public class EvilWizardPhase2 : MonoBehaviour
{   
    public EvilWizardPhase2Parameter parameter;

    public StateTransitionInterface currentState;

    private Dictionary<Phase2StatesTypes, StateTransitionInterface> states = new Dictionary<Phase2StatesTypes, StateTransitionInterface>();
    
    void Start()
    {  
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
        currentState.OnUpdate();
    }

    public void TransitionState(Phase2StatesTypes type){
        // Debug.Log($"[EW] Transitioning {currentState} to {type}");
        if(currentState != null){
            currentState.OnExit();
        }

        currentState = states[type];

        currentState.OnEnter();
    }
}
