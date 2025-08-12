using UnityEngine;
/*

                        PHASE 1 STATES

*/
public class EWRunState : StateTransitionInterface
{
    private EvilWizard manager;

    private EvilWizardParameter parameter;

    public EWRunState(EvilWizard manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){ 
        // in boss's first phase and in attack mode 1 phase 1
        if(parameter.combatManager.attackPhase == 2){
            parameter.animator.Play("Run");
        }
        if(parameter.combatManager.attackPhase == 3){
            // coroutine: start running and stutter-teleport from start point
            //            between 2 teleport points
            //            until it appears in front of the player.
            //            before striking, there should be a wind up time for player to react.
            //            then the wizard disappears and show up from the beginning idling
            parameter.combatManager.StartCoroutine(parameter.combatManager.StutterRunAndAttack());
        }
        

    }
    public void OnUpdate(){
        if(parameter.combatManager.attackPhase == 2){
            parameter.movementManager.FlipTo(parameter.target);
            parameter.movementManager.RunTowardsPlayer();
            if(parameter.combatManager.PlayerIsInRange()){
                manager.TransitionState(EvilWizardStateTypes.AttackMode1_1);
                parameter.RB.linearVelocity = Vector2.zero;
            }
        }
        if(parameter.combatManager.attackPhase == 3){
            if(parameter.combatManager.isAttacking){
                return;
            }
            manager.TransitionState(EvilWizardStateTypes.AttackMode1_2);
        }
        
    }
    public void OnExit(){

    }
}
public class EWAttackMode1_1State : StateTransitionInterface
{
    private EvilWizard manager;

    private EvilWizardParameter parameter;

    private AnimatorStateInfo info;

    public EWAttackMode1_1State(EvilWizard manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        if(parameter.combatManager.attackPhase == 2){
            parameter.animator.Play("Attack2");
        }
        else if(parameter.combatManager.attackPhase == 1){
            // summon some top-down lasers
            parameter.combatManager.StartCoroutine(parameter.combatManager.SummonALaser());

            // cast a spell
            parameter.movementManager.FlipTo(parameter.target);
            parameter.animator.Play("Attack2");
        }
    }
    public void OnUpdate(){
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(info.normalizedTime < 0.99f){
            return;
        }
        if(parameter.combatManager.isCasting){
            return;
        }
        manager.TransitionState(EvilWizardStateTypes.Decide);
        
        
    }
    public void OnExit(){
        
    }
}

public class EWAttackMode1_2State : StateTransitionInterface
{
    private EvilWizard manager;

    private EvilWizardParameter parameter;

    private AnimatorStateInfo info;
    public EWAttackMode1_2State(EvilWizard manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        // transitioning from stutter attack
        parameter.animator.Play("Attack1");
    }
    public void OnUpdate(){
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(info.normalizedTime <= 0.99f){
            return;
        }
        manager.TransitionState(EvilWizardStateTypes.Vulnerable);
    }
    public void OnExit(){
        
    }
}

public class EWDeathMode1State : StateTransitionInterface
{
    private EvilWizard manager;

    private EvilWizardParameter parameter;

    public EWDeathMode1State(EvilWizard manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        // disable velocity
        parameter.movementManager.DisableVelocity();
        parameter.combatManager.StartCoroutine(parameter.combatManager.PlayDeathAndSetOutPhase2());
        
    }
    public void OnUpdate(){
        parameter.animator.Play("Death");
    }
    public void OnExit(){
        
    }
}

public class EWIdleMode1State : StateTransitionInterface
{
    private EvilWizard manager;

    private EvilWizardParameter parameter;

    private float timer;

    public EWIdleMode1State(EvilWizard manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.animator.Play("Idle");
    }
    public void OnUpdate(){
        timer += Time.deltaTime;
        if(timer >= 1.5f){
            manager.TransitionState(EvilWizardStateTypes.Decide);
        }
    }
    public void OnExit(){
        
    }
}

public class EWDecideState : StateTransitionInterface{
    private EvilWizard manager;

    private EvilWizardParameter parameter;

    private int randomOneOrTwoOrThree;

    private float timer = 0;

    public EWDecideState(EvilWizard manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        
        parameter.animator.Play("Idle");
        randomOneOrTwoOrThree = Random.Range(1, 4); // [1, 3)
    }

    public void OnUpdate(){
        timer += Time.deltaTime;
        if(timer <= 0.7f){
            return;
        }
        if(randomOneOrTwoOrThree == 1){
            // stutter-teleport to player and attack
            parameter.combatManager.attackPhase = 3;
            manager.TransitionState(EvilWizardStateTypes.Run);
            parameter.combatManager.damage = EW1CombatManager.HEAVY_DAMAGE;
        }
        else{ 
            parameter.combatManager.CalculateDistance();
            if(parameter.combatManager.distance >= 10f){ // cast a spell and summon laser
                parameter.combatManager.attackPhase = 1;
                parameter.combatManager.damage = EW1CombatManager.LASER_DAMAGE;
                manager.TransitionState(EvilWizardStateTypes.AttackMode1_1);
            }
            else{ // when the player is near: run to player and attack
                parameter.combatManager.attackPhase = 2;

                parameter.combatManager.damage = EW1CombatManager.LIGHT_DAMAGE;
                manager.TransitionState(EvilWizardStateTypes.Run);
            }
        }
    }
    public void OnExit(){
        timer = 0;
    }
}

public class EWVulnerableState : StateTransitionInterface{
    private EvilWizard manager;

    private EvilWizardParameter parameter;

    public EWVulnerableState(EvilWizard manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.Penalty());
    }

    public void OnUpdate(){
        // if coroutine ends:
        if(parameter.combatManager.isInPenalty){
            return;
        }
        manager.TransitionState(EvilWizardStateTypes.Decide);
    }

    public void OnExit(){

    }
}
public class EWHurtStatePhase1 : StateTransitionInterface{
    private EvilWizard manager;

    private EvilWizardParameter parameter;

    public EWHurtStatePhase1(EvilWizard manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.ExecuteGetHit());
    }

    public void OnUpdate(){
        // if coroutine ends:
        if(parameter.combatManager.getHit){
            return;
        }
        manager.TransitionState(EvilWizardStateTypes.Decide);
    }

    public void OnExit(){

    }
}

/*
                        PHASE 2 STATES
*/

/*
    Walk for about 3 seconds if wizard cannot reach the player, then teleport to player, attacking with effect.
    if player was in range, then attack without effect.
*/
public class EWWalkState : StateTransitionInterface
{
    private EvilWizardPhase2 manager;

    private EvilWizardPhase2Parameter parameter;

    private float timer;

    public EWWalkState(EvilWizardPhase2 manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.animator.Play("Walk");
        timer = 0;
    }
    public void OnUpdate(){
        parameter.movementManager.WalkTowardsPlayer();

        parameter.movementManager.FlipTo(parameter.target);

        timer += Time.deltaTime;
        if(timer > 1.5f){
            manager.TransitionState(Phase2StatesTypes.AttackWithEffect);
        }
        if(parameter.combatManager.PlayerIsInRange()){
            manager.TransitionState(Phase2StatesTypes.AttackWithoutEffect);
        }
    }
    public void OnExit(){
        
    }
}
public class EWAttackWithEffectState : StateTransitionInterface
{
    private EvilWizardPhase2 manager;

    private EvilWizardPhase2Parameter parameter;

    public EWAttackWithEffectState(EvilWizardPhase2 manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.TeleportAndAttack());
    }
    public void OnUpdate(){
        if(parameter.combatManager.isInCoroutine){
            return;
        }
        manager.TransitionState(Phase2StatesTypes.Phase2Decide);
    }
    public void OnExit(){
        
    }
}
public class EWAttackWithoutEffectState : StateTransitionInterface
{
    private EvilWizardPhase2 manager;

    private EvilWizardPhase2Parameter parameter;

    private AnimatorStateInfo info;

    public EWAttackWithoutEffectState(EvilWizardPhase2 manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.damage = EW2CombatManager.LIGHT_DAMAGE;
        parameter.animator.Play("Attack-NoEffect");
        parameter.RB.linearVelocity = Vector2.zero;
        
    }
    public void OnUpdate(){
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(info.normalizedTime <= 0.99f){
            return;
        }
        manager.TransitionState(Phase2StatesTypes.Phase2Decide);
    }
    public void OnExit(){
        
    }
}

/*
    when the wizard is healthy: summon a brunch of homing lasers and then transition to decide

    when the wizard is striking along with the wolf: become invincible and summon homing lasers like decribed above,
    but with some time intervals.

    the number of lasers is 7
    they are launched sequentially and within a short of time, homing the player
*/
public class EWHomingLaserState : StateTransitionInterface
{
    private EvilWizardPhase2 manager;

    private EvilWizardPhase2Parameter parameter;

    private float timer;

    public EWHomingLaserState(EvilWizardPhase2 manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        if(parameter.combatManager.wolfIsAlive){
            if(timer >= 4f){
                timer = 0;
                parameter.combatManager.StartCoroutine(parameter.combatManager.SummonLasers());
                parameter.combatManager.canSummonHomingLaser = true;
            }
            else{
                parameter.animator.Play("Idle");
            }
            
        }
        // there is no wolf
        else{
            parameter.RB.linearVelocity = Vector2.zero;
            parameter.combatManager.StartCoroutine(parameter.combatManager.SummonLasers());
        }
        
        // wolf exists and still in cool down
    }
    public void OnUpdate(){
        if(parameter.combatManager.isInCoroutine){
            return;
        }

        // if wolf exists, stay in this state. and there will be seconds of interval between every 2 casts
        if(parameter.combatManager.wolfIsAlive){
            timer += Time.deltaTime;
            manager.TransitionState(Phase2StatesTypes.HomingLaser);
        }
        else{
            manager.TransitionState(Phase2StatesTypes.Phase2Decide);
        }
        
    }
    public void OnExit(){
        
    }
}
public class EWSummonWolfState : StateTransitionInterface
{
    private EvilWizardPhase2 manager;

    private EvilWizardPhase2Parameter parameter;

    public EWSummonWolfState(EvilWizardPhase2 manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.SummonAWolf());
    }
    public void OnUpdate(){
        if(parameter.combatManager.isInCoroutine){
            return;
        }
        manager.TransitionState(Phase2StatesTypes.HomingLaser);
    }
    public void OnExit(){
        
    }
}
public class EWLaserWallState : StateTransitionInterface
{
    private EvilWizardPhase2 manager;

    private EvilWizardPhase2Parameter parameter;

    public EWLaserWallState(EvilWizardPhase2 manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.SummonLaserWall());
    }
    public void OnUpdate(){
        if(parameter.combatManager.isInCoroutine){
            return;
        }
        manager.TransitionState(Phase2StatesTypes.Phase2Vulnerable);
    }
    public void OnExit(){
        
    }
}
public class EWHurtState : StateTransitionInterface
{
    private EvilWizardPhase2 manager;

    private EvilWizardPhase2Parameter parameter;

    public EWHurtState(EvilWizardPhase2 manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.ExecuteGetHit());
    }
    public void OnUpdate(){
        // if coroutine ends:
        if(parameter.combatManager.getHit){
            return;
        }
        manager.TransitionState(Phase2StatesTypes.Phase2Decide);
    }
    public void OnExit(){
        
    }
}
public class EWPhase2VulnerableState : StateTransitionInterface
{
    private EvilWizardPhase2 manager;

    private EvilWizardPhase2Parameter parameter;

    private float timer;

    public EWPhase2VulnerableState(EvilWizardPhase2 manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        timer = 0;
        // play vulnerable animation
        parameter.animator.Play("Hurt-NoEffect");
    }
    public void OnUpdate(){
        timer += Time.deltaTime;
        if(timer <= 3f){
            return;
        }
        manager.TransitionState(Phase2StatesTypes.Phase2Decide);
    }
    public void OnExit(){
        
    }
}
public class Phase2DeathState : StateTransitionInterface
{
    private EvilWizardPhase2 manager;

    private EvilWizardPhase2Parameter parameter;

    public Phase2DeathState(EvilWizardPhase2 manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        // disable velocity
        parameter.movementManager.DisableVelocity();
        parameter.animator.Play("Death");
    }
    public void OnUpdate(){
        parameter.animator.Play("Death");
    }
    public void OnExit(){
        
    }
}

public class EWStartState : StateTransitionInterface
{
    private EvilWizardPhase2 manager;

    private EvilWizardPhase2Parameter parameter;

    public EWStartState(EvilWizardPhase2 manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        // Play death without effect in reverse and then idle a bit
        parameter.combatManager.StartCoroutine(parameter.combatManager.RiseFromDeathAndIdle());
    }
    public void OnUpdate(){
        if(parameter.combatManager.isInCoroutine){
            return;
        }
        manager.TransitionState(Phase2StatesTypes.Phase2Decide);
    }
    public void OnExit(){
        
    }
}

public class EWPhase2DecideState : StateTransitionInterface
{
    private EvilWizardPhase2 manager;

    private EvilWizardPhase2Parameter parameter;

    private float timer;

    private int healthStatus;

    public EWPhase2DecideState(EvilWizardPhase2 manager){
        this.manager = manager;
        this.parameter = manager.parameter;
        
    }
    public void OnEnter(){
        timer = 0;
        parameter.animator.Play("Idle");
    }
    public void OnUpdate(){
        
        healthStatus = parameter.combatManager.checkBossHeathStatus();
        timer += Time.deltaTime;
        if(timer >= 1.5f){

            /*
                when in healthy state:
                homing laser is the priority, and should be executed whenever it can be.
                every time it finishes, start cool down, and during this, hunt the player down
            */
            if(healthStatus == EW2CombatManager.IS_HEALTHY){
                // 2 phases in HEALTHY state
                
                if(parameter.combatManager.canSummonHomingLaser){
                    manager.TransitionState(Phase2StatesTypes.HomingLaser);
                }
                else{
                    manager.TransitionState(Phase2StatesTypes.Walk);
                }
            }   

            /*
                when in IS WOUNDED state:
                the wizard will keep summmon lasers if wolf exists, otherwise, the behaviour keeps the same with that in healthy state
            */
            else if(healthStatus == EW2CombatManager.IS_WOUNDED){
                if(parameter.combatManager.canSummonWolf){
                    manager.TransitionState(Phase2StatesTypes.SummonWolf);
                }
                else{
                    if(parameter.combatManager.canSummonHomingLaser){
                        manager.TransitionState(Phase2StatesTypes.HomingLaser);
                    }
                    else{
                        manager.TransitionState(Phase2StatesTypes.Walk);
                    }
                }
            }
            /*
                when in UNHEALTHY state
            */
            else if(healthStatus == EW2CombatManager.IS_UNHEALTHY){
                if(parameter.combatManager.canSummonLaserWall){
                    manager.TransitionState(Phase2StatesTypes.LaserWall);
                }
                else{
                    if(parameter.combatManager.canSummonHomingLaser){
                        manager.TransitionState(Phase2StatesTypes.HomingLaser);
                    }
                    else{
                        manager.TransitionState(Phase2StatesTypes.Walk);
                    }
                
                }
                
            }
        }

    }
    public void OnExit(){
        
    }
}
