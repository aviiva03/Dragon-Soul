using UnityEngine;

public class StealthPatrolState : State {

    private readonly StealthGuardController _guard;

    private int _currentPointIndex;
    private bool _isWaiting;
    private float _waitTimer;
    private const float WaitAtPoint = 2f;

    public StealthPatrolState(StealthGuardController guard) {
        _guard = guard;
    }

    public override void Enter() {
        _guard.Animator.SetBool("isIdle", false);
        _guard.Animator.SetBool("isWalking", true);

        _guard.Agent.speed = _guard.PatrolSpeed;
        _guard.Agent.updateRotation = true;

        _isWaiting = false;
        _waitTimer = 0f;

        if (_guard.PatrolPoints.Length == 0) return;

        _guard.MoveTo(_guard.PatrolPoints[_currentPointIndex].position);
    }

    public override void Update() {
        if (_guard.DistanceToPlayer() <= _guard.DetectionRange && _guard.CanSeePlayer()) {
            _guard.ToAlert();
            return;
        }

        if (_isWaiting) {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= WaitAtPoint) {
                _isWaiting = false;
                _waitTimer = 0f;

                _guard.Animator.SetBool("isIdle", false);
                _guard.Animator.SetBool("isWalking", true);

                _currentPointIndex = (_currentPointIndex + 1) % _guard.PatrolPoints.Length;
                _guard.MoveTo((_guard.PatrolPoints[_currentPointIndex].position));
            }
            return;
        }

        if (!_guard.Agent.pathPending && _guard.Agent.remainingDistance <= _guard.Agent.stoppingDistance) {
            _isWaiting = true;
            _waitTimer = 0f;
            _guard.StopMoving();

            _guard.Animator.SetBool("isWalking", false);
            _guard.Animator.SetBool("isIdle", true);
        }
    }

    public override void Exit() {
        _guard.Animator.SetBool("isWalking", false);
        _guard.Animator.SetBool("isIdle", false);
        _guard.Agent.isStopped = false;
    }
}
