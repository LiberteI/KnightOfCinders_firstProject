using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class HitData{
    public GameObject initiator;

    public GameObject targetHurtBox;

    public float damage;

    public float knockBackDir;

    public FeedbackData feedbackData;

    public HitData(GameObject initiator, float damage){
        // pass 2 params
        this.initiator = initiator;

        this.damage = damage;

        // instantiate feedbackData
        this.feedbackData = new FeedbackData();
    }
}
[Serializable]
public class FeedbackData{
    [Header("Hit Stop")]
    public float stopTime;

    [Header("Camera Shake")]
    public float amplitude;

    public float frequency;

    public float duration;

    

}

public class HitBoxManager : MonoBehaviour
{
    [Header("Attack Initiator")]
    [SerializeField] private GameObject initiator;

    [Header("Player Manager (for damage)")]
    [SerializeField] private CombatManager playerCombatManager;
    

    private void OnTriggerEnter2D(Collider2D other){
        // Debug.Log($"Laser trigger entered with {other.name}");
        if(initiator == null){
            Debug.Log("initiator is null");
            return;
        }
        // if(initiator.GetComponentInParent<EnemyCombatManager>() != null){
        //     Debug.Log($"initiator: {initiator}, other: {other.name}" );
        // }
        if(initiator.CompareTag("Enemy") && other.CompareTag("Player")){
            // if the initiator is enemy
            
            // get runtime damage:
            EnemyCombatManager manager = initiator.GetComponentInParent<EnemyCombatManager>();
            
            // pass hit data
            
            HitData data = new HitData(initiator, manager.damage);
            if(data == null){
                Debug.Log("data is null");
                return;
            }
            data.targetHurtBox = other.gameObject;
            // update knock back direction at runtime
            data.knockBackDir = data.targetHurtBox.transform.position.x - data.initiator.transform.position.x;

            // assign customised feedback
            AssignFeedbackData(manager.sourceFeedback, data.feedbackData);
            // Debug.Log($"initiator: {data.initiator}, targetHurtBox: {data.targetHurtBox}, damage: {data.damage}, kbdir: {data.knockBackDir}");
            
            // check should anounce first
            if(!PlayerHitShouldOccur()){
                return;
            }
            // raise announcement
            EventManager.RaiseHitOccured(data);
            
        }
        else if(initiator.CompareTag("Player") && other.CompareTag("Enemy")){
            // if the initiator is player

            // get runtime damage:
            HitData data = new HitData(initiator, playerCombatManager.damage);


            // pass hit data
            data.targetHurtBox = other.gameObject;

            // update knock back dir at runtime
            data.knockBackDir = data.targetHurtBox.transform.position.x - data.initiator.transform.position.x;

            // assign player customised feedback data
            AssignFeedbackData(playerCombatManager.sourceFeedback, data.feedbackData);

            
            // Debug.Log($"initiator: {data.initiator}, targetHurtBox: {data.targetHurtBox}, damage: {data.damage}, kbdir: {data.knockBackDir}");
            // raise hit occured takes in hit target
            EventManager.RaiseHitOccured(data);

        }
    }

    // setters for intantiating prefabs during runtime.
    public void SetInitiator(GameObject initiator){
        this.initiator = initiator;
    }


    public void SetCombatManager(CombatManager cm){
        playerCombatManager = cm;
    }
    
    public void AssignFeedbackData(FeedbackData source, FeedbackData target){
        // assign data manually

        // hit stop
        target.stopTime = source.stopTime;

        // camera shake
        target.duration = source.duration;

        target.amplitude = source.amplitude;

        target.frequency = source.frequency;
        
    }

    private bool PlayerHitShouldOccur(){
        // define in what circustances hit should not occur;
        
        // if player is invincible
        if(playerCombatManager.IsInvincible()){
            return false;
        }
        // if player has died
        if(playerCombatManager.knight.parameter.healthManager.isDead){
            return false;
        }
        return true;
    }

    
}
