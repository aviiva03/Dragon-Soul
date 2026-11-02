using UnityEngine;

public class PlayerHeavyAttackState : PlayerState {

    public enum Phase { Windup, Impact, Recovery }

    private Phase _phase;
    private float _phaseTimer;
    private bool _hasHit;
    private bool _stepDone;

    public PlayerHeavyAttackState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f)
        : base(c, m, f) { }

    public override void Enter() {
        Controller.MarkHeavyAttackTriggered();

        Controller.CanMove = false;
        Controller.CanRotate = false;
        Controller.MoveDirection = Vector3.zero;
        Controller.SetAttacking(true);
        Controller.ResetCombo();
        Controller.ConsumeAttackBuffer();
        Controller.ConsumeStamina(Stats.HeavyAttackStaminaCost);

        if (Controller.Sword != null) Controller.Sword.Appear();

        _phase = Phase.Windup;
        _phaseTimer = Stats.HeavyAttackWindup;
        _hasHit = false;
        _stepDone = false;

        // Фиксируем поворот
        Vector3 lookDir = Controller.transform.forward;
        Controller.transform.rotation = Quaternion.LookRotation(lookDir);

        Animator.PlayHeavyAttack();
    }

    public override void Update() {
        _phaseTimer -= Time.deltaTime;

        if (!Controller.IsGrounded && _phase != Phase.Recovery) {
            Controller.SetAttacking(false);
            StateMachine.ChangeState(Factory.Fall);
            return;
        }

        switch (_phase) {
            case Phase.Windup:
                if (_phaseTimer <= 0f) {
                    _phase = Phase.Impact;
                    _phaseTimer = Stats.HeavyAttackImpact;
                }
                break;

            case Phase.Impact:
                if (!_stepDone) {
                    _stepDone = true;
                    // Маленький шаг вперед при ударе
                    Controller.StartStep(Controller.transform.forward, Stats.HeavyAttackStepDistance, Stats.HeavyAttackStepDuration);
                }
                if (!_hasHit) {
                    _hasHit = true;
                    PerformHitCheck();
                }
                if (_phaseTimer <= 0f) {
                    _phase = Phase.Recovery;
                    _phaseTimer = Stats.HeavyAttackRecovery;
                }
                break;

            case Phase.Recovery:
                if (_phaseTimer <= 0f) {
                    Controller.SetAttacking(false);
                    ReturnToLocomotion();
                }
                break;
        }
    }

    public override void Exit() {
        Controller.SetAttacking(false);
        if (Controller.Sword != null) Controller.Sword.AttackFinished();

        Controller.CanMove = true;
        Controller.CanRotate = true;
    }

    private void PerformHitCheck() {
        Controller.Hitbox.Activate(Stats.HeavyAttackDamage, Controller.transform.forward);
    }

    private void ReturnToLocomotion() {
        if (!Controller.IsGrounded) StateMachine.ChangeState(Factory.Fall);
        else if (Controller.HasMoveInput) StateMachine.ChangeState(Factory.Run);
        else StateMachine.ChangeState(Factory.Idle);
    }
}