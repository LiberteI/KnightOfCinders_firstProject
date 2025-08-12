using UnityEngine;

public class DWIdleState : StateTransitionInterface{
    private DarkWolf manager;
    
    private DarkWolfParameter parameter;

    private float timer;

    public DWIdleState(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        // play animation
        parameter.animator.Play("DarkWolf_2d_Idle Animation");

        timer = 0f;
    }

    public void OnUpdate(){
        // the wolf should idle a moment and then spot the player
        // after this, the wolf should be in the fighting curMode
        timer += Time.deltaTime;
        if(timer >= 3f){
            manager.TransitionState(DarkWolfStateType.Run);
        }
    }

    public void OnExit(){
        
    }
}
public class DWHurtState : StateTransitionInterface{
    private DarkWolf manager;
    
    private DarkWolfParameter parameter;

    

    public DWHurtState(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.dwCombatManager.StartCoroutine(parameter.dwCombatManager.ExecuteGetHit());
        
    }
    public void OnUpdate(){
        if(parameter.dwCombatManager.getHit){
            return;
        }
        manager.TransitionState(DarkWolfStateType.Decide);
    }
    public void OnExit(){
        
    }
}

public class DWDeadState : StateTransitionInterface{
    private DarkWolf manager;

    private DarkWolfParameter parameter;

    public DWDeadState(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        // normalise darkWolf.parameter.animator
        parameter.animator.speed = 1f;
        // stop all coroutines
        parameter.dwCombatManager.StopAllCoroutines();
        parameter.dwMovementManager.DisableVelocity();
        // recover transparency after exiting
        Color c = parameter.SR.color;
        c.a = 1f;
        parameter.SR.color = c;

        parameter.animator.Play("DarkWolf_2d_Death Animation");
       
    }
    public void OnUpdate(){
        parameter.animator.Play("DarkWolf_2d_Death Animation");
    }
    public void OnExit(){

    }
}
public class DWRunState : StateTransitionInterface{
    private DarkWolf manager;

    private DarkWolfParameter parameter;

    public DWRunState(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
    }
    public void OnUpdate(){
        parameter.dwMovementManager.RunTowardsPlayer();
        
        parameter.dwMovementManager.FlipTo(parameter.target);
        if(parameter.dwMovementManager.isMoving){
            parameter.animator.Play("RunWithoutCollider");
        }

        if(parameter.dwCombatManager.PlayerIsInRange()){
            manager.TransitionState(DarkWolfStateType.Attack);
        }
        
    }
    public void OnExit(){

    }
}
public class DWAttackState : StateTransitionInterface{
    private DarkWolf manager;

    private DarkWolfParameter parameter;

    private AnimatorStateInfo info;
    public DWAttackState(DarkWolf manager){
      
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        // recover transparency
        Color c = parameter.SR.color;
        c.a = 1f;
        parameter.SR.color = c;

        parameter.dwCombatManager.damage = DWCombatManager.NORMAL_DAMAGE;
        parameter.animator.Play("DarkWolf_2d_Attack Animation");
    }
    public void OnUpdate(){
        
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if(info.normalizedTime >= .98){
            manager.TransitionState(DarkWolfStateType.Decide);
        }
    }
    public void OnExit(){

    }
}
public class DWWalkState : StateTransitionInterface{
    private DarkWolf manager;

    private DarkWolfParameter parameter;

    private float timer = 0f;

    public DWWalkState(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        // recover transparency
        Color c = parameter.SR.color;
        c.a = 1f;
        parameter.SR.color = c;
        
        parameter.animator.Play("DarkWolf_2d_Walk Animation");
        if(!parameter.dwCombatManager.PlayerIsInRange()){
            // prevent fade out jittering
            parameter.dwCombatManager.StartCoroutine(parameter.dwCombatManager.FadeOut());
        }
        
        
    }
    public void OnUpdate(){
        
        timer += Time.deltaTime;
        parameter.dwMovementManager.FlipTo(parameter.target);
        parameter.dwMovementManager.WalkTowardsPlayer();
        
        if(timer <= 2f && parameter.dwCombatManager.PlayerIsInRange()){
            manager.TransitionState(DarkWolfStateType.Attack);
        }
        else if(timer > 2f){ // teleport to the face of the player
            // if player is facing right
            if(parameter.target.localScale.x > 0){
                Vector3 newPos = manager.transform.position;
                newPos.x = parameter.target.position.x + 2.5f;
                manager.transform.position = newPos;
            }
            else{ // if player is facing left
                // do not modify x directly or prompting an error.
                Vector3 newPos = manager.transform.position;
                newPos.x = parameter.target.position.x - 2.5f;
                manager.transform.position = newPos;
            }
            manager.TransitionState(DarkWolfStateType.Attack);
        }
        
    }
    public void OnExit(){
        // recover transparency after exiting
        Color c = parameter.SR.color;
        c.a = 1f;
        parameter.SR.color = c;

        // make sure to face player.
        parameter.dwMovementManager.FlipTo(parameter.target);

        //reset timer
        timer = 0;
    }
}

