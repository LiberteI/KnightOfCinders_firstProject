using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    public float maxHealth;

    public float curHealth;

    public bool isDead;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        
        curHealth = maxHealth;
        EventManager.RaiseEnemyHealthChanged(this.gameObject, curHealth, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        CheckIsDead();
    }

    private void CheckIsDead(){
        if(!isDead){
            if(curHealth <= 0){
                // set flag
                isDead = true;

                // broadcast
                EventManager.RaiseEnemyDied(this.gameObject);
            }
        }
        
    }

    public void TakeDamage(float damage){
        if(CanTakeDamage()){
            curHealth -= damage;
            curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
            EventManager.RaiseEnemyHealthChanged(this.gameObject, curHealth, maxHealth);
        }
        
    }
    public bool CanTakeDamage(){
        if(isDead){
            return false;
        }
        return true;
    }
}
