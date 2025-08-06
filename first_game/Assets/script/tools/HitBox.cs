using UnityEngine;

public class HitBox : MonoBehaviour
{   
    public Collider2D hitBox;

    public PlayerCombat combatControl;

    public Transform skeleton;

    public Transform darkWolf;

    public Transform wizard;

    public Transform laser;

    [SerializeField]
    private HealthBar healthBar;

    private bool hasHit;

    public void OnTriggerEnter2D(Collider2D other){
        // make sure GameObject hitbox is not tagged or layerMasked as enemy.
        // enemy layer and player layer are unchecked to interact in project settings. 
        if(other.CompareTag("Player")){
            if(!combatControl.isDead) {
                if(skeleton != null){
                    combatControl.SkeletonHit(skeleton);
                    // healthBar.TakeDamage(combatControl.SkeletonHit(skeleton));
                }
                if(darkWolf != null){
                    combatControl.DarkWolfHit(darkWolf);
                    // healthBar.TakeDamage(combatControl.DarkWolfHit(darkWolf));
                }
                if(wizard != null){
                    combatControl.WizardHitDownward(wizard);
                    // healthBar.TakeDamage(combatControl.WizardHitDownward(wizard));
                }
                if(laser != null){
                    combatControl.WizardHitDownward(laser);
                    // healthBar.TakeDamage(combatControl.WizardHitDownward(laser));
                }
                
            }
        }
    }
}