public class DWDecideState : StateTransitionInterface{
    private DarkWolf manager;

    private DarkWolfParameter parameter;

    private float timer;

    private int oneOrTwo;

    public DWDecideState(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){

        // recover transparency
        Color c = parameter.SR.color;
        c.a = 1f;
        parameter.SR.color = c;

        parameter.rb.linearVelocity = new Vector2(0, parameter.rb.linearVelocity.y);
        parameter.animator.Play("DarkWolf_2d_Idle Animation");
        timer = 0f;
        oneOrTwo = Random.Range(0 , 2); //[0, 2)
    }
    public void OnUpdate(){
        
        timer += Time.deltaTime;
        if(parameter.dwCombatManager.curMode == 1){
            if(timer > 1f){
                if(oneOrTwo == 1){
                    // Attack Type 1: 
                    manager.TransitionState(DarkWolfStateType.Run);
                }
                else{
                    // Attack type 2: 
                    manager.TransitionState(DarkWolfStateType.Walk);
                }
            }
        }
        else{
            if(timer > 0.5f){
                // attack becomes faster
                if(oneOrTwo == 1){
                    // Attack type 1:
                    manager.TransitionState(DarkWolfStateType.Run);
                }
                else{
                    // Attack type Charge:
                    manager.TransitionState(DarkWolfStateType.Charge);
                }
            }
        }
    }
    public void OnExit(){
    
    }
}
public class DWBerserkState : StateTransitionInterface{
    // this state is the start point of entering curMode 2
    private DarkWolf manager;

    private DarkWolfParameter parameter;

    public DWBerserkState(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        // disable linear velocity
        parameter.rb.linearVelocity = Vector2.zero;
        parameter.dwCombatManager.StartCoroutine(parameter.dwCombatManager.GoingBerserk());
        // shake body and its eyes turn red.
    }
    
    public void OnUpdate(){
        if(parameter.dwCombatManager.isGoingBerserk){
            return;
        }
        manager.TransitionState(DarkWolfStateType.Decide);
    }

    public void OnExit(){
    }
}

public class DWChargeState : StateTransitionInterface{
    // winds up for a moment and rush to player causing impact damage on contact
    private DarkWolf manager;

    private DarkWolfParameter parameter;

    public DWChargeState(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        parameter.dwCombatManager.StartCoroutine(parameter.dwCombatManager.WindUpThenCharge());
    }
    
    public void OnUpdate(){
        if(!parameter.dwCombatManager.isCharging){
            // if not hit, give wolf a punishment
            // getting hit in vulnerable state will give it a heavy damage
            if(parameter.dwCombatManager.hasHit){
                parameter.dwCombatManager.hasHit = false;
                manager.TransitionState(DarkWolfStateType.Decide);
            }
            else{
                manager.TransitionState(DarkWolfStateType.Vulnerable);
            }
        }
    }

    public void OnExit(){
        
    }
}
public class DWVulnerableState : StateTransitionInterface{
    private DarkWolf manager;

    private DarkWolfParameter parameter;

    public DWVulnerableState(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        parameter.dwCombatManager.isVulnerable = true;
        parameter.dwCombatManager.StartCoroutine(parameter.dwCombatManager.RaisePenalty());
        // double the damage
        parameter.knight.parameter.combatManager.damageMultiplier = 2f;
        parameter.knight.parameter.combatManager.MultiplyCurDamage();
    }
    public void OnUpdate(){
        
        
        if(parameter.dwCombatManager.isInPenalty){
            return;
        }
        manager.TransitionState(DarkWolfStateType.Decide);
    }
    public void OnExit(){
        parameter.knight.parameter.combatManager.damageMultiplier = 1f;
        parameter.knight.parameter.combatManager.MultiplyCurDamage();
        parameter.dwCombatManager.isVulnerable = false;
    }
}

