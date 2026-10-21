using UnityEngine;

public class EnemyPatrolState : State {

    private readonly EnemyController _enemy;

    private int _currentPointIndex;
    private bool _isWaiting;
    private float _waitTimer;
    private const float WaitAtPoint = 2f;

    public EnemyPatrolState(EnemyController enemy) {
        _enemy = enemy;
    }

    public override void Enter() {
        _enemy.Animator.SetBool("isIdle", false);
        _enemy.Animator.SetBool("isWalking", true);
        _enemy.Animator.SetBool("isRunning", false);

        _enemy.Agent.speed = _enemy.PatrolSpeed;
        _enemy.Agent.updateRotation = true;

        _isWaiting = false;
        _waitTimer = 0f;

        if (_enemy.PatrolPoints.Length == 0) return;

        _enemy.MoveTo(_enemy.PatrolPoints[_currentPointIndex].position);
    }

    public override void Update() {
        if (_enemy.DistanceToPlayer() <= _enemy.DetectionRange && _enemy.CanSeePlayer()) {
            _enemy.ToChase();
            return;
        }

        if (_isWaiting) {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= WaitAtPoint) {
                _isWaiting = false;
                _waitTimer = 0f;

                _enemy.Animator.SetBool("isIdle", false);
                _enemy.Animator.SetBool("isWalking", true);

                _currentPointIndex = (_currentPointIndex + 1) % _enemy.PatrolPoints.Length;
                _enemy.MoveTo(_enemy.PatrolPoints[_currentPointIndex].position);
            }
            return;
        }

        if (!_enemy.Agent.pathPending && _enemy.Agent.remainingDistance <= _enemy.Agent.stoppingDistance) {
            _isWaiting = true;
            _waitTimer = 0f;
            _enemy.StopMoving();

            _enemy.Animator.SetBool("isWalking", false);
            _enemy.Animator.SetBool("isIdle", true);
        }
    }

    public override void Exit() {
        _enemy.Animator.SetBool("isWalking", false);
        _enemy.Animator.SetBool("isIdle", false);
        _enemy.Agent.isStopped = false;
    }
}
