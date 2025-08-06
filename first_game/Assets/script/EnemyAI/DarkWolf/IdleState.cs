using UnityEngine;

public class IdleState1 : StateTransitionInterface{
    private DarkWolf manager;
    
    private Parameter1 parameter;

    private float timer;

    public IdleState1(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        // play animation
        parameter.animator.Play("DarkWolf_2d_Idle Animation");

        timer = 0f;
    }

    public void OnUpdate(){
        if(parameter.isDead){
            manager.TransitionState(StateTypes1.Death);
        }
        // the wolf should idle a moment and then spot the player
        // after this, the wolf should be in the fighting mode
        timer += Time.deltaTime;
        if(parameter.getHit){
            manager.TransitionState(StateTypes1.Damage);
        }

        if(timer >= 3f){
            manager.TransitionState(StateTypes1.Run);
        }
    }

    public void OnExit(){
        
    }
}
public class DamageState1 : StateTransitionInterface{
    private DarkWolf manager;
    
    private Parameter1 parameter;

    private AnimatorStateInfo info;

    public DamageState1(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        
        
    }
    public void OnUpdate(){
        info = parameter.animator.GetCurrentAnimatorStateInfo(0); // make sure animation is thoroughly played

        if(parameter.health > 0 && info.normalizedTime < 1.0f){
            return;
        }

        
        else if(!parameter.getHit){
            manager.TransitionState(StateTypes1.Decide);
        }
    }
    public void OnExit(){
        parameter.getHit = false;
    }
}

public class DeadState1 : StateTransitionInterface{
    private DarkWolf manager;

    private Parameter1 parameter;

    public DeadState1(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        // recover transparency after exiting
        Color c = parameter.SR.color;
        c.a = 1f;
        parameter.SR.color = c;

        parameter.animator.Play("DarkWolf_2d_Death Animation");
       
    }
    public void OnUpdate(){
        
    }
    public void OnExit(){

    }
}
public class RunState1 : StateTransitionInterface{
    private DarkWolf manager;

    private Parameter1 parameter;

    public RunState1(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
    }
    public void OnUpdate(){
        manager.RunTowardsPlayer();
        if(parameter.isDead){
            manager.TransitionState(StateTypes1.Death);
        }
        manager.FlipTo(parameter.target);
        if(parameter.isMoving){
            parameter.animator.Play("RunWithoutCollider");
        }

        if(manager.PlayerIsInRange()){
            manager.TransitionState(StateTypes1.Attack);
        }
        
    }
    public void OnExit(){

    }
}
public class AttackState1 : StateTransitionInterface{
    private DarkWolf manager;

    private Parameter1 parameter;

    private AnimatorStateInfo info;
    public AttackState1(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        parameter.animator.Play("DarkWolf_2d_Attack Animation");
    }
    public void OnUpdate(){
        if(parameter.isDead){
            manager.TransitionState(StateTypes1.Death);
        }
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if(info.normalizedTime >= .98){
            manager.TransitionState(StateTypes1.Decide);
        }
    }
    public void OnExit(){

    }
}
public class WalkState1 : StateTransitionInterface{
    private DarkWolf manager;

    private Parameter1 parameter;

    private float timer = 0f;

    public WalkState1(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        parameter.animator.Play("DarkWolf_2d_Walk Animation");
        manager.StartCoroutine(manager.FadeOut());
        
    }
    public void OnUpdate(){
        if(parameter.mode == 2){
            manager.TransitionState(StateTypes1.Mode2);
        }
        timer += Time.deltaTime;
        manager.FlipTo(parameter.target);
        manager.WalkTowardsPlayer();
        
        if(timer <= 2f && manager.PlayerIsInRange()){
            manager.TransitionState(StateTypes1.Attack);
        }
        else if(timer > 2f){ // teleport to the face of the player
            // if player is facing right
            if(parameter.target.localScale.x > 0){
                Vector3 newPos = parameter.self.position;
                newPos.x = parameter.target.position.x + 2.5f;
                parameter.self.position = newPos;
            }
            else{ // if player is facing left
                // do not modify x directly or prompting an error.
                Vector3 newPos = parameter.self.position;
                newPos.x = parameter.target.position.x - 2.5f;
                parameter.self.position = newPos;
            }
            manager.TransitionState(StateTypes1.Attack);
        }
        if(parameter.isDead){
            manager.TransitionState(StateTypes1.Death);
        }
        
    }
    public void OnExit(){
        // recover transparency after exiting
        Color c = parameter.SR.color;
        c.a = 1f;
        parameter.SR.color = c;

        // make sure to face player.
        manager.FlipTo(parameter.target);

        //reset timer
        timer = 0;
    }
}

public class DecideState1 : StateTransitionInterface{
    private DarkWolf manager;

    private Parameter1 parameter;

    private float timer;

    private int oneOrTwo;

    public DecideState1(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        parameter.rb.linearVelocity = new Vector2(0, parameter.rb.linearVelocity.y);
        parameter.animator.Play("DarkWolf_2d_Idle Animation");
        timer = 0f;
        oneOrTwo = Random.Range(0 , 2); //[0, 2)
    }
    public void OnUpdate(){
        
        
        timer += Time.deltaTime;
        if(parameter.mode == 1){
            if(timer > 1f){
                if(oneOrTwo == 1){
                    // Attack Type 1: 
                    manager.TransitionState(StateTypes1.Run);
                }
                else{
                    // Attack type 2: 
                    manager.TransitionState(StateTypes1.Walk);
                }
            }
        }
        else{
            if(timer > 0.5f){
                // attack becomes faster
                if(oneOrTwo == 1){
                    // Attack type 1:
                    manager.TransitionState(StateTypes1.Run);
                }
                else{
                    // Attack type Charge:
                    manager.TransitionState(StateTypes1.Charge);
                }
            }
        }
    }
    public void OnExit(){
    
    }
}
public class Mode2State1 : StateTransitionInterface{
    // this state is the start point of entering mode 2
    private DarkWolf manager;

    private Parameter1 parameter;

    public Mode2State1(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        manager.StartCoroutine(manager.GoingBerserk());
        // shake body and its eyes turn red.
    }
    
    public void OnUpdate(){
        if(parameter.isGoingBerserk){
            return;
        }
        manager.TransitionState(StateTypes1.Decide);
    }

    public void OnExit(){
    }
}

public class ChargeState1 : StateTransitionInterface{
    // winds up for a moment and rush to player causing impact damage on contact
    private DarkWolf manager;

    private Parameter1 parameter;

    public ChargeState1(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        manager.StartCoroutine(manager.WindUpThenCharge());
    }
    
    public void OnUpdate(){
        if(!parameter.isCharging){
            // if not hit, give wolf a punishment
            // getting hit in vulnerable state will give it a heavy damage
            if(parameter.hasHit){
                parameter.hasHit = false;
                manager.TransitionState(StateTypes1.Decide);
            }
            else{
                manager.TransitionState(StateTypes1.Vulnerable);
            }
        }
    }

    public void OnExit(){
        
    }
}
public class VulnerableState1 : StateTransitionInterface{
    private DarkWolf manager;

    private Parameter1 parameter;

    public VulnerableState1(DarkWolf manager){
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        manager.StartCoroutine(manager.Punishment());
    }
    public void OnUpdate(){
        if(parameter.isInPunishment){
            parameter.combatControl.damage = 2 * parameter.combatControl.damage;
            return;
        }
        parameter.combatControl.damage = parameter.combatControl.damage / 2;
        manager.TransitionState(StateTypes1.Decide);
    }
    public void OnExit(){

    }
}

