using UnityEngine;
using System;
public static class EventManager{
    public static event Action<GameObject, float, float> OnKnightHealthChanged;

    public static event Action OnKnightDied;
    
    public static event Action<float, float> OnKnightStaminaChanged;

    public static event Action OnKnightStaminaDepleted;

    public static event Action<HitData> OnHitOccured;

    public static event Action <GameObject, float, float> OnEnemyHealthChanged;
    
    public static event Action <GameObject> OnEnemyDied;

    public static event Action<ArenaSetUp> OnEnterBossFight;

    public static event Action<ArenaSetUp> OnExitBossFight;

    public static event Action<string> OnSceneChanged;

    public static event Action OnWinning;

    public static event Action OnDefeat;

    public static void RaiseDefeat(){
        if(OnDefeat != null){
            OnDefeat();
        }
    }

    public static void RaiseWinning(){
        if(OnWinning != null){
            OnWinning();
        }
    }

    public static void RaiseSceneChanged(string newScene){
        if(OnSceneChanged != null){
            OnSceneChanged(newScene);
        }
    }
    public static void KnightHealthChanged(GameObject knight, float curHealth, float maxHealth){
        if(OnKnightHealthChanged != null){
            OnKnightHealthChanged(knight, curHealth, maxHealth);
        }
    }

    public static void RaiseKnightDied(){
        if(OnKnightDied != null){
            OnKnightDied();
        }
    }

    public static void KnightStaminaChanged(float cur, float max){
        if(OnKnightStaminaChanged != null){
            OnKnightStaminaChanged(cur, max);
        }
    }

    public static void KnightStaminaDepleted(){
        if(OnKnightStaminaDepleted != null) {
            OnKnightStaminaDepleted();
        }
    }

    public static void RaiseHitOccured(HitData data){
        if(OnHitOccured != null){
            OnHitOccured(data);
        }
    }

    public static void RaiseEnemyHealthChanged(GameObject enemy, float cur, float max){
        if(OnEnemyHealthChanged != null){
            OnEnemyHealthChanged(enemy, cur, max);
        }
    }
    public static void RaiseEnemyDied(GameObject enemy){
        // Debug.Log($"OnEnemyDied invoked for: {enemy.name}");
        if(OnEnemyDied != null){
            OnEnemyDied(enemy);
        }
    }

    public static void RaiseEnterBossFight(ArenaSetUp curSetUp){
        if(OnEnterBossFight != null){
            OnEnterBossFight(curSetUp);
        }
    }

    public static void RaiseExitBossFight(ArenaSetUp curSetUp){
        if(OnExitBossFight != null){
            OnExitBossFight(curSetUp);
        }
    }
}
