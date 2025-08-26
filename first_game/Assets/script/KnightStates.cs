using UnityEngine;


public class KIdleState : PlayerStateInterface
{
    /*
        this state has 2 idles, after 2 seconds without movement, the knight will sit down.
        and this state is the cross road to other states
    */
    private Knight manager;

    private KnightParameter parameter;

    private float timer;

    private AnimatorStateInfo info;
    public KIdleState(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        timer = 0;
        // hardcoded bonus
        parameter.staminaManager.idleBonus = 3f;
        parameter.healthManager.healthRenerateBonus = 4f;
        parameter.animator.Play("Idle");
    }
    public void OnUpdate(){
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        timer += Time.deltaTime;
        if(timer < 2f){
            return;
        }
        if(info.IsName("Idle2")){
            return;
        }
        parameter.staminaManager.idleBonus = 4f;
        parameter.healthManager.healthRenerateBonus = 6f;
        parameter.animator.Play("Idle2");
    }
    public void OnFixedUpdate(){}
    public void OnExit(){
        parameter.staminaManager.idleBonus = 0;
        parameter.healthManager.healthRenerateBonus = 0f;
    
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
    }

    public void HandleInput(){
        // --------------------- COMBAT ----------------------//
        
        // defend
        if(Input.GetKey("s")){
            manager.TransitionState(KnightStateTypes.Defend);
            return;
        }
        
        // light attack combo
        if(Input.GetKeyDown("j")){
            if(parameter.combatManager.curLightAttack == null || parameter.combatManager.curLightAttack == "Attack3"){
                manager.TransitionState(KnightStateTypes.Attack1);
            }
            else if(parameter.combatManager.curLightAttack == "Attack1"){
                manager.TransitionState(KnightStateTypes.Attack2);
            }
            else if(parameter.combatManager.curLightAttack == "Attack2"){
                manager.TransitionState(KnightStateTypes.Attack3);
            }
        }

        // heavy attack combo
        if(Input.GetKeyDown("u")){
            if(parameter.combatManager.curHeavyAttack == null || parameter.combatManager.curHeavyAttack == "PowerAttack1"){
                if(parameter.staminaManager.DeductStamina(StaminaCostTypes.HeavyAttack, false)){
                    manager.TransitionState(KnightStateTypes.HeavyAttack2);
                }
                
            }
            else if(parameter.combatManager.curHeavyAttack == "PowerAttack2"){
                if(parameter.staminaManager.DeductStamina(StaminaCostTypes.HeavyAttack, false)){
                    manager.TransitionState(KnightStateTypes.HeavyAttack1);
                }
            }
            
            return;
        }
        

        // Shield Strick
        if(Input.GetKeyDown("h")){
            if(parameter.staminaManager.DeductStamina(StaminaCostTypes.ShieldStrike, false)){
                manager.TransitionState(KnightStateTypes.ShieldStrike);
            }
            return;
            
        }
        // ------------------------ movement --------------------- // 
        // Roll
        if(Input.GetKeyDown("l")){
            // check stamina first
            if(parameter.staminaManager.DeductStamina(StaminaCostTypes.Roll, false)){
                manager.TransitionState(KnightStateTypes.Roll);
            }
            return;
        }
        // jump
        if(Input.GetKeyDown("k") && parameter.movementManager.DecideCanJump()){
            manager.TransitionState(KnightStateTypes.Jump);
            return;
        }
        // move
        if(parameter.movementManager.GetHorizontal() != 0){
            if(Input.GetKey(KeyCode.LeftShift) && parameter.staminaManager.DeductStamina(StaminaCostTypes.Run, false) && parameter.staminaManager.curStamina > 5f){
                manager.TransitionState(KnightStateTypes.Run);
                return;
            }
            else{
                manager.TransitionState(KnightStateTypes.Walk);
                return;
            }
        }

    

        
    }

    public void OnGetHit(HitData data){
        
    }
}

public class KRunState : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    public KRunState(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.movementManager.SetIsRunning(true);
        parameter.animator.Play("Run");
        
    }
    public void OnUpdate(){
        
    }
    public void OnFixedUpdate(){
        bool success = parameter.staminaManager.DeductStamina(StaminaCostTypes.Run, false);
        if(!success){
            
            if(parameter.movementManager.IsMoving()){
                manager.TransitionState(KnightStateTypes.Walk);
            }
            else{
                manager.TransitionState(KnightStateTypes.Idle);
            }
            
        }
    }
    public void OnExit(){
        
        
        parameter.combatManager.ResetAllFlags();
    }
    public void HandleInput(){
        // ------------------- COMBAT --------------------//

        // defend while running
        if(Input.GetKey("s")){
            manager.TransitionState(KnightStateTypes.Defend);
            return;
        }

        
        // Run Attack
        if(Input.GetKeyDown("j")){
            if(parameter.staminaManager.DeductStamina(StaminaCostTypes.RunAttack, false)){
                manager.TransitionState(KnightStateTypes.RunAttack);
            }
            return;
        }
        

        
        // heavy attack combo
        if(Input.GetKeyDown("u")){
            if(parameter.combatManager.curHeavyAttack == null || parameter.combatManager.curHeavyAttack == "PowerAttack1"){
                if(parameter.staminaManager.DeductStamina(StaminaCostTypes.HeavyAttack, false)){
                    manager.TransitionState(KnightStateTypes.HeavyAttack2);
                }
                
            }
            else if(parameter.combatManager.curHeavyAttack == "PowerAttack2"){
                if(parameter.staminaManager.DeductStamina(StaminaCostTypes.HeavyAttack, false)){
                    manager.TransitionState(KnightStateTypes.HeavyAttack1);
                }
            }
            
            return;
        }
        // Shield Strick
        if(Input.GetKeyDown("h")){
            if(parameter.staminaManager.DeductStamina(StaminaCostTypes.ShieldStrike, false)){
                manager.TransitionState(KnightStateTypes.ShieldStrike);
            }
            return;
            
        }
    // ----------------------- Movement --------------------- // 
        // roll based on run speed
        if(Input.GetKeyDown("l")){
            // check stamina first
            if(parameter.staminaManager.DeductStamina(StaminaCostTypes.Roll, false)){
                manager.TransitionState(KnightStateTypes.Roll);
            }
            return;
        }
        // jump while running
        if(Input.GetKeyDown("k") && parameter.movementManager.DecideCanJump()){
            manager.TransitionState(KnightStateTypes.Jump);
            return;
        }
        // transition back to idle if no movement occurs
        if(parameter.movementManager.GetHorizontal() == 0){
            
            manager.TransitionState(KnightStateTypes.Idle);
        }
        // transition back to walk if shift is released
        else{
            if(!Input.GetKey(KeyCode.LeftShift)){
                
                manager.TransitionState(KnightStateTypes.Walk);
            }
        }
    }
    public void OnGetHit(HitData data){

    }
}

public class KWalkState : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    public KWalkState(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.movementManager.SetIsRunning(false);
        parameter.staminaManager.walkBonus = 2f;
        parameter.animator.Play("Walk");
    }
    public void OnUpdate(){}
    public void OnFixedUpdate(){}
    public void OnExit(){
        parameter.staminaManager.walkBonus = 0;
    
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
    }
    public void HandleInput(){
        // ------------------- COMBAT --------------------//
        
        // defend while walking
        if(Input.GetKey("s")){
            manager.TransitionState(KnightStateTypes.Defend);
            return;
        }

        // light attack combo
        if(Input.GetKeyDown("j")){
            if(parameter.combatManager.curLightAttack == null || parameter.combatManager.curLightAttack == "Attack3"){
                manager.TransitionState(KnightStateTypes.Attack1);
            }
            else if(parameter.combatManager.curLightAttack == "Attack1"){
                manager.TransitionState(KnightStateTypes.Attack2);
            }
            else if(parameter.combatManager.curLightAttack == "Attack2"){
                manager.TransitionState(KnightStateTypes.Attack3);
            }
        }

        // heavy attack combo
        if(Input.GetKeyDown("u")){
            if(parameter.combatManager.curHeavyAttack == null || parameter.combatManager.curHeavyAttack == "PowerAttack1"){
                if(parameter.staminaManager.DeductStamina(StaminaCostTypes.HeavyAttack, false)){
                    manager.TransitionState(KnightStateTypes.HeavyAttack2);
                }
                
            }
            else if(parameter.combatManager.curHeavyAttack == "PowerAttack2"){
                if(parameter.staminaManager.DeductStamina(StaminaCostTypes.HeavyAttack, false)){
                    manager.TransitionState(KnightStateTypes.HeavyAttack1);
                }
            }
            
            return;
        }

        // Shield Strick
        if(Input.GetKeyDown("h")){
            if(parameter.staminaManager.DeductStamina(StaminaCostTypes.ShieldStrike, false)){
                manager.TransitionState(KnightStateTypes.ShieldStrike);
            }
            return;
            
        }
        // ------------------------------ Movemment --------------------------- // 
        // roll based on walk speed
        if(Input.GetKeyDown("l")){
            // check stamina first
            if(parameter.staminaManager.DeductStamina(StaminaCostTypes.Roll, false)){
                manager.TransitionState(KnightStateTypes.Roll);
            }
            return;
        }
        // jump while walking
        if(Input.GetKeyDown("k") && parameter.movementManager.DecideCanJump()){
            manager.TransitionState(KnightStateTypes.Jump);
            return;
        }
        // transition back to idle if there is no movement
        if(parameter.movementManager.GetHorizontal() == 0){
            manager.TransitionState(KnightStateTypes.Idle);
            return;
        }
        // transition to run if start running
        if(Input.GetKey(KeyCode.LeftShift) && parameter.staminaManager.DeductStamina(StaminaCostTypes.Run, false) && parameter.staminaManager.curStamina > 5f){
                manager.TransitionState(KnightStateTypes.Run);
                return;
            }

        
    
    }
    public void OnGetHit(HitData data){

    }
}
public class KJumpState : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    private float graceTimer;

    public KJumpState(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.movementManager.isJumping = true;
        graceTimer = 0;
        parameter.animator.Play("Jump");
        parameter.movementManager.Jump();
    }
    public void OnUpdate(){
        graceTimer += Time.deltaTime;
        if(graceTimer <= 0.2f){
            return;
        }
        if(parameter.movementManager.IsJumping()){
            return;
        }
        else{
            if(parameter.movementManager.run && parameter.movementManager.horizontal != 0){
                manager.TransitionState(KnightStateTypes.Run);
            }
            else if(!parameter.movementManager.run && parameter.movementManager.horizontal != 0){
                manager.TransitionState(KnightStateTypes.Walk);
            }
            else{
                manager.TransitionState(KnightStateTypes.Idle);
            }
        }
    }
    public void OnFixedUpdate(){}
    public void OnExit(){
        
        
        
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
    }
    public void HandleInput(){
        if(Input.GetKeyDown("j")){
            manager.TransitionState(KnightStateTypes.JumpAttack);
        }
    }
    public void OnGetHit(HitData data){

    }
}

public class KRollState : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    public KRollState(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.movementManager.StartCoroutine(parameter.movementManager.Roll());
    }
    public void OnUpdate(){
        if(parameter.movementManager.isRolling){
            return;
        }
        manager.TransitionState(KnightStateTypes.Idle);
    }
    public void OnFixedUpdate(){}
    public void OnExit(){
            
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
        
    }
    public void HandleInput(){
        
    }
    public void OnGetHit(HitData data){

    }
}


public class KHurtState : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    private HitData incomingHitData;

    public KHurtState(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;

        
    }
    public void OnEnter(){
        // disable linear velocity
        parameter.RB.linearVelocity = Vector2.zero;

        // play hurt sound
        parameter.playerSoundManager.PlayHurtSound();
    }
    public void OnUpdate(){
        // Debug.Log("in hurt state");
        if(parameter.combatManager.getHit){
            return;
        }
        manager.TransitionState(KnightStateTypes.Idle);
    }
    public void OnFixedUpdate(){}
    public void OnExit(){      
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
    }
    public void HandleInput(){}
    public void OnGetHit(HitData data){
        incomingHitData = data;
        parameter.combatManager.StartCoroutine(parameter.combatManager.ExecuteGetHit(incomingHitData));
       
    }

}
public class KDeathState : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    private AnimatorStateInfo info;

    public KDeathState(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        // play death animation
        parameter.animator.Play("Dead");
    }
    public void OnUpdate(){
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if(info.normalizedTime < 0.99f){
            return;
        }

        EventManager.RaiseDefeat();
    }
    public void OnFixedUpdate(){}
    public void OnExit(){      
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
    }
    public void HandleInput(){}
    public void OnGetHit(HitData data){

    }
}


public class KDefendState : PlayerStateInterface
{
    /*
        1. keep defending if key is held. 
        2. Drain stamina if get hit while blocking
        3. disable horizontal speed
        4. take damage and break defence if get hit from the back.
    */
    private Knight manager;

    private KnightParameter parameter;

