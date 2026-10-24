using UnityEngine;

public class EnemyChaseState : State {

    private readonly EnemyController _enemy;

    private Vector3 _strafeTarget;
    private float _strafeTimer;
    private const float StrafeDuration = 1.5f;

    public enum EnemyLocomotion { Idle, Walk, Run }

    public EnemyChaseState(EnemyController enemy) {
        _enemy = enemy;
    }

    public override void Enter() {
        _enemy.Animator.SetBool("isIdle", false);
        _enemy.Animator.SetBool("isWalking", false);
        _enemy.Animator.SetBool("isRunning", true);

        _enemy.Agent.speed = _enemy.ChaseSpeed;
        _enemy.Agent.updateRotation = false;

        _strafeTimer = 0;
    }

    public override void Update() {
        if (_enemy.DistanceToPlayer() > _enemy.LoseSightRange || !_enemy.CanSeePlayer()) {
            _enemy.SimpleAttackSelector.ResetLastSelected();
            _enemy.ToIdle();
            return;
        }

        _enemy.RotateTowardsPlayer(15f);

        float distance = _enemy.DistanceToPlayer();
        float preferred = _enemy.PreferredDistance;
        float tolerance = _enemy.DistanceTolerance;

        if (_enemy.CanAttack()) {
            ComboData combo = _enemy.SimpleAttackSelector.SelectCombo(distance);
            if (combo != null) {
                _enemy.SetPendingCombo(combo);
                _enemy.ToAttack();
                return;
            }
        }

        if (distance > preferred + tolerance) {
            SetLocomotion(EnemyLocomotion.Run);
            _enemy.Agent.speed = _enemy.ChaseSpeed;
            _enemy.MoveTo(_enemy.Player.position);
            return;
        }

        if (distance < preferred - tolerance) {
            SetLocomotion(EnemyLocomotion.Walk);
            _enemy.Agent.speed = _enemy.ChaseSpeed * 0.6f;

            Vector3 away = (_enemy.transform.position - _enemy.Player.position).normalized;
            away.y = 0f;
            Vector3 target = _enemy.transform.position + away * 2f;
            _enemy.MoveTo(target);
            return;
        }

        SetLocomotion(EnemyLocomotion.Walk);
        _enemy.Agent.speed = _enemy.ChaseSpeed * 0.5f;

        _strafeTimer += Time.deltaTime;

        if (_strafeTimer >= StrafeDuration || !_enemy.Agent.hasPath) {
            PickNewStrafeTarget();
        }

        _enemy.MoveTo(_strafeTarget);
    }

    public override void Exit() {
        _enemy.Animator.SetBool("isRunning", false);
        _enemy.Animator.SetBool("isWalking", false);
        _enemy.Agent.updateRotation = true;
    }

    private void SetLocomotion(EnemyLocomotion mode) {
        switch (mode) {
            case EnemyLocomotion.Idle:
                _enemy.Animator.SetBool("isRunning", false);
                _enemy.Animator.SetBool("isWalking", false);
                _enemy.Animator.SetBool("isIdle", true);
                break;
            case EnemyLocomotion.Walk:
                _enemy.Animator.SetBool("isRunning", false);
                _enemy.Animator.SetBool("isWalking", true);
                _enemy.Animator.SetBool("isIdle", false);
                break;
            case EnemyLocomotion.Run:
                _enemy.Animator.SetBool("isRunning", true);
                _enemy.Animator.SetBool("isWalking", false);
                _enemy.Animator.SetBool("isIdle", false);
                break;
        }
    }

    private void PickNewStrafeTarget() {
        _strafeTimer = 0f;

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
        offset *= _enemy.PreferredDistance;

        _strafeTarget = _enemy.Player.position + offset;
    }
}