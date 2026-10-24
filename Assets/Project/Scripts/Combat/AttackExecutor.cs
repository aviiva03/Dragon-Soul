using System;
using UnityEngine;

public class AttackExecutor {

    private readonly EnemyBase _enemy;

    private AttackData _data;
    private float _timer;
    private bool _hitDelivered;

    private enum Phase { Windup, Strike, Recovery }
    private Phase _phase;

    public event Action OnHitMoment;
    public event Action OnFinished;

    public AttackExecutor(EnemyBase enemy) { _enemy = enemy; }

    public void Start(AttackData data) {
        if (data == null) {
            Debug.LogError("[AttackExecutor] Данные атаки (AttackData) равны null! Проверь скрипт.");
            _data = null;
            return;
        }

        _data = data;
        _phase = Phase.Windup;
        _timer = 0f;
        _hitDelivered = false;

        _enemy.RotateTowardsPlayer(12f);
        _enemy.Animator.SetTrigger(_data.animationTrigger);
    }

    public bool Tick() {
        if (_data == null) return true;

        _timer += Time.deltaTime;

        switch (_phase) {
            case Phase.Windup:
                _enemy.RotateTowardsPlayer(12f);
                if (_timer >= _data.windupTime) Next(Phase.Strike);
                break;

            case Phase.Strike:
                if (!_hitDelivered && _timer >= _data.strikeTime * 0.5f) {
                    _hitDelivered = true;
                    ExecuteMovement();
                    OnHitMoment?.Invoke();
                }
                if (_timer >= _data.strikeTime) Next(Phase.Recovery);
                break;

            case Phase.Recovery:
                if (_timer >= _data.recoveryTime) {
                    OnFinished?.Invoke();
                    return true;
                }
                break;
        }

        return false;
    }

    private void Next(Phase next) {
        _phase = next;
        _timer = 0f;
    }

    private void ExecuteMovement() {
        if (_data.forwardJumpDistance <= 0f) return;

        Vector3 direction;
        if (_data.jumpToPlayer) {
            direction = _enemy.Player.position - _enemy.transform.position;
            direction.y = 0f;
            direction.Normalize();
        } else {
            direction = _enemy.transform.forward;
        }

        Vector3 target = _enemy.transform.position + direction * _data.forwardJumpDistance;

        _enemy.Agent.Warp(target);
    }
}