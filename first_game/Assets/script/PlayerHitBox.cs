using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    public PlayerCombat manager; 

    public GameObject enemy;

    public void OnTriggerEnter2D(Collider2D other){ //attack enemy
        if(other == null) return;
        
        if(other.CompareTag("Enemy")){
            GameObject hitObj = other.gameObject;
            enemy = hitObj;
            // enemy.GetComponent<>();
            if(enemy.name == "EvilWizardDamageHitBox"){
                // access the script in parent
                EvilWizardPhase2 script = enemy.GetComponentInParent<EvilWizardPhase2>();
                if(script != null){
                    script.parameter.health -= manager.damage;
                }
            }

            if(enemy.name == "DamageHitBox"){
                EvilWizard script = enemy.GetComponentInParent<EvilWizard>();
                if(script != null){
                    script.parameter.health -= manager.damage;
                }
            }

            if(enemy.name == "DarkWolfDamageBox"){
                DarkWolf script = enemy.GetComponentInParent<DarkWolf>();
                if(script != null){
                    script.parameter.health -= manager.damage;
                }
            }


            if(manager.attackType == "Light"){
                AttackSense.Instance.HitPause(manager.lightPause);
                AttackSense.Instance.CameraShake(manager.shakeTime, manager.lightStrength);
                // Debug.Log("light");
            }
            else if(manager.attackType == "Heavy"){
                AttackSense.Instance.HitPause(manager.heavyPause);
                AttackSense.Instance.CameraShake(manager.shakeTime, manager.heavyStrength);
                // Debug.Log("heavy");
            }
        }
        Skeleton skeleton = other.GetComponent<Skeleton>();
        DarkWolf darkWolf = other.GetComponent<DarkWolf>();
        EvilWizard wizard = other.GetComponent<EvilWizard>();
        
        if(skeleton != null){
            other.GetComponent<Skeleton>().parameter.getHit = true;
        }
        if(darkWolf != null){
            other.GetComponent<DarkWolf>().parameter.getHit = true;
        } 
        if(wizard != null){
            other.GetComponent<EvilWizard>().parameter.getHit = true;
        } 
    }
}
