using UnityEngine;

public class StealthAlertState : State {

    private readonly StealthGuardController _guard;

    private float _visibleTimer;
    private float _lostTimer;
    private bool _detected;
    private bool _exclamationShow;

    private const float SuspicionThreshold = 0.5f;

    public StealthAlertState(StealthGuardController guard) {
        _guard = guard;
    }

    public override void Enter() {
        _guard.Animator.SetBool("isIdle", false);
        _guard.Animator.SetBool("isWalking", false);
        _guard.Animator.SetBool("isAlert", true);

        _guard.StopMoving();
        _guard.Agent.updateRotation = false;

        _visibleTimer = 0f;
        _lostTimer = 0f;
        _detected = false;
        _exclamationShow = false;

        _guard.AlertIndicator.ShowQuestion();
    }

    public override void Update() {
        _guard.RotateTowardsPlayer(_guard.AlertLookSpeed);

        bool canSee = _guard.DistanceToPlayer() <= _guard.DetectionRange && _guard.CanSeePlayer();

        if (canSee) {
            _visibleTimer += Time.deltaTime;
            _lostTimer = 0f;

            if (!_exclamationShow && _visibleTimer >= SuspicionThreshold) {
                _exclamationShow = true;
                _guard.AlertIndicator.ShowExclamation();
            }

            if (_visibleTimer >= _guard.DetectionConfirmTime && !_detected) {
                _detected = true;
                TriggerRespawn();
            }
        } else {
            _lostTimer += Time.deltaTime;
            _visibleTimer = 0f;

            if (_exclamationShow) {
                _exclamationShow = false;
                _guard.AlertIndicator.ShowQuestion();
            }

            if (_lostTimer >= _guard.AlertDuration) {
                _guard.AlertIndicator.HideAll();
                _guard.ToPatrol();
                return;
            }
        }
    }

    public override void Exit() {
        _guard.Agent.updateRotation = true;
        _guard.AlertIndicator.HideAll();
        _guard.Animator.SetBool("isAlert", false);
    }

    private void TriggerRespawn() {
        if (GameManager.Instance != null)
            GameManager.Instance.RespawnPlayer();
    }
}
