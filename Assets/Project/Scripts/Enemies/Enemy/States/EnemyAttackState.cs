using UnityEngine;

public class EnemyAttackState : State {

    private readonly EnemyController _enemy;
    private readonly AttackExecutor _executor;

    private ComboData _currentCombo;
    private int _stepIndex;

    private AttackData CurrentStep => _currentCombo.steps[_stepIndex];

    public EnemyAttackState(EnemyController enemy) {
        _enemy = enemy;
        _executor = new AttackExecutor(enemy);

        _executor.OnHitMoment += HandleHitMoment;
        _executor.OnFinished += HandleFinished;
    }

    public override void Enter() {
        _enemy.Animator.SetBool("isIdle", false);
        _enemy.Animator.SetBool("isWalking", false);
        _enemy.Animator.SetBool("isRunning", false);

        _enemy.StopMoving();
        _enemy.Agent.updateRotation = false;

        if (_enemy.PendingCombo != null) {
            _currentCombo = _enemy.PendingCombo;
            _enemy.ClearPendingCombo();
        } else {
            _currentCombo = _enemy.SimpleAttackSelector.SelectCombo(_enemy.DistanceToPlayer());
        }

        if (_currentCombo == null) {
            _enemy.ToChase();
            return;
        }

        _stepIndex = 0;
        _executor.Start(CurrentStep);
    }

    public override void Update() {
        _executor.Tick();
    }

    public override void Exit() {
        _enemy.Agent.updateRotation = true;
    }

    private void HandleHitMoment() {
        if (!IsPlayerInAttackZone(CurrentStep)) return;

        if (_enemy.Player.TryGetComponent(out IDamageable target))
            target.TakeDamage(CurrentStep.damage, _enemy.Player.position);
    }

    private bool IsPlayerInAttackZone(AttackData data) {
        Vector3 toPlayer = _enemy.Player.position - _enemy.transform.position;
        toPlayer.y = 0f;

        if (toPlayer.magnitude > data.range) return false;
        if (data.hitAngle >= 360f) return true;

        float angle = Vector3.Angle(_enemy.transform.forward, toPlayer);
        return angle <= data.hitAngle * 0.5f;
    }

    private void HandleFinished() {
        _stepIndex++;

        if (_stepIndex >= _currentCombo.steps.Length) {
            _enemy.NotifyAttackFinished();

            if (_enemy.DistanceToPlayer() <= _enemy.DetectionRange && _enemy.CanSeePlayer())
                _enemy.ToChase();
            else
                _enemy.ToIdle();
        } else {
            _executor.Start(CurrentStep);
        }
    }
}
