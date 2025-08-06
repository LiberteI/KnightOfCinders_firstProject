using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public PlayerMovement playerMovement;

    
    public int maxHealth = 10000;
    public int playerHealth;

    private float heavyAttackTimer = -1f;
    private float lightAttackTimer = -1f;

    public int damage;

    public bool isAttacking = false;
    private bool isHeavyAttacking;
    public bool isBlocking;

    private int comboStep = 0;
    public string attackType;
    private float timer;
    private float interval = 0.5f;
    
    public bool isDead;

    public bool isHit;
    private Skeleton skeleton;
    public DarkWolf darkWolf;

    public float disableTime;

    [Header("Hit Feel")]
    public float shakeTime;
    public int lightPause;
    public float lightStrength;

    public int heavyPause;
    public float heavyStrength;
    
    void Start(){
        playerHealth = maxHealth;
    }
    void Update()
    {
        Attack();
        Block();
        if(isHit){
            BeingHit();
        }
    }   

    

    private void BeingHit(){ // disable movement when being hit 
        playerMovement.rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        playerMovement.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Block(){
        if(playerMovement.getIsJumping()) return;
        if(Input.GetKey("s")){
            animator.SetBool("IsBlocking", true);
            isBlocking = true;
        }
        else{
            animator.SetBool("IsBlocking", false);
            isBlocking = false;
        }
    }

    private void Attack(){
        if(Input.GetKey("w") && Input.GetKeyDown("j") && (comboStep == 2 || comboStep == 0)){
            
            isHeavyAttacking = true;
            damage = 60;
            if(heavyAttackTimer <= 0){
                isAttacking = true;
                attackType = "Heavy";
                comboStep = 0;

                timer = interval;
                animator.SetTrigger("H_Attack");
                animator.SetInteger("AttackStep", comboStep);
                heavyAttackTimer = 0.5f; 
            }   
            StartCoroutine("endHeavyAttack");
        }

        if(Input.GetKeyDown("j") && !isAttacking){
            if(isHeavyAttacking) return;
            
            damage = 30;
            isAttacking = true;
            attackType = "Light";
            comboStep ++;
            if(comboStep > 2){
                isAttacking = false;
                comboStep = 1;

                return;
            }
            
        
            timer = interval;

            animator.SetTrigger("L_Attack");
            animator.SetInteger("AttackStep", comboStep);
            
        }

    
        if(timer != 0){
            timer -= Time.deltaTime;
            if(timer <= 0){
                timer = 0;
                comboStep = 0;
                animator.SetInteger("AttackStep", comboStep);
                isAttacking = false;
            }
        }
        if(heavyAttackTimer > 0){
            heavyAttackTimer -= Time.deltaTime;
        }
        if(lightAttackTimer> 0){
            lightAttackTimer -= Time.deltaTime;
        }

    }

    public void AttackOver(){
        isAttacking = false;
    }
    
    private IEnumerator endHeavyAttack(){
        yield return new WaitForSeconds(0.5f);
        isHeavyAttacking = false;
    }

    public int SkeletonHit(Transform skeleton){
        int damage = 100;
        playerHealth -= damage;
        animator.SetTrigger("IsHit");
        
        if(playerHealth <= 0){
            Die();
        }
        return damage;
    }
    
    public void DarkWolfHit(Transform darkWolf){
        int damage = 0;
        if(!this.darkWolf.parameter.isCharging){
            damage = 500;
            playerHealth -= damage;
        }
        else{
            damage = 800;
            playerHealth -= damage;
        }
        animator.SetTrigger("IsHit");
        
        if(playerHealth <= 0){
            Die();
        }
        // return damage;
    }

    public void WizardHitDownward(Transform wizard){
        int damage = 500;

        playerHealth -= damage;

        animator.SetTrigger("IsHit");

        if(playerHealth <= 0){
            Die();
        }
        // return damage;
    }

    public void LaserHit(Transform laser){
        int damage = 600;

        playerHealth -= damage;

        animator.SetTrigger("IsHit");

        if(playerHealth <= 0){
            Die();
        }
        // return damage;
    }

    public void Die(){
        playerMovement.speed = 0;
        playerMovement.horizontal = 0;
        playerMovement.rb.linearVelocity = Vector2.zero;
        playerMovement.enabled = false;
        animator.SetBool("IsDead", true);
        isDead = true;
    }
    public void SetIsHit(){
        isHit = true;
    }
    public void DisableIsHit(){
        isHit = false;
    }

}
