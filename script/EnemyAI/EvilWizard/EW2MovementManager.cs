using UnityEngine;

public class EW2MovementManager : EnemyMovementManager
{
    public EvilWizardPhase2 evilWizard;
    
    public override void FlipTo(Transform target){
        if(target != null){
            if(transform.position.x < target.position.x){
                transform.localScale = new Vector3(-8, 8, 8);
            }
            else if(transform.position.x > target.position.x){
                transform.localScale = new Vector3(8, 8, 8);
            }
        }
    }

    public override void WalkTowardsPlayer(){
        if(evilWizard.parameter.target == null){
            return;
        }

        float direction = evilWizard.parameter.target.position.x - transform.position.x;

        if(direction > 0){
            evilWizard.parameter.RB.linearVelocity = new Vector2(1 * walkSpeed, evilWizard.parameter.RB.linearVelocity.y);
        }
        else{
            evilWizard.parameter.RB.linearVelocity = new Vector2(-1 * walkSpeed, evilWizard.parameter.RB.linearVelocity.y);
        }
    }
}