    public KDefendState(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.animator.Play("Defend");
        // disable horizontal movement
        parameter.RB.linearVelocity = Vector2.zero;
    }
    public void OnUpdate(){
        
        // set flag
        parameter.combatManager.isDefending = true;
    }
    public void OnFixedUpdate(){}
    public void OnExit(){
        
            
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
        
    }
    public void HandleInput(){
        if(!Input.GetKey("s")){
            manager.TransitionState(KnightStateTypes.Idle);
        }
    }
    public void OnGetHit(HitData data){

    }
}
public class KAttack1State : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    public KAttack1State(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.LightAttack("Attack1"));
        
    }
    public void OnUpdate(){
        if(parameter.combatManager.isLightAttacking){
            return;
        }
        manager.TransitionState(KnightStateTypes.Idle);
    }
    public void OnFixedUpdate(){}
    public void OnExit(){
            
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
        
    }
    public void HandleInput(){}
    public void OnGetHit(HitData data){

    }
}
public class KAttack2State : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    public KAttack2State(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.LightAttack("Attack2"));
    }
    public void OnUpdate(){
        if(parameter.combatManager.isLightAttacking){
            return;
        }
        manager.TransitionState(KnightStateTypes.Idle);
    }
    public void OnFixedUpdate(){}
    public void OnExit(){      
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
        
    }
    public void HandleInput(){}
    public void OnGetHit(HitData data){

    }
}
public class KAttack3State : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    public KAttack3State(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.LightAttack("Attack3"));
    }
    public void OnUpdate(){
        if(parameter.combatManager.isLightAttacking){
            return;
        }
        manager.TransitionState(KnightStateTypes.Idle);
    }
    public void OnFixedUpdate(){}
    public void OnExit(){
            
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
    }
    public void HandleInput(){}
    public void OnGetHit(HitData data){

    }
}
public class KRunAttackState : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    public KRunAttackState(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.RunAttack());
    }
    public void OnUpdate(){
        if(parameter.combatManager.isRunAttacking){
            return;
        }
        manager.TransitionState(KnightStateTypes.Idle);
    }
    public void OnFixedUpdate(){}
    public void OnExit(){
            
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
    }
    public void HandleInput(){}
    public void OnGetHit(HitData data){

    }
}
public class KHeavyAttack1State : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    public KHeavyAttack1State(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.PowerAttack("PowerAttack1"));
        
    }
    public void OnUpdate(){
        if(parameter.combatManager.isHeavyAttacking){
            return;
        }
        manager.TransitionState(KnightStateTypes.Idle);
    }
    public void OnFixedUpdate(){}
    public void OnExit(){
            
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
    }
    public void HandleInput(){}
    public void OnGetHit(HitData data){

    }
}
public class KHeavyAttack2State : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    public KHeavyAttack2State(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.PowerAttack("PowerAttack2"));
    }
    public void OnUpdate(){
        if(parameter.combatManager.isHeavyAttacking){
            return;
        }
        manager.TransitionState(KnightStateTypes.Idle);
    }
    public void OnFixedUpdate(){}
    public void OnExit(){
            
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
    }
    public void HandleInput(){}
    public void OnGetHit(HitData data){

    }
}

public class kJumpAttackState : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    public kJumpAttackState(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.JumpAttack());
    }
    public void OnUpdate(){
        if(parameter.combatManager.isJumpAttacking){
            return;
        }
        manager.TransitionState(KnightStateTypes.Idle);
    }
    public void OnFixedUpdate(){}
    public void OnExit(){
            
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
    }
    public void HandleInput(){}
    public void OnGetHit(HitData data){

    }
}
public class KShieldStrikeState : PlayerStateInterface
{
    private Knight manager;

    private KnightParameter parameter;

    public KShieldStrikeState(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }
    public void OnEnter(){
        parameter.combatManager.StartCoroutine(parameter.combatManager.ShieldStrike());
    }
    public void OnUpdate(){
        if(parameter.combatManager.isShieldStriking){
            return;
        }
        manager.TransitionState(KnightStateTypes.Idle);
    }
    public void OnFixedUpdate(){}
    public void OnExit(){
            
        parameter.movementManager.SetIsRunning(false);
        parameter.combatManager.ResetAllFlags();
    }
    public void HandleInput(){}
    public void OnGetHit(HitData data){

    }
}

public class KInvulnerableState : PlayerStateInterface{
    private Knight manager;

    private KnightParameter parameter;

    public KInvulnerableState(Knight manager){
        this.manager = manager;

        this.parameter = manager.parameter;
    }

    public void OnEnter(){
        
    }
    public void OnUpdate(){

    }
    public void OnFixedUpdate(){}
    public void OnExit(){

    }
    public void HandleInput(){}
    public void OnGetHit(HitData data){

    }
}



