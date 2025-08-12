using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public enum EvilWizardStateTypes{
    Run, AttackMode1_1, AttackMode1_2, DeathMode1, IdleMode1,
    Decide, Vulnerable, Hurt
}

[Serializable]
public class EvilWizardParameter{
    public Animator animator;

    public Rigidbody2D RB;

    public Transform target;

    public LayerMask playerLayer;

    public GameObject laserPrefab;

    public GameObject Phase2Wizard;

    public EW1CombatManager combatManager;

    public EW1MovementManager movementManager;

    public UIManager uiManager;

    public EnemyHealthManager healthManager;

    public Knight knight;

    public GameObject ew1HurtBox;
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
    
    void Start()
    {   
        states.Add(EvilWizardStateTypes.Run, new EWRunState(this));
        states.Add(EvilWizardStateTypes.AttackMode1_1, new EWAttackMode1_1State(this));
        states.Add(EvilWizardStateTypes.AttackMode1_2, new EWAttackMode1_2State(this));
        states.Add(EvilWizardStateTypes.DeathMode1, new EWDeathMode1State(this));
        states.Add(EvilWizardStateTypes.IdleMode1, new EWIdleMode1State(this));
        states.Add(EvilWizardStateTypes.Decide, new EWDecideState(this));
        states.Add(EvilWizardStateTypes.Vulnerable, new EWVulnerableState(this));
        states.Add(EvilWizardStateTypes.Hurt, new EWHurtStatePhase1(this));
        TransitionState(EvilWizardStateTypes.IdleMode1);
    }

    
    void Update()
    {   
        
        currentState.OnUpdate();
    }

    public void TransitionState(EvilWizardStateTypes type){
        // Debug.Log($"Transitioning from {currentState} to {type}");
        if(currentState != null){
            currentState.OnExit();
        }

        currentState = states[type];

        currentState.OnEnter();
    }

    

    

    
    
    
}