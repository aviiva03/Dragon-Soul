using UnityEngine;

public class EnemyIdleState : State {

    private readonly EnemyController _enemy;
    private float _timer;
    private const float IdleDuration = 3f;

    public EnemyIdleState(EnemyController enemy) {
        _enemy = enemy;
    }

    public override void Enter() {
        _enemy.Animator.SetBool("isIdle", true);
        _enemy.Animator.SetBool("isWalking", false);
        _enemy.Animator.SetBool("isRunning", false);

        _enemy.StopMoving();
        _enemy.Agent.updateRotation = true;

        _timer = 0f;
    }

    public override void Update() {
        bool canSee = _enemy.DistanceToPlayer() <= _enemy.DetectionRange && _enemy.CanSeePlayer();

        if (canSee) {
            _enemy.RotateTowardsPlayer(8f);
            if (_enemy.DistanceToPlayer() <= _enemy.AttackRange) {
                _enemy.ToChase();
                return;
            }
        }

        _timer += Time.deltaTime;

        if (_timer >= IdleDuration)
            _enemy.ToPatrol();
    }

    public override void Exit() {
        _enemy.Animator.SetBool("isIdle", false);
    }
}

//using UnityEngine;

//public class EnemyIdleState : State {

//    private readonly EnemyController _enemy;
//    private float _timer;
//    private const float IdleDuration = 3f;

//    public EnemyIdleState(EnemyController enemy) {
//        _enemy = enemy;
//    }

//    public override void Enter() {
//        _enemy.Animator.SetBool("isIdle", true);
//        _enemy.Animator.SetBool("isWalking", false);
//        _enemy.Animator.SetBool("isRunning", false);

//        _enemy.StopMoving();
//        _enemy.Agent.updateRotation = false;

//        _timer = 0f;
//    }

//    public override void Update() {
//        bool canSee = _enemy.DistanceToPlayer() <= _enemy.DetectionRange && _enemy.CanSeePlayer();

//        if (canSee) {
//            _enemy.RotateTowardsPlayer(8f);

//            if (_enemy.DistanceToPlayer() <= _enemy.AttackRange && _enemy.CanAttack()) {
//                _enemy.ToChase();
//                return;
//            }
//            return;
//        }

//        _timer += Time.deltaTime;

//        if (_timer >= IdleDuration)
//            _enemy.ToPatrol();
//    }

//    public override void Exit() {
//        _enemy.Animator.SetBool("isIdle", false);
//        _enemy.Agent.updateRotation = true;
//    }
//}
