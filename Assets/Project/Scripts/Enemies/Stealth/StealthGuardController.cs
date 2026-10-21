using UnityEngine;

public class StealthGuardController : EnemyBase {
    [Header("Movement")]
    [SerializeField] private float _patrolSpeed = 2f;
    [SerializeField] private float _alertLookSpeed = 4f;

    [Header("Patrol")]
    [SerializeField] private Transform[] _patrolPoints;

    [Header("Alert")]
    [Tooltip("Сколько секунд игрок должен быть вне видимости, чтобы охранник успокоился")]
    [SerializeField] private float _alertDuration = 1.5f;
    [Tooltip("Время после обнаружения до респавна игрока")]
    [SerializeField] private float _detectionConfirmTime = 1.5f;

    [Header("UI")]
    [SerializeField] private AlertIndicator _alertIndicator;

    private StealthPatrolState _patrolState;
    private StealthAlertState _alertState;

    public float PatrolSpeed => _patrolSpeed;
    public float AlertLookSpeed => _alertLookSpeed;
    public Transform[] PatrolPoints => _patrolPoints;
    public float AlertDuration => _alertDuration;
    public float DetectionConfirmTime => _detectionConfirmTime;
    public AlertIndicator AlertIndicator => _alertIndicator;

    public void ToPatrol() => ChangeState(_patrolState);
    public void ToAlert() => ChangeState(_alertState);

    protected override void Start() {
        base.Start();

        _patrolState = new StealthPatrolState(this);
        _alertState = new StealthAlertState(this);

        ChangeState(_patrolState);
    }
}
