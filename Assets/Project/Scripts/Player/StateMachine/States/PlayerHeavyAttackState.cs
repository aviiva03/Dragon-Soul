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
        Controller.SetAttacking(true);
        Controller.ResetCombo();
        Controller.ConsumeAttackBuffer();
        Controller.ConsumeStamina(Stats.HeavyAttackStaminaCost);

        Controller.Sword.Appear();

        _phase = Phase.Windup;
        _phaseTimer = Stats.HeavyAttackWindup;
        _hasHit = false;
        _stepDone = false;

        // Поворот в сторону ввода (или камеры)
        Vector3 aimDirection = Controller.GetCameraRelativeMoveDirection();
        if (aimDirection.sqrMagnitude < 0.01f)
            aimDirection = Controller.CameraRef.PlanarForward;
        Controller.transform.rotation = Quaternion.LookRotation(aimDirection);

        Animator.PlayHeavyAttack();
    }

    public override void Update() {
        _phaseTimer -= Time.deltaTime;

        // Падение — прерываем (кроме Recovery)
        if (!Controller.IsGrounded && _phase != Phase.Recovery) {
            Controller.SetAttacking(false);
            StateMachine.ChangeState(Factory.Fall);
            return;
        }

        switch (_phase) {
            case Phase.Windup:
                ApplyWindupMovement();
                if (_phaseTimer <= 0f) {
                    _phase = Phase.Impact;
                    _phaseTimer = Stats.HeavyAttackImpact;
                }
                break;

            case Phase.Impact:
                if (!_stepDone) {
                    _stepDone = true;
                    Vector3 stepDir = Controller.transform.forward;
                    Controller.StartStep(
                        stepDir,
                        Stats.HeavyAttackStepDistance,
                        Stats.HeavyAttackStepDuration);
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
        // Запускаем таймер исчезновения меча
        if (Controller.Sword != null) Controller.Sword.AttackFinished();
    }

    private System.Collections.IEnumerator DisappearDelayed(float delay) {
        yield return new WaitForSeconds(delay);
        Controller.Sword.Disappear();
    }

    private void ApplyWindupMovement() {
        Vector3 camForward = Controller.CameraRef.PlanarForward;
        Vector3 camRight = Controller.CameraRef.PlanarRight;
        Vector3 inputDir = camForward * Controller.MoveInput.y + camRight * Controller.MoveInput.x;

        float speed = Stats.WalkSpeed * Stats.HeavyAttackWindupMoveMultiplier;
        Vector3 move = inputDir.normalized * speed;
        Controller.MoveCharacter(move * Time.deltaTime);

        if (inputDir.sqrMagnitude > 0.01f) {
            Quaternion targetRot = Quaternion.LookRotation(inputDir);
            Controller.transform.rotation = Quaternion.Slerp(
                Controller.transform.rotation,
                targetRot,
                5f * Time.deltaTime);
        }
    }

    private void PerformHitCheck() {
        Controller.Hitbox.Activate(
            Stats.HeavyAttackDamage,
            Controller.transform.forward);

        Debug.Log($"[ATTACK] HEAVY hit! damage={Stats.HeavyAttackDamage}");
    }

    private void ReturnToLocomotion() {
        if (!Controller.IsGrounded)
            StateMachine.ChangeState(Factory.Fall);
        else if (Controller.HasMoveInput)
            StateMachine.ChangeState(Factory.Run);
        else
            StateMachine.ChangeState(Factory.Idle);
    }
}