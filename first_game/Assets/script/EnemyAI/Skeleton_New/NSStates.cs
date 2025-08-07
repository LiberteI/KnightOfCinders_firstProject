using UnityEngine;

public class NSIdleState : StateTransitionInterface
{
    public NewSkeleton manager;

    private NewSkeletonParameter parameter;

    private float timer;

    public NSIdleState(NewSkeleton manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        
        parameter.animator.Play("Idle");
        timer = 0f;
    }

    public void OnUpdate(){
        timer += Time.deltaTime;

        if(timer >= 0.5f){
            // let knock back to function
            // disable velocity
            parameter.movementManager.DisableVelocity();
            manager.TransitionState(NSStateType.Walk);
        }
    }

    public void OnExit(){

    }
}

public class NSWalkState : StateTransitionInterface
{
    private NewSkeleton manager;

    private NewSkeletonParameter parameter;

    public NSWalkState(NewSkeleton manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.animator.Play("Walk");
    }

    public void OnUpdate(){
        if(parameter.nsCombatManager.currentRole != SkeletonRole.Backuper){
            parameter.movementManager.FlipTo(parameter.target);
            parameter.movementManager.WalkTowardsPlayer();
            if(parameter.combatManager.PlayerIsInRange()){
                manager.TransitionState(NSStateType.Attack);
            }
        }
        
    }

    public void OnExit(){

    }
}


public class NSDefendState : StateTransitionInterface
{
    private NewSkeleton manager;

    private NewSkeletonParameter parameter;

    public NSDefendState(NewSkeleton manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.animator.Play("Defend");
    }

    public void OnUpdate(){
        if(parameter.nsCombatManager.currentRole == SkeletonRole.Frontliner){
            manager.TransitionState(NSStateType.Idle);
            return;
        }
        if(parameter.nsCombatManager.currentRole == SkeletonRole.Flanker){
            manager.TransitionState(NSStateType.Sneak);
            return;
        }
        parameter.movementManager.FlipTo(parameter.target);
        if(parameter.combatManager.PlayerIsInRange()){
            manager.TransitionState(NSStateType.Attack);
        }
        
        // set flags here to start transitions.
    }

    public void OnExit(){

    }
}

public class NSHurtState : StateTransitionInterface
{
    private NewSkeleton manager;

    private NewSkeletonParameter parameter;

    public NSHurtState(NewSkeleton manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.nsCombatManager.StartCoroutine(parameter.nsCombatManager.ExecuteGetHit());
    }

    public void OnUpdate(){
        if(parameter.nsCombatManager.getHit){
            return;
        }
        if(parameter.nsCombatManager.currentRole == SkeletonRole.Flanker){
            manager.TransitionState(NSStateType.Sneak);
            return;
        }
        manager.TransitionState(NSStateType.Idle);
    }

    public void OnExit(){

    }
}

public class NSDieState : StateTransitionInterface
{
    private NewSkeleton manager;

    private NewSkeletonParameter parameter;

    public NSDieState(NewSkeleton manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        // set linear velocity to 0
        parameter.movementManager.DisableVelocity();
    }

    public void OnUpdate(){
        // continuously play death animation
        parameter.animator.Play("Die");
    }

    public void OnExit(){

    }
}

public class NSAttackState : StateTransitionInterface
{
    private NewSkeleton manager;

    private NewSkeletonParameter parameter;

    public NSAttackState(NewSkeleton manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        if(parameter.nsCombatManager.currentRole == SkeletonRole.Backuper){
            parameter.nsCombatManager.StartCoroutine(parameter.nsCombatManager.Attack1());
        }
        else if(parameter.nsCombatManager.currentRole == SkeletonRole.Frontliner){
            if(parameter.nsCombatManager.DecideAttack() == NSAttackType.Attack1){
                parameter.nsCombatManager.StartCoroutine(parameter.nsCombatManager.Attack1());
            }
            else if(parameter.nsCombatManager.DecideAttack() == NSAttackType.Attack2){
                parameter.nsCombatManager.StartCoroutine(parameter.nsCombatManager.Attack2());
            }
            else{
                parameter.nsCombatManager.StartCoroutine(parameter.nsCombatManager.Attack3());
            }
        }
        else{
            if(parameter.nsCombatManager.IsInFrontOfKnight()){
                parameter.nsCombatManager.StartCoroutine(parameter.nsCombatManager.Attack2());
            }
            else{
                parameter.nsCombatManager.StartCoroutine(parameter.nsCombatManager.Attack3());
            }
        }
        
        
    }

    public void OnUpdate(){
        if(parameter.nsCombatManager.isAttacking){
            return;
        }
        if(parameter.nsCombatManager.currentRole == SkeletonRole.Backuper){
            manager.TransitionState(NSStateType.Defend);
        }
        else if(parameter.nsCombatManager.currentRole == SkeletonRole.Frontliner){
            manager.TransitionState(NSStateType.Idle);
        }
        else{
            manager.TransitionState(NSStateType.Sneak);
        }
        
    }

    public void OnExit(){

    }

    
}
public class NSSneakState : StateTransitionInterface
{
    private NewSkeleton manager;

    private NewSkeletonParameter parameter;

    public NSSneakState(NewSkeleton manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.animator.Play("Walk");
    }

    public void OnUpdate(){
        if(parameter.nsCombatManager.FlankerShouldAttack()){
            manager.TransitionState(NSStateType.Walk);
            return;
        }
        parameter.nsCombatManager.CalculateDistance();
        parameter.nsCombatManager.DecideMove();
    }

    public void OnExit(){

    }
}

public class NSRetreatState : StateTransitionInterface
{
    private NewSkeleton manager;

    private NewSkeletonParameter parameter;

    public NSRetreatState(NewSkeleton manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        
    }

    public void OnUpdate(){
        
    }

    public void OnExit(){

    }
}

