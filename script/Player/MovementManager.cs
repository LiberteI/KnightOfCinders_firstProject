using UnityEngine;
using System.Collections;

public class MovementManager : MonoBehaviour
{
    [Header("HORIZONTAL MOVEMENT")]
    [SerializeField] private float walkSpeed;

    [SerializeField] private float runSpeed;

    public float horizontal;

    public bool run;

    private bool isFacingRight;

    public bool isRolling = false;

    public float lightAttackSpeed;

    public float heavyAttackSpeed;

    public float shieldStrikeSpeed;

    [Header("SWIM")]
    [SerializeField] private LayerMask Water;

    [SerializeField] private Transform body;

    [SerializeField] private float bodyRadius;

    public float vertical;

    [SerializeField] private float swimSpeed;

    [Header("JUMP")]
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float groundCheckRadius;

    public bool isJumping;

    [SerializeField] private float jumpCoolDown;

    [SerializeField] private float jumpForce;

    [SerializeField] private LayerMask groundLayer;

    private float currentJumpCoolDown;

    [Header("KNIGHT")]
    [SerializeField] private Knight knight;

    [SerializeField] private Transform knightTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(knight.transform.localScale.x > 0){
            isFacingRight = true;
        }
        else{
            isFacingRight = false;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        Flip();
        ReadVertical();
        ReadHorizontal();
        DecideCanJump();
        CheckIfHasJustLanded();
    }

    void FixedUpdate(){
        if(run){
            Run();
        }
        else{
            Walk();
        }
        Swim();
        
    }

    public void SetIsRunning(bool value){
        run = value;
    }

    public void ReadVertical(){
        vertical = Input.GetAxisRaw("Vertical");
    }
    public void ReadHorizontal(){
        horizontal = Input.GetAxisRaw("Horizontal");
    }
    private void Flip(){
        if(!CanFlip()){
            return;
        }
        if(horizontal == 0){
            return;
        }
        bool shouldFaceRight = false;
        Vector3 newPos = knightTransform.localScale;
        if(horizontal > 0){
            shouldFaceRight = true;
        }
        else{
            shouldFaceRight = false;
        }
        if(isFacingRight != shouldFaceRight){
            if(shouldFaceRight){
                newPos.x = Mathf.Abs(newPos.x);
            }
            else{
                newPos.x =  -Mathf.Abs(newPos.x);
            }
            transform.localScale = newPos;
            isFacingRight = shouldFaceRight;
        }
    }
    public float GetHorizontal(){
        return horizontal;
    }
    public void Walk(){
        if(!CanMove()){
            return;
        }
        knight.parameter.RB.linearVelocity = new Vector2(horizontal * walkSpeed, knight.parameter.RB.linearVelocity.y);
    }

    public void Run(){
        if(!CanMove()){
            return;
        }
        knight.parameter.RB.linearVelocity = new Vector2(horizontal * runSpeed, knight.parameter.RB.linearVelocity.y);
    }

