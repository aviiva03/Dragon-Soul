using UnityEngine;

public class DormantState : State {

    private readonly BossController _boss;

    public DormantState(BossController boss) { _boss = boss; }

    public override void Enter() {
        _boss.Animator.SetBool("isIdle", true);
        _boss.Animator.SetBool("isCombat", false);

        _boss.StopMoving();
        _boss.Agent.updateRotation = true;
    }

    public override void Update() {
        if (_boss.DistanceToPlayer() <= _boss.DetectionRange && _boss.CanSeePlayer())
            _boss.ToCombat();
    }

    public override void Exit() {
        _boss.Animator.SetBool("isIdle", false);
    }
}
