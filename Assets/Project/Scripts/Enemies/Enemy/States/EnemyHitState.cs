using UnityEngine;

public class EnemyHitState : State {

    private readonly EnemyController _enemy;

    private float _timer;
    private const float HitDuration = 0.3f;

    public EnemyHitState(EnemyController enemy) {
        _enemy = enemy;
    }

    public override void Enter() {
        _enemy.StopMoving();

        // Гасим все локомоции
        _enemy.Animator.SetBool("isIdle", false);
        _enemy.Animator.SetBool("isWalking", false);
        _enemy.Animator.SetBool("isRunning", false);

        // Триггер анимации
        _enemy.Animator.SetTrigger("hit");

        _timer = 0f;
    }

    public override void Update() {
        _timer += Time.deltaTime;

        if (_timer >= HitDuration) {
            // Возврат в бой или Idle
            if (_enemy.DistanceToPlayer() <= _enemy.DetectionRange && _enemy.CanSeePlayer())
                _enemy.ToChase();
            else
                _enemy.ToIdle();
        }
    }

    public override void Exit() { }
}