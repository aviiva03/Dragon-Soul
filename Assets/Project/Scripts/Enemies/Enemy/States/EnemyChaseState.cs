using UnityEngine;

public class EnemyChaseState : State {

    private readonly EnemyController _enemy;

    private float _strafeTimer;
    private Vector3 _strafeTarget;
    private const float StrafeDuration = 1.5f; // Как долго он идет в одном направлении
    private const float StrafeSpeedMultiplier = 0.4f; // Скорость при кружении
    private const float StrafeDistanceOffset = 1.2f; // Отступ для кружения (чтобы не толкать игрока)

    public EnemyChaseState(EnemyController enemy) {
        _enemy = enemy;
    }

    public override void Enter() {
        _enemy.Animator.SetBool("isIdle", false);
        _enemy.Animator.SetBool("isWalking", false);
        _enemy.Animator.SetBool("isRunning", true);

        _enemy.Agent.speed = _enemy.ChaseSpeed;
        _enemy.Agent.updateRotation = false;

        _strafeTimer = 0f;
        PickNewStrafeTarget();
    }

    public override void Update() {
        // 1. Потеряли игрока
        if (_enemy.DistanceToPlayer() > _enemy.LoseSightRange || !_enemy.CanSeePlayer()) {
            _enemy.SimpleAttackSelector.ResetLastSelected();
            _enemy.ToIdle();
            return;
        }

        // 2. Всегда смотрим на игрока
        _enemy.RotateTowardsPlayer(15f);

        float distance = _enemy.DistanceToPlayer();

        // 3. Если в зоне атаки и кулдаун прошел — БЬЕМ!
        if (distance <= _enemy.AttackRange && _enemy.CanAttack()) {
            ComboData combo = _enemy.SimpleAttackSelector.SelectCombo(distance);
            if (combo != null) {
                _enemy.SetPendingCombo(combo);
                _enemy.ToAttack();
                return;
            }
        }

        // 4. Если игрок подошел слишком близко (толкает нас), отходим назад
        if (distance < _enemy.AttackRange * 0.8f) {
            _enemy.Animator.SetBool("isRunning", false);
            _enemy.Animator.SetBool("isWalking", true);
            _enemy.Animator.SetBool("isIdle", false);
            _enemy.Agent.speed = _enemy.ChaseSpeed * StrafeSpeedMultiplier;

            // Вектор от игрока к врагу (направление отхода)
            Vector3 awayDir = (_enemy.transform.position - _enemy.Player.position).normalized;
            _enemy.MoveTo(_enemy.transform.position + awayDir * 2f);
            return;
        }

        // 5. Если в зоне атаки (но не слишком близко) и ждем кулдаун — КРУЖИМ на расстоянии
        if (distance <= _enemy.AttackRange + StrafeDistanceOffset) {
            _enemy.Animator.SetBool("isRunning", false);
            _enemy.Animator.SetBool("isWalking", true);
            _enemy.Animator.SetBool("isIdle", false);

            _enemy.Agent.speed = _enemy.ChaseSpeed * StrafeSpeedMultiplier;

            _strafeTimer += Time.deltaTime;
            if (_strafeTimer >= StrafeDuration) {
                PickNewStrafeTarget();
            }

            _enemy.MoveTo(_strafeTarget);
            return;
        }

        // 6. Если далеко — БЕЖИМ
        _enemy.MoveTo(_enemy.Player.position);
        _enemy.Agent.speed = _enemy.ChaseSpeed;

        _enemy.Animator.SetBool("isIdle", false);
        _enemy.Animator.SetBool("isWalking", false);
        _enemy.Animator.SetBool("isRunning", true);
    }

    public override void Exit() {
        _enemy.Animator.SetBool("isRunning", false);
        _enemy.Animator.SetBool("isWalking", false);
        _enemy.Animator.SetBool("isIdle", false);
        _enemy.Agent.updateRotation = true;
    }

    private void PickNewStrafeTarget() {
        _strafeTimer = 0f;

        // Выбираем случайный угол для кружения
        float randomAngle = Random.Range(-120f, 120f);

        // Направление от игрока к врагу
        Vector3 directionToEnemy = (_enemy.transform.position - _enemy.Player.position).normalized;

        // Поворачиваем это направление на случайный угол
        Vector3 strafeDir = Quaternion.Euler(0, randomAngle, 0) * directionToEnemy;

        // Ставим цель на безопасном расстоянии (радиус атаки + отступ)
        float safeDistance = _enemy.AttackRange + StrafeDistanceOffset;
        _strafeTarget = _enemy.Player.position + strafeDir * safeDistance;
    }
}