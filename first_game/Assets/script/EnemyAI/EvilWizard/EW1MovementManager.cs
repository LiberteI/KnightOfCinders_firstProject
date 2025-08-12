using UnityEngine;

public class EW1MovementManager : EnemyMovementManager
{
    public float runSpeed;

    public EvilWizard evilWizard;

    public override void FlipTo(Transform target){
        if(target != null){
            if(transform.position.x > target.position.x){
                transform.localScale = new Vector3(-6, 6, 6);
            }
            else if(transform.position.x < target.position.x){
                transform.localScale = new Vector3(6, 6, 6);
            }
        }
    }

    public void RunTowardsPlayer(){
        if(evilWizard.parameter.target == null){
            return;
        }

        float direction = evilWizard.parameter.target.position.x - transform.position.x;

        if(direction > 0){
            evilWizard.parameter.RB.linearVelocity = new Vector2(1 * runSpeed, evilWizard.parameter.RB.linearVelocity.y);
        }
        else{
            evilWizard.parameter.RB.linearVelocity = new Vector2(-1 * runSpeed, evilWizard.parameter.RB.linearVelocity.y);
        }
    }

    
}
