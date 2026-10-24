using UnityEngine;

public class EnemyController : EnemyBase {

    [Header("Movement Speeds")]
    [SerializeField] private float _patrolSpeed = 2f;
    [SerializeField] private float _chaseSpeed = 4f;

    [Header("Patrol")]
    [SerializeField] private Transform[] _patrolPoints;

    [Header("Combat")]
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _preferredDistance = 4f;
    [SerializeField] private float _distanceTolerance = 1f;
    [SerializeField] private float _globalAttackCooldown = 2.0f;
    [SerializeField] private SimpleAttackSelector _attackSelector;

    private float _lastAttackTime = -999f;
    private float _lastHitTime = -999f;
    private const float HitCooldown = 0.5f;

    private EnemyIdleState _idleState;
    private EnemyPatrolState _patrolState;
    private EnemyChaseState _chaseState;
    private EnemyAttackState _attackState;
    private EnemyDeadState _deadState;
    private EnemyHitState _hitState;

    public float PatrolSpeed => _patrolSpeed;
    public float ChaseSpeed => _chaseSpeed;
    public Transform[] PatrolPoints => _patrolPoints;
    public float AttackRange => _attackRange;
    public float PreferredDistance => _preferredDistance;
    public float DistanceTolerance => _distanceTolerance;
    public SimpleAttackSelector SimpleAttackSelector => _attackSelector;
    public ComboData PendingCombo { get; private set; }

    public void ToIdle() => ChangeState(_idleState);
    public void ToPatrol() => ChangeState(_patrolState);
    public void ToChase() => ChangeState(_chaseState);
    public void ToAttack() => ChangeState(_attackState);
    public void ToDead() => ChangeState(_deadState);
    public void ToHit() => ChangeState(_hitState);

    protected override void Start() {
        base.Start();

        _idleState = new EnemyIdleState(this);
        _patrolState = new EnemyPatrolState(this);
        _chaseState = new EnemyChaseState(this);
        _attackState = new EnemyAttackState(this);
        _deadState = new EnemyDeadState(this);
        _hitState = new EnemyHitState(this);

        if (Health != null) {
            Health.OnDied += HandleDeath;
            Health.OnDamaged += HandleDamaged;
        }

        ChangeState(_idleState);
    }

    private void OnDestroy() {
        if (Health != null) {
            Health.OnDied -= HandleDeath;
            Health.OnDamaged -= HandleDamaged;
        }
    }

    private void HandleDeath() => ToDead();

    private void HandleDamaged(int amount, Vector3 point) {
        if (!Health.IsAlive) return;
        if (CurrentState == _attackState || CurrentState == _deadState || CurrentState == _hitState) return;

        if (Time.time - _lastHitTime < HitCooldown) return;
        _lastHitTime = Time.time;

        ToHit();
    }

    public bool CanAttack() => Time.time - _lastAttackTime >= _globalAttackCooldown;

    public void SetPendingCombo(ComboData combo) => PendingCombo = combo;
    public void ClearPendingCombo() => PendingCombo = null;
    public void NotifyAttackFinished() => _lastAttackTime = Time.time;
}