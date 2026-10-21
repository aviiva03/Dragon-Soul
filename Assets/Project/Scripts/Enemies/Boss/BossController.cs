using UnityEngine;

public class BossController : EnemyBase {

    [Header("Combat")]
    [SerializeField] private float _maxAttackRange = 10f;
    [SerializeField] private float _preferredDistance = 6f;
    [SerializeField] private float _distanceTolerance = 1.5f;
    [SerializeField] private float _globalAttackCooldown = 2.5f;
    [SerializeField] private float _chaseSpeed = 4f;

    [Header("References")]
    [SerializeField] private AttackSelector _attackSelector;
    [SerializeField] private BossPhaseController _phaseController;

    private float _lastAttackTime = -999f;

    private DormantState _dormantState;
    private CombatState _combatState;
    private BossDeadState _deadState;

    public float MaxAttackRange => _maxAttackRange;
    public float PreferredDistance => _preferredDistance;
    public float DistanceTolerance => _distanceTolerance;
    public float ChaseSpeed => _chaseSpeed;
    public AttackSelector AttackSelector => _attackSelector;
    public BossPhaseController PhaseController => _phaseController;

    public void ToDormant() => ChangeState(_dormantState);
    public void ToCombat() => ChangeState(_combatState);
    public void ToDead() => ChangeState(_deadState);

    protected override void Start() {
        base.Start();

        _dormantState = new DormantState(this);
        _combatState = new CombatState(this);
        _deadState = new BossDeadState(this);

        if (Health != null)
            Health.OnDied += HandleDeath;

        ChangeState(_dormantState);
    }

    private void OnDestroy() {
        if (Health != null)
            Health.OnDied -= HandleDeath;
    }

    private void HandleDeath() {
        ToDead();
    }

    public bool CanAttack() {
        float mult = 1f;
        if (_phaseController != null && _phaseController.CurrentPhase != null)
            mult = _phaseController.CurrentPhase.cooldownMultiplier;

        return Time.time - _lastAttackTime >= _globalAttackCooldown * mult;
    }

    public void NotifyAttackFinished() { _lastAttackTime = Time.time; }

    public float GetCurrentSpeed() {
        float mult = 1f;
        if (_phaseController != null && _phaseController.CurrentPhase != null)
            mult = _phaseController.CurrentPhase.speedMultiplier;

        return _chaseSpeed * mult;
    }
}