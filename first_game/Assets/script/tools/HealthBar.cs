using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class HealthBar : MonoBehaviour
{
    /*
        The health bar is used by players and all the enemies.
        Behaviour: The default of the health bar is set to full. if the target takes damage, 
        the health bar would first shrink the red part slowly(not at once), and then stay where it is at,
        leaving a white bar. Then the white bar drops aligning with where the red part is.

        the heath bar would stay on the top of regular enemies, including prefabs that the bosses summon, as well as the player.
        But for the bosses, theirs are long ones sticked on the top of the screen.
        The behaviour follows the same as what has described above.
    */
    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private Slider whiteBarSlider;


    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float currentHealth;

    [SerializeField]
    private float whiteBarSpeed;

    [SerializeField]
    private PlayerCombat playerCombatScript;

    private float prevHealth;
    [SerializeField]
    private Transform healthBarCanvas;
    
    void Start(){
        maxHealth = playerCombatScript.maxHealth;
        currentHealth = playerCombatScript.playerHealth;

        prevHealth = currentHealth;

        healthSlider.maxValue = maxHealth;
        

        healthSlider.value = currentHealth;
    }
    void Update(){
        
        

        float playerHealth = playerCombatScript.playerHealth;
        if(Mathf.Abs(playerHealth - prevHealth) > 0.01f){
            UpdateHealth(playerHealth);
        }
        
    }
    // ensure health bar doesnt flip with the player
    void LateUpdate(){
        healthBarCanvas.rotation = Quaternion.identity;
    }

    // public void TakeDamage(float amount){
    //     currentHealth -= amount;
    //     currentHealth = Mathf.Clamp(currentHealth, 0 , maxHealth);
    //     UpdateHealth(currentHealth);
    // }
    public void UpdateHealth(float newHealth){
        if(newHealth != currentHealth){
            // restrict health between 0 and maxHealth
            currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
            prevHealth = currentHealth;
        
            // update redBar
            healthSlider.value = currentHealth;

            // trigger delayed effect(coroutine)
            StartCoroutine(UpdateWhiteBar());
        }   
        
    }

    private IEnumerator UpdateWhiteBar(){
    
        // red bar target percentage
        float target = currentHealth / maxHealth;

        // while white bar value > red bar percentage
        while(whiteBarSlider.value > target){
            whiteBarSlider.value -= Time.deltaTime * whiteBarSpeed / maxHealth;
            yield return null;
        }

        whiteBarSlider.value = target;
    }

    
}
