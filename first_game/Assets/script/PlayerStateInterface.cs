using UnityEngine;

public interface PlayerStateInterface
{
    void OnEnter();

    void OnUpdate();

    void OnExit();

    void HandleInput();

    void OnFixedUpdate();

    void OnGetHit(HitData data);
}
