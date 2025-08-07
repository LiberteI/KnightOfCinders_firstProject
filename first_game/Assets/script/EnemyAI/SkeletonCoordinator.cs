using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public enum SkeletonRole{
    Frontliner, Flanker, Backuper
}
public class SkeletonCoordinator : MonoBehaviour
{

    /*
        Scenario: a group of skeletons will cooperate to attack the player.
        the total number could be around 7. with 1 flanker, 2 frontliner, and rest being back-uper.
       -1. skeletons are placed at the border of the map once the camera is fixed. blocking player's way to the next scene *

        0. skeletons starts as back-upers. then 3 of them are chosen to be flanker and frontliner *
            
        1. 2 frontliners will always be hunting the player with a normal speed *

        2. 1 flanker will sneak around and hunt player with a swifter speed but less HP. *
            - flanker will keep distance with the player.(sneaky state). *
              In sneaky state, the flanker maintains mid-range distance. It will decide how to move every 0.5s.
              if the player is chasing the flanker, the flanker will retreat but not instantly. 
              if get hit, it will retreat instantly.
              if the player is too far away, the flanker will approach the player.

              psudo-code:
              private float decideTimer = 0.5f;
              private float curDecideTimer;

              private float curDistance; // current distance
              private float desiredDistance; // the range that skeleton wants to keep from the player
              private float distanceMargin; // a margin value that keeps skeleton from jittering
               


              Update():
              FlipTo(player);
              CalculateDistance();
              if(curDecideTimer >= 0) keep deducting.

              
              if(getHit) RetreatImmediately();
              every 0.5s: DecideMove();
              
              void DecideMove():
               // too close: desiredDistance - margin ==> run away
               // too far: desiredDistance + margin ==> sneak on player
               // just right: [desiredDistance - margin, desiredDistance + margin] ==> idle
              
              
              
            - Every 3–5 seconds,  *
              - if it is in front of the player:
                it briefly dashes in to perform a light attack and then retreats, simulating probing behavior.
              - if it is at the back of the player:
                it briefly dashes in to perform a heavy attack and then retreats, but if player turns around, it will back up again.
            - if it get hit, transition to hurt and then back to sneaky.  *
        3. the rest back-upers will join the fight if either of their ally falls. they replace the position that has fallen.
            when they are watching in the back, they play defend animation and attack if player comes around. *

        4. Threat Level Shifting (Agro Switch) *
            Skeletons re-evaluate target every 2 seconds.

            if the frontliner's HP is less than 10%, a back-uper will join as a frontliner.

            if the original flanker is dead, a back-uper will join as a flanker.

            Purpose: Keeps combat dynamic, avoids player cheesing.

    */  
    [SerializeField] private GameObject skeletonPrefab;

    private List<NSCombatManager> skeletons = new List<NSCombatManager>();

    [SerializeField] int skeletonCount;

    [SerializeField] Knight knight;

    [SerializeField] Transform knightTransform;

    [SerializeField] NSCombatManager curFrontliner1;

    [SerializeField] NSCombatManager curFrontliner2;

    [SerializeField] NSCombatManager curFlanker;

    [SerializeField] float AgroSwitchTimer;

    [SerializeField] float curAgroSwitchTimer;
    
    private void SpawnSkeletons(int skeletonCount){
        for(int i = 0; i < skeletonCount; i++){
            // randomly assign skeletons' spawn points
            Vector3 spPoint = new Vector3(0, 0, 0);
            spPoint.x = UnityEngine.Random.Range(180f, 187f);
            spPoint.y = 4f;
            spPoint.z = 2f;

            GameObject instance = Instantiate(skeletonPrefab, spPoint, Quaternion.identity);
            NSCombatManager cm = instance.GetComponentInChildren<NSCombatManager>();
            skeletons.Add(cm);

            // assign default role
            skeletons[i].currentRole = SkeletonRole.Backuper;

            // assign target to knight at run time
            skeletons[i].knight = this.knight;
            skeletons[i].newSkeleton.parameter.target = knightTransform;
            skeletons[i].newSkeleton.parameter.movementManager.target = knightTransform;

            // set active
            instance.SetActive(true);
        }
    }
    void Start(){
        if(skeletonCount < 3){
            Debug.Log("count is too small.");
            return;
        }
        if(skeletonPrefab == null){
            Debug.Log("Prefab is not assigned!");
            return;
        }
        // randomly spawn skeletons as a squat.
        SpawnSkeletons(skeletonCount);
        StartCoroutine(StartAttacking());
        
    }

    void Update(){
        UpdateAgroSwitchTimer();
        CheckEveryTwoSeconds();
    }
    private void AssignRole(){
        // since skeletons' positions are random by default, it is easier to assign roles manually.
        // and cache skeleton script

        
        skeletons[0].currentRole = SkeletonRole.Frontliner;
        curFrontliner1 = skeletons[0];
        
        skeletons[1].currentRole = SkeletonRole.Frontliner; 
        curFrontliner2 = skeletons[1];

        skeletons[2].currentRole = SkeletonRole.Flanker;
        curFlanker = skeletons[2];

        // set health and speed here.
        // 1.8 time of normal speed but only 0.2 time of normal health
        curFlanker.newSkeleton.parameter.movementManager.walkSpeed = 1.5f * curFlanker.newSkeleton.parameter.movementManager.walkSpeed;
        curFlanker.newSkeleton.parameter.healthManager.maxHealth = 0.2f * curFlanker.newSkeleton.parameter.healthManager.maxHealth;
    }

    private IEnumerator StartAttacking(){
        yield return new WaitForSeconds(1.5f);

        AssignRole();
    }


    private void TrackCurSkeletonSquat(){
        if(curFrontliner1 == null || curFrontliner2 == null || curFlanker == null){
            return;
        }
        // Debug.Log("Executed");
        /* 
            check on each skeleton that is in battle every 2 seconds *

            if the current frontliner's HP is less than 10%, loop through back-upers and choose the first one to be a new frontliner.
            the new frontliner will join the fight. *

            if the current flanker is dead, find a new flanker. *

            if there is no skeleton anymore, return. *
        */
        if(curAgroSwitchTimer > 0){
            return;
        }
        if(curFrontliner1.newSkeleton.parameter.healthManager.isDead || 
            curFrontliner1.newSkeleton.parameter.healthManager.curHealth < 0.1f * curFrontliner1.newSkeleton.parameter.healthManager.maxHealth){
            // loop through squat and find the first non dead back-uper
            for(int i = 0; i < skeletonCount; i ++){
                if(!skeletons[i].newSkeleton.parameter.healthManager.isDead){
                    if(skeletons[i].newSkeleton.parameter.nsCombatManager.currentRole == SkeletonRole.Backuper){
                        skeletons[i].newSkeleton.parameter.nsCombatManager.currentRole = SkeletonRole.Frontliner;
                        // update current frontliner
                        Debug.Log("1 is replaced");
                        curFrontliner1 = skeletons[i];
                        break;
                    }
                }
            }
        }
        if(curFrontliner2.newSkeleton.parameter.healthManager.isDead || 
            curFrontliner2.newSkeleton.parameter.healthManager.curHealth < 0.1f * curFrontliner2.newSkeleton.parameter.healthManager.maxHealth){
            // loop through squat and find the first non dead back-uper
            for(int i = 0; i < skeletonCount; i ++){
                if(!skeletons[i].newSkeleton.parameter.healthManager.isDead){
                    if(skeletons[i].newSkeleton.parameter.nsCombatManager.currentRole == SkeletonRole.Backuper){
                        skeletons[i].newSkeleton.parameter.nsCombatManager.currentRole = SkeletonRole.Frontliner;

                        // update current frontliner
                        Debug.Log("2 is replaced");
                        curFrontliner2 = skeletons[i];
                        break;
                    }
                }
            }
        }
        if(curFlanker.newSkeleton.parameter.healthManager.isDead){
            Debug.Log("Flank is dead");
            // loop through squat and find the first non dead back-uper
            for(int i = 0; i < skeletonCount; i ++){
                Debug.Log($"i: {i}");
                if(!skeletons[i].newSkeleton.parameter.healthManager.isDead){
                    Debug.Log("found a not dead skeleton");
                    if(skeletons[i].newSkeleton.parameter.nsCombatManager.currentRole == SkeletonRole.Backuper){
                        skeletons[i].newSkeleton.parameter.nsCombatManager.currentRole = SkeletonRole.Flanker;

                        // update current frontliner
                        curFlanker = skeletons[i];
                        Debug.Log("flanker is replaced");
                        // set health and speed here.
                        // 1.8 time of normal speed but only 0.2 time of normal health
                        curFlanker.newSkeleton.parameter.movementManager.walkSpeed = 1.5f * curFlanker.newSkeleton.parameter.movementManager.walkSpeed;
                        curFlanker.newSkeleton.parameter.healthManager.maxHealth = 0.2f * curFlanker.newSkeleton.parameter.healthManager.maxHealth;
                        break;
                    }
                }
            }
        }
        curAgroSwitchTimer = AgroSwitchTimer;
    }
    private void UpdateAgroSwitchTimer(){
        if(curAgroSwitchTimer > 0){
            curAgroSwitchTimer -= Time.deltaTime;
        }
    }
    private void CheckEveryTwoSeconds(){
        if(curAgroSwitchTimer > 0){
            return;
        }
        TrackCurSkeletonSquat();
    }


}
