using UnityEngine;

public class DWMovementManager : EnemyMovementManager
{
    public float wolfWalkSpeed;

    public float runSpeed;

    public float chargeSpeed;

    public bool isMoving;

    public DarkWolf darkWolf;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetIsMoving();
    }

    public override void FlipTo(Transform target){
        if(target != null){
            if(transform.position.x < target.position.x){
                transform.localScale = new Vector3(-6, 6, 6);
            }
            else if(transform.position.x > target.position.x){
                transform.localScale = new Vector3(6, 6, 6);
            }
        }
    }

    public void SetIsMoving(){
        if(darkWolf.parameter.rb.linearVelocity.sqrMagnitude > 0.01f){
            isMoving = true;
        }
        else{
            isMoving = false;
        }
    }

    public void RunTowardsPlayer(){
        if(darkWolf.parameter.target == null){
            return;
        }

        float direction = darkWolf.parameter.target.position.x - transform.position.x;

        if(direction > 0){
            darkWolf.parameter.rb.linearVelocity = new Vector2(1 * runSpeed, darkWolf.parameter.rb.linearVelocity.y);
        }
        else{
            darkWolf.parameter.rb.linearVelocity = new Vector2(-1 * runSpeed, darkWolf.parameter.rb.linearVelocity.y);
        }
    }

    public override void WalkTowardsPlayer(){
        if(darkWolf.parameter.target == null){
            return;
        }

        float direction = darkWolf.parameter.target.position.x - transform.position.x;

        if(direction > 0){
            darkWolf.parameter.rb.linearVelocity = new Vector2(1 * darkWolf.parameter.dwMovementManager.walkSpeed, darkWolf.parameter.rb.linearVelocity.y);
        }
        else{
            darkWolf.parameter.rb.linearVelocity = new Vector2(-1 * darkWolf.parameter.dwMovementManager.walkSpeed, darkWolf.parameter.rb.linearVelocity.y);
        }
    }
}
