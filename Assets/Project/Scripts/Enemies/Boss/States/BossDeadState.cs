using UnityEngine;

public class BossDeadState : State {

    private readonly BossController _boss;
    private float _timer;
    private const float DeathDelay = 5f;

    public BossDeadState(BossController boss) {
        _boss = boss;
    }

    public override void Enter() {
        _boss.StopMoving();
        _boss.Agent.enabled = false;

        var colliders = _boss.GetComponentsInChildren<Collider>();
        foreach (var col in colliders) col.enabled = false;

        _boss.Animator.SetBool("isIdle", false);
        _boss.Animator.SetBool("isCombat", false);

        _boss.Animator.SetTrigger("death");

        _timer = 0f;
    }

    public override void Update() {
        _timer += Time.deltaTime;
        if (_timer >= DeathDelay) {
            Object.Destroy(_boss.gameObject);
        }
    }

    public override void Exit() { }
}