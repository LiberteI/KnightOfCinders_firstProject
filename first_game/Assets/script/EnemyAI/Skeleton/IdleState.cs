using UnityEngine;

public class IdleState : IState
{
    private Skeleton manager;
    private Parameter parameter;

    private float timer;
 
    public IdleState(Skeleton manager){
        this.manager = manager;
        this.parameter = manager.parameter;

    }
    public void OnEnter(){
        parameter.animator.Play("Skeleton_Idle");
    }
    
    public void OnUpdate(){
        if(parameter.health <= 0){
            parameter.isDead = true;
        }
        timer += Time.deltaTime;

        if(parameter.getHit){
            manager.TransitionState(StateTypes.Hit);
        }

        if(parameter.target != null && 
           parameter.target.position.x >= parameter.chasePoints[0].position.x &&
           parameter.target.position.x <= parameter.chasePoints[1].position.x) {
            manager.TransitionState(StateTypes.React);
        }

        if(timer >= parameter.idleTime){
            manager.TransitionState(StateTypes.Patrol);
        }
    }

    public void OnExit(){
        timer = 0;
    }
}

public class PatrolState : IState
{
    private Skeleton manager;
    private Parameter parameter;

    private int patrolPosition;

    public PatrolState(Skeleton manager){
        this.manager = manager;
        this.parameter = manager.parameter;

    }
    public void OnEnter(){
        parameter.animator.Play("Skeleton_Walk");
    }
    
    public void OnUpdate(){

 
        manager.FlipTo(parameter.patrolPoints[patrolPosition]);

        if(parameter.getHit){
            manager.TransitionState(StateTypes.Hit);
        }
        
        manager.transform.position = Vector2.MoveTowards(
            manager.transform.position,
            parameter.patrolPoints[patrolPosition].position,
            parameter.moveSpeed * Time.deltaTime
            );  
        if(parameter.target != null && 
           parameter.target.position.x >= parameter.chasePoints[0].position.x &&
           parameter.target.position.x <= parameter.chasePoints[1].position.x) {
            manager.TransitionState(StateTypes.React);
        }

        

        if(Vector2.Distance(
            manager.transform.position, 
            parameter.patrolPoints[patrolPosition].position
            ) < 1f){


            manager.TransitionState(StateTypes.Idle);
        }

    }

    public void OnExit(){
        patrolPosition++;

        if(patrolPosition >= parameter.patrolPoints.Length){
            patrolPosition = 0;
        }
    }
}

public class ChaseState : IState
{
    private Skeleton manager;
    private Parameter parameter;

    public ChaseState(Skeleton manager){
        this.manager = manager;
        this.parameter = manager.parameter;

    }
    public void OnEnter(){ 
        parameter.animator.Play("Skeleton_Walk");

    }
    
    public void OnUpdate(){
        manager.FlipTo(parameter.target);

        if(parameter.getHit){
            manager.TransitionState(StateTypes.Hit);
        }

        if(parameter.target){
            manager.transform.position = Vector2.MoveTowards(
                manager.transform.position,
                parameter.target.position,
                parameter.chaseSpeed * Time.deltaTime
            );
        }
        if(parameter.target == null || 
            manager.transform.position.x < parameter.chasePoints[0].position.x ||
            manager.transform.position.x > parameter.chasePoints[1].position.x){

            manager.TransitionState(StateTypes.Idle);
        }
        if(Physics2D.OverlapCircle(parameter.attackCircleCentre.position, parameter.attackCircleRadius, parameter.targetLayer)){
            manager.TransitionState(StateTypes.Attack);
        }
    }

    public void OnExit(){

    }
}


public class AttackState : IState
{
    private Skeleton manager;
    private Parameter parameter;
    private AnimatorStateInfo info;

    public AttackState(Skeleton manager){
        this.manager = manager;
        this.parameter = manager.parameter;

    }
    public void OnEnter(){
        parameter.animator.Play("Skeleton_Attack");
    }
    
    public void OnUpdate(){
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        

        if(parameter.getHit){
            manager.TransitionState(StateTypes.Hit);
        }

        if(info.normalizedTime >= .95){
            manager.TransitionState(StateTypes.Chase);
        }
    }

    public void OnExit(){

    }
}

public class ReactState : IState
{
    private Skeleton manager;
    private Parameter parameter;
    private AnimatorStateInfo info;
   

    public ReactState(Skeleton manager){
        this.manager = manager;
        this.parameter = manager.parameter;

    }
    public void OnEnter(){
        parameter.animator.Play("Skeleton_React");
        
    }
    
    public void OnUpdate(){
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if(parameter.getHit){
            manager.TransitionState(StateTypes.Hit);
        }

        if(info.normalizedTime >= .95){
            manager.TransitionState(StateTypes.Chase);
        }
    }

    public void OnExit(){

    }
}

public class HitState : IState
{
    private Skeleton manager;
    private Parameter parameter;
    
    private AnimatorStateInfo info;

    public HitState(Skeleton manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        manager.KnockedBack();
        parameter.animator.Play("Skeleton_Hit"); // Play animation
        parameter.health -= parameter.combatControl.damage; // Get damage based on combo type
    }
    
    public void OnUpdate(){
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(parameter.getHit){
            manager.TransitionState(StateTypes.Hit);
        }
        
        if(parameter.health > 0 && info.normalizedTime < 1.0f) return;

        

       if(parameter.health <= 0){
            manager.TransitionState(StateTypes.Dead);
       }
       else if(!parameter.getHit){
            parameter.target = GameObject.FindWithTag("Player").transform;

            manager.TransitionState(StateTypes.Chase);
        }
    }

    public void OnExit(){
        parameter.getHit = false;
    }
}

public class DeadState : IState
{
    private Skeleton manager;
    private Parameter parameter;
   
    public DeadState(Skeleton manager){
        this.manager = manager;
        this.parameter = manager.parameter;

    }
    public void OnEnter(){
        parameter.animator.Play("Skeleton_Dead");
    }
    
    public void OnUpdate(){
        
    }

    public void OnExit(){

    }
}

