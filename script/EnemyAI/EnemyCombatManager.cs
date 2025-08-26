using UnityEngine;
using System.Collections;
public class EnemyCombatManager : MonoBehaviour
{
    [SerializeField] protected Transform attackCircleCentre;
 
    [SerializeField] protected float attackCircleRadius;

    [SerializeField] protected LayerMask playerLayer;

    public bool isAttacking;

    public float damage;

    public FeedbackData sourceFeedback;

    void Start(){
        
    }
    public virtual void GetHit(HitData data){
        
    }

    public virtual void SetFeedback(){

    }

    public bool PlayerIsInRange(){
        return Physics2D.OverlapCircle(attackCircleCentre.position, attackCircleRadius, playerLayer);
    }

    protected IEnumerator AnimationFinishes(string animationName, Animator animator){
        // wait to get into animation
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        while (!info.IsName(animationName)){
            yield return null;
            info = animator.GetCurrentAnimatorStateInfo(0);
        }
        while(info.IsName(animationName)){
            
            if(info.normalizedTime > 0.99f){
                break;
            }
            info = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
    }

    private void OnDrawGizmos(){
        Gizmos.DrawWireSphere(attackCircleCentre.position, attackCircleRadius);
    }   


}
