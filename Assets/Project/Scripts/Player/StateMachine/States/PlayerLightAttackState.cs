using UnityEngine;

public class PlayerLightAttackState : PlayerState {

    private float _attackTimeLeft;
    private float _attackDuration;
    private bool _hasHit;

    public PlayerLightAttackState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f)
        : base(c, m, f) { }

    public override void Enter() {
        _attackDuration = Controller.LightAttackComboIndex == 0
            ? Stats.LightAttack1Duration
            : Stats.LightAttack2Duration;
        _attackTimeLeft = _attackDuration;
        _hasHit = false;

        Controller.SetAttacking(true);

        Vector3 aimDirection = Controller.GetCameraRelativeMoveDirection();
        if (aimDirection.sqrMagnitude < 0.01f)
            aimDirection = Controller.CameraRef.PlanarForward;
        Controller.transform.rotation = Quaternion.LookRotation(aimDirection);

        Controller.StartStep(
            aimDirection,
            Stats.LightAttackStepDistance,
            Stats.LightAttackStepDuration);

        // Показываем меч при начале атаки
        if (Controller.Sword != null) Controller.Sword.Appear();

        Animator.PlayLightAttack(Controller.LightAttackComboIndex);
    }

    public override void Update() {
        _attackTimeLeft -= Time.deltaTime;

        if (!Controller.IsGrounded) {
            Controller.SetAttacking(false);
            StateMachine.ChangeState(Factory.Fall);
            return;
        }

        float elapsed = _attackDuration - _attackTimeLeft;
        if (!_hasHit && elapsed >= _attackDuration * 0.4f) {
            _hasHit = true;
            PerformHitCheck();
        }

        if (Controller.WantsHeavyAttack) {
            StateMachine.ChangeState(Factory.HeavyAttack);
            return;
        }

        if (_attackTimeLeft <= 0f) {
            Controller.SetAttacking(false);

            if (Controller.HasBufferedAttack) {
                Controller.ConsumeAttackBuffer();
                Controller.AdvanceCombo();
                StateMachine.ChangeState(Factory.LightAttack);
                return;
            }

            Controller.StartComboWindow();
            Controller.AdvanceCombo();
            ReturnToLocomotion();
        }
    }

    public override void Exit() {
        Controller.SetAttacking(false);
        // Говорим мечу, что атака закончилась (запустится таймер на исчезновение)
        if (Controller.Sword != null) Controller.Sword.AttackFinished();
    }

    private void PerformHitCheck() {
        float damage = Controller.LightAttackComboIndex == 0
            ? Stats.LightAttackDamage1
            : Stats.LightAttackDamage2;
        float radius = Stats.LightAttackHitboxRadius;

        Controller.Hitbox.Activate(damage, Controller.transform.forward);
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