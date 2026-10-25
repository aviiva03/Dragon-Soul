using UnityEngine;

public class CombatState : State {

    private readonly BossController _boss;
    private readonly AttackExecutor _executor;

    private enum Phase { Managing, Approaching, Strafing, Attacking, Repositioning }
    private Phase _phase;

    private ComboData _currentCombo;
    private int _stepIndex;

    private Vector3 _strafeTarget;
    private float _strafeTimer;
    private const float StrafeDuration = 1.2f;

    private float _repositionTimer;
    private const float RepositionDuration = 0.8f;

    private AttackData CurrentStep => _currentCombo.steps[_stepIndex];

    public CombatState(BossController boss) {
        _boss = boss;
        _executor = new AttackExecutor(boss);
        _executor.OnHitMoment += HandleHitMoment;
        _executor.OnFinished += HandleAttackFinished;
    }

    public override void Enter() {
        _boss.Animator.SetBool("isIdle", false);
        _boss.Animator.SetBool("isCombat", true);
        _boss.Agent.updateRotation = false;
        _boss.Agent.speed = _boss.GetCurrentSpeed();

        _currentCombo = null;
        _stepIndex = 0;

        EnterManaging();
    }

    public override void Update() {
        // Если потеряли игрока — уходим в Dormant
        if (_boss.DistanceToPlayer() > _boss.LoseSightRange || !_boss.CanSeePlayer()) {
            _boss.AttackSelector.ResetLastSelected();
            _boss.ToDormant();
            return;
        }

        switch (_phase) {
            case Phase.Managing: TickManaging(); break;
            case Phase.Approaching: TickApproaching(); break;
            case Phase.Strafing: TickStrafing(); break;
            case Phase.Attacking: _executor.Tick(); break;
            case Phase.Repositioning: TickRepositioning(); break;
        }
    }

    public override void Exit() {
        _boss.Animator.SetBool("isCombat", false);
        _boss.Agent.updateRotation = true;
        _boss.StopMoving();
    }

    private void EnterManaging() {
        _phase = Phase.Managing;
        _boss.Agent.speed = _boss.GetCurrentSpeed();
    }

    private void TickManaging() {
        _boss.RotateTowardsPlayer(12f);
        float distance = _boss.DistanceToPlayer();

        if (_boss.CanAttack()) {
            ComboData combo = _boss.AttackSelector.SelectCombo(distance);
            if (combo != null) {
                StartAttack(combo);
                return;
            }
        }

        if (distance > _boss.PreferredDistance + _boss.DistanceTolerance) {
            EnterApproaching();
        } else if (distance < _boss.PreferredDistance - _boss.DistanceTolerance) {
            EnterApproaching();
        } else {
            EnterStrafing();
        }
    }

    private void EnterApproaching() {
        _phase = Phase.Approaching;
        _boss.Agent.speed = _boss.GetCurrentSpeed();
    }

    private void TickApproaching() {
        _boss.RotateTowardsPlayer(12f);
        float distance = _boss.DistanceToPlayer();
        float target = _boss.PreferredDistance;

        if (Mathf.Abs(distance - target) <= _boss.DistanceTolerance) {
            EnterManaging();
            return;
        }

        if (_boss.CanAttack()) {
            ComboData combo = _boss.AttackSelector.SelectCombo(distance);
            if (combo != null) {
                StartAttack(combo);
                return;
            }
        }

        if (distance > target) {
            _boss.MoveTo(_boss.Player.position);
        } else {
            Vector3 away = (_boss.transform.position - _boss.Player.position).normalized;
            away.y = 0f;
            Vector3 dest = _boss.transform.position + away * 2f;
            _boss.MoveTo(dest);
        }
    }

    private void EnterStrafing() {
        _phase = Phase.Strafing;
        _strafeTimer = 0f;
        _boss.Agent.speed = _boss.GetCurrentSpeed() * 0.6f;
        PickNewStrafeTarget();
    }

    private void TickStrafing() {
        _boss.RotateTowardsPlayer(12f);
        float distance = _boss.DistanceToPlayer();

        if (_boss.CanAttack()) {
            ComboData combo = _boss.AttackSelector.SelectCombo(distance);
            if (combo != null) {
                StartAttack(combo);
                return;
            }
        }

        _strafeTimer += Time.deltaTime;

        if (_strafeTimer >= StrafeDuration) {
            EnterManaging();
            return;
        }

        _boss.MoveTo(_strafeTarget);
    }

    private void PickNewStrafeTarget() {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
        offset *= _boss.PreferredDistance;
        _strafeTarget = _boss.Player.position + offset;
    }

    private void StartAttack(ComboData combo) {
        _currentCombo = combo;
        _stepIndex = 0;
        _phase = Phase.Attacking;
        _boss.StopMoving();
        _executor.Start(CurrentStep);
    }

    private void HandleHitMoment() {
        if (!IsPlayerInAttackZone(CurrentStep)) return;
        if (_boss.Player.TryGetComponent(out IDamageable target))
            target.TakeDamage(CurrentStep.damage, _boss.Player.position);
    }

    private bool IsPlayerInAttackZone(AttackData data) {
        Vector3 toPlayer = _boss.Player.position - _boss.transform.position;
        toPlayer.y = 0f;
        if (toPlayer.magnitude > data.range) return false;
        if (data.hitAngle >= 360f) return true;
        float angle = Vector3.Angle(_boss.transform.forward, toPlayer);
        return angle <= data.hitAngle * 0.5f;
    }

    private void HandleAttackFinished() {
        _stepIndex++;
        if (_stepIndex >= _currentCombo.steps.Length) {
            EnterRepositioning();
        } else {
            _executor.Start(CurrentStep);
        }
    }

    private void EnterRepositioning() {
        _boss.NotifyAttackFinished();
        _phase = Phase.Repositioning;
        _repositionTimer = 0f;
        _boss.Agent.speed = _boss.GetCurrentSpeed();

        Vector3 away = (_boss.transform.position - _boss.Player.position).normalized;
        away.y = 0f;
        Vector3 target = _boss.transform.position + away * 3f;
        _boss.MoveTo(target);
    }

    private void TickRepositioning() {
        _boss.RotateTowardsPlayer(6f);
        _repositionTimer += Time.deltaTime;
        if (_repositionTimer >= RepositionDuration) {
            EnterManaging();
        }
    }
}