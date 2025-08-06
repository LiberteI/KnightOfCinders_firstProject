using UnityEngine;
using System.Collections;


public class PlayerMovement : MonoBehaviour
{   // Moving and jumping
    public float horizontal;
    public float speed = 8f;
    

    private float jumpingPower = 16f;
    private bool isFacingRight = true;

    [SerializeField] public Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemy;

    // Dashing
    private bool canDash = false;
    private bool isDashing;
    public float dashingPower = 3.5f;
    public float dashingTime = 0.5f;
    private float dashingCoolDown = 1f;
    [SerializeField] private TrailRenderer tr;

    public Animator animator;

    public bool isJumping;

    public int jumpState;
    
    public PlayerCombat combatControl;

    void Update(){
        
        handleDashing();
        
        Move();
        
    
        if(!isJumping) startDash();


        Jump();
        

        // Disable running animation states
        disableRunningAnimationWhenJumping();
        Flip();
    }


    private void Move(){
        
        if(isDashing) return;
        if(combatControl.isDead) return;
        if(combatControl.isHit) return;
       
        horizontal = Input.GetAxisRaw("Horizontal");
        
        if(combatControl.isBlocking){
            speed = 0;
            horizontal = 0;
        }
        // Stay put when blocking or dead
        else speed = 8f;
        
        if(combatControl.isAttacking){
            if(isFacingRight) horizontal = 1;
            else horizontal = -1;
            if(combatControl.attackType == "Light") rb.linearVelocity = new Vector2(horizontal * 3, rb.linearVelocity.y);
            else rb.linearVelocity = new Vector2(horizontal * 5, rb.linearVelocity.y);
        }
        else rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    private void disableRunningAnimationWhenJumping(){
        if(isJumping){
            animator.SetFloat("Speed", 0f);
        }else{
            animator.SetFloat("Speed", Mathf.Abs(horizontal));
        }
    }

    

    private void handleDashing(){
        canDash = true;
        if(isDashing){
            animator.SetBool("IsDashing", true);
            return;
        } 
        animator.SetBool("IsDashing", false);
    }

    private void startDash(){
        if(isDashing) return;
        if(Input.GetKeyDown("l") && canDash && IsGrounded()){
            StartCoroutine(Dash());
        }
    }

    private void Jump(){
        if(isDashing) return;
        if(Input.GetKeyDown("k") && IsGrounded() && !combatControl.isAttacking){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }

        // If the player is off the ground and have a velocity to y+
        if(!IsGrounded() && rb.linearVelocity.y > 0){
            isJumping = true;
            jumpState = 1;
        }
        // If the player is falling
        else if(rb.linearVelocity.y < 0){
            isJumping = true;
            jumpState = 2;
        }
        
        if(IsGrounded()){
            isJumping = false;
            jumpState = 0;
        } 
        animator.SetBool("IsJumping", isJumping);
        animator.SetInteger("JumpState", jumpState);

    }

    private bool IsGrounded(){
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer) || Physics2D.OverlapCircle(groundCheck.position, 0.1f, enemy);
    }

    private void Flip(){
        if(isDashing) return;
        if(combatControl.isAttacking) return;

        if(isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f){
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash(){
        canDash = false;
        isDashing = true;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true;
    }

    private void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
    }
    
    public bool getIsJumping(){
        return isJumping;
    }

    public void setSpeed(float sp){// Move a little bit while attacking
        if(isFacingRight) rb.linearVelocity = new Vector2(sp * 1, rb.linearVelocity.y);
        else rb.linearVelocity = new Vector2(sp * -1, rb.linearVelocity.y);
    }
}
