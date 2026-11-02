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

        // Блокировка движения и поворота
        Controller.CanMove = false;
        Controller.CanRotate = false;
        Controller.MoveDirection = Vector3.zero;
        Controller.SetAttacking(true);

        // Фиксируем поворот в сторону взгляда
        Vector3 lookDir = Controller.transform.forward;
        Controller.transform.rotation = Quaternion.LookRotation(lookDir);

        if (Controller.Sword != null) Controller.Sword.Appear();

        Animator.PlayLightAttack(Controller.LightAttackComboIndex);
    }

    public override void Update() {
        _attackTimeLeft -= Time.deltaTime;

        // Если упали с обрыва — прерываем атаку
        if (!Controller.IsGrounded) {
            Controller.SetAttacking(false);
            StateMachine.ChangeState(Factory.Fall);
            return;
        }

        // Наносим урон в середине анимации
        float elapsed = _attackDuration - _attackTimeLeft;
        if (!_hasHit && elapsed >= _attackDuration * 0.4f) {
            _hasHit = true;
            PerformHitCheck();
        }

        // Прерывание на тяжелую атаку
        if (Controller.WantsHeavyAttack) {
            StateMachine.ChangeState(Factory.HeavyAttack);
            return;
        }

        // Конец атаки
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
        if (Controller.Sword != null) Controller.Sword.AttackFinished();

        // Возвращаем управление
        Controller.CanMove = true;
        Controller.CanRotate = true;
    }

    private void PerformHitCheck() {
        float damage = Controller.LightAttackComboIndex == 0 ? Stats.LightAttackDamage1 : Stats.LightAttackDamage2;
        Controller.Hitbox.Activate(damage, Controller.transform.forward);
    }

    private void ReturnToLocomotion() {
        if (!Controller.IsGrounded) StateMachine.ChangeState(Factory.Fall);
        else if (Controller.HasMoveInput) StateMachine.ChangeState(Factory.Run);
        else StateMachine.ChangeState(Factory.Idle);
    }
}