using UnityEngine;
using System.Collections;
public class EnemyMovementManager : MonoBehaviour
{
    [SerializeField] private float walkSpeed;

    [SerializeField] private Transform target;

    [SerializeField] private Rigidbody2D RB;
    
    public void FlipTo(Transform target){
        if(target != null){
            Vector3 newPos = transform.localScale;
            
            if(transform.position.x < target.position.x){
                newPos.x = Mathf.Abs(newPos.x);
                transform.localScale = newPos;
            }
            else if(transform.position.x > target.position.x){
                newPos.x = -Mathf.Abs(newPos.x);
                transform.localScale = newPos;
            }
        }
    }

    public void WalkTowardsPlayer(){
        if(target == null){
            return;
        }

        float direction = target.position.x - transform.position.x;

        if(direction > 0){
            RB.linearVelocity = new Vector2(1 * walkSpeed, RB.linearVelocity.y);
        }
        else{
            RB.linearVelocity = new Vector2(-1 * walkSpeed, RB.linearVelocity.y);
        }
    }

    public void DisableVelocity(){
        RB.linearVelocity = Vector2.zero;
    }
}