    public void Jump(){
        if(!DecideCanJump()){
            return;
        }
        float xSpeed = 0;
        if(run){
            xSpeed = runSpeed;
        }
        else{
            xSpeed = walkSpeed;
        }
        knight.parameter.RB.linearVelocity = new Vector2(xSpeed, jumpForce);
    }
    /*
        In Roll coroutine:
        1. disable other movements
        2. assign speed based on walking or running, until animation ends.
        3. player is invincible
    */
    public IEnumerator Roll(){
        
        if(!CanRoll()){
            yield break;
        }
        isRolling = true;
        float curSpeed = 0;
        if(run){
            curSpeed = runSpeed;
        }
        else{
            curSpeed = walkSpeed;
        }
        knight.parameter.animator.Play("Roll");
        AnimatorStateInfo info = knight.parameter.animator.GetCurrentAnimatorStateInfo(0);
        
        
        while(info.normalizedTime < 1f){
            info = knight.parameter.animator.GetCurrentAnimatorStateInfo(0);
            knight.parameter.RB.linearVelocity = new Vector2(curSpeed * transform.localScale.x, knight.parameter.RB.linearVelocity.y);
            yield return null;
        }
        isRolling = false;
    }
    public bool CanFlip(){
        // disable flip when: rolling, defending, died, hit
        if(knight.parameter.combatManager.isDefending){
            return false;
        }
        if(isRolling){
            return false;
        }
        if(knight.parameter.combatManager.IsAttacking()){
            return false;
        }
        if(knight.parameter.healthManager.isDead){
            return false;
        }
        if(knight.parameter.combatManager.getHit){
            return false;
        }
        
        return true;
    }
    public bool CanRoll(){

        // disable roll when: not on the ground, is rolling, is attacking, died, hit
        if(!IsGrounded()){
            return false;
        }
        if(isRolling){
            return false;
        }
        if(knight.parameter.combatManager.IsAttacking()){
            return false;
        }
        if(knight.parameter.healthManager.isDead){
            return false;
        }
        if(knight.parameter.combatManager.getHit){
            return false;
        }
        
        return true;
    }
    public bool CanMove(){
        // disable move when defending, rolling, attacking, died, hit
        if(knight.parameter.combatManager.isDefending){
            return false;
        }
        if(isRolling){
            return false;
        }
        // exclude jump attacking
        if(knight.parameter.combatManager.isJumpAttacking){
            return true;
        }
        if(knight.parameter.combatManager.IsAttacking()){
            return false;
        }
        if(knight.parameter.healthManager.isDead){
            return false;
        }
        if(knight.parameter.combatManager.getHit){
            return false;
        }
        return true;
    }
    public bool DecideCanJump(){
        /*
            player cannot jump if:
            1. is not grounded
            2. is jumping
            3. has just landed.
            4. is dead
            5. is hit
        */
        if(!IsGrounded()){
            return false;
        }
        if(IsJumping()){
            return false;
        }
        if(currentJumpCoolDown > 0){
            return false;
        }
        if(knight.parameter.healthManager.isDead){
            return false;
        }

        if(knight.parameter.combatManager.getHit){
            return false;
        }
        return true;
    }

    public bool IsMoving(){
        if(knight.parameter.RB.linearVelocity.x != 0){
            return true;
        }
        return false;
    }
    public bool IsJumping(){
        return !IsGrounded() && knight.parameter.RB.linearVelocity.y != 0;
    }
    public bool IsGrounded(){
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
    }
    public bool CheckIfHasJustLanded(){
        /*
            in CheckIfHasJustLanded, execute:
            1. start jump cool down, which is a very short period.
            2. return a flag implying player has landed.
            3. reset isJumping flag.
        */
        
        // is has just landed if:
        // is jumping but has grounded.
        if(IsJumping()){
            if(IsGrounded()){
                StartCoroutine(StartJumpCoolDown());
                return true;
            }
        }
        return false;
        
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(body.position, bodyRadius);
    }

    private IEnumerator StartJumpCoolDown(){
        currentJumpCoolDown = jumpCoolDown;
        while(currentJumpCoolDown > 0){
            currentJumpCoolDown -= Time.deltaTime;
            yield return null;
        }
    } 

    public void Swim(){
        // Debug.Log(IsInSewer());
        if(!IsInSewer()){
            knight.parameter.RB.gravityScale = 5f;
            knight.parameter.RB.linearDamping = 0f;
            return;
        }
        knight.parameter.RB.gravityScale = 1f;
        knight.parameter.RB.linearDamping = 30f;

        knight.parameter.RB.linearVelocity = new Vector2(knight.parameter.RB.linearVelocity.x, vertical * swimSpeed);
    }

    private bool IsInSewer(){
        
        return Physics2D.OverlapCircle(body.position, bodyRadius, Water);
    }

    
}
