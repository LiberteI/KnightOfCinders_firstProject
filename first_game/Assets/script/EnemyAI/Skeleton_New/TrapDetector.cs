using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class TrapDetector : MonoBehaviour
{
    // this script is for activating skeleton when player comes by.

    [SerializeField] NewSkeleton skeleton;

    [SerializeField] GameObject healthBar;

    [SerializeField] Transform detectorCentre;

    [SerializeField] float detectorRadius;

    [SerializeField] LayerMask playerLayer;

    [SerializeField] private GamePlayCoordinator gpCoordinator;

    private bool hasInvoked = false;

    private bool hasCleared;


    void Start(){
        StartCoroutine(SetSkeletonDead());
        skeleton.parameter.healthManager.maxHealth = 150f;
    }

    void Update(){
        InvokeSkeletonIfPlayerInRange();
        CheckHasCleared();
    }


    private IEnumerator SetSkeletonDead(){
        yield return null;
        skeleton.TransitionState(NSStateType.Trap);
    }

    private void OnDrawGizmos(){
        Gizmos.DrawWireSphere(detectorCentre.position, detectorRadius);
    }

    private bool PlayerIsInRange(){
        return Physics2D.OverlapCircle(detectorCentre.position, detectorRadius, playerLayer);
    }

    private void InvokeSkeletonIfPlayerInRange(){
        if(hasInvoked){
            return;
        }
        if(!PlayerIsInRange()){
            return;
        }

        StartCoroutine(InvokeSkeleton());
        hasInvoked = true;
    }
    private IEnumerator InvokeSkeleton(){
        skeleton.parameter.animator.Play("BoneAwakening");
        yield return null;
        // wait to get into animation
        AnimatorStateInfo info = skeleton.parameter.animator.GetCurrentAnimatorStateInfo(0);
        while (!info.IsName("BoneAwakening")){
            yield return null;
            info = skeleton.parameter.animator.GetCurrentAnimatorStateInfo(0);
        }
        // wait animation to finish
        while(info.IsName("BoneAwakening")){
            
            if(info.normalizedTime > 0.99f){
                break;
            }
            info = skeleton.parameter.animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }


        skeleton.TransitionState(NSStateType.Idle);
        healthBar.SetActive(true);
    }

    private void CheckHasCleared(){
        if(hasCleared){
            return;
        }
        if(skeleton.parameter.healthManager.isDead){
            hasCleared = true;
            EventManager.RaiseExitBossFight(gpCoordinator.curArena);
        }
    }
}
