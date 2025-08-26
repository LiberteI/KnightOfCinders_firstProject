using UnityEngine;



public class CombatFeedbackManager : MonoBehaviour
{


    /*
        game feel meter ususally consists of several parts:
        
        1. Weapon sound. *

        2. Weapon trail (aborted)

        3. Feedback hit flash (aborted)

        4. Feedback hit react *

        5. impact VFX(Visual effects): particle effects (aborted)

        6. impact sounds *

        7. Feedback hit stop *

        8. feedback knockback *

        9. UI health bar animation *

        10. camera shake *

        11. camera cinematic attack finisher *
    */   

    [SerializeField] HitStopManager hitStopManager;

    [SerializeField] CameraShakeManager cameraShakeManager;

    void OnEnable(){
        EventManager.OnHitOccured += OnHit;

        EventManager.OnExitBossFight += OnFinish;
    }

    void OnDisable(){
        EventManager.OnHitOccured -= OnHit;

        EventManager.OnExitBossFight -= OnFinish;
    }
    
    // hit occurs
    public void OnHit(HitData data){
        hitStopManager.ApplyHitStop(data.feedbackData.stopTime);
    
        cameraShakeManager.ApplyCameraShake(data.feedbackData.amplitude, data.feedbackData.frequency, data.feedbackData.duration);
    }

    public void OnFinish(ArenaSetUp curSetUp){
        hitStopManager.ApplyEnemyFinisher();

        cameraShakeManager.ApplyCameraShake(1f, 0.5f, 1f);
    }
}
