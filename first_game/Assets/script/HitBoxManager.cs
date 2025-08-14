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

    public HitData(GameObject initiator, float damage){
        // pass 2 params
        this.initiator = initiator;

        this.damage = damage;
    }
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
            // Debug.Log($"initiator: {data.initiator}, targetHurtBox: {data.targetHurtBox}, damage: {data.damage}, kbdir: {data.knockBackDir}");
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
    
}
