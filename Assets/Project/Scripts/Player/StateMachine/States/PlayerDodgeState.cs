using UnityEngine;

public class PlayerDodgeState : PlayerState {

    private float _dodgeTimeLeft;

    public PlayerDodgeState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f)
        : base(c, m, f) { }

    public override void Enter() {
        _dodgeTimeLeft = Stats.DodgeDuration;

        // Направление рывка
        Vector3 direction = Controller.GetCameraRelativeMoveDirection();
        if (direction.sqrMagnitude < 0.01f)
            direction = Controller.transform.forward;

        Controller.SetDodgeDirection(direction.normalized);
        Controller.ConsumeStamina(Stats.DodgeStaminaCost);
        Controller.MarkDodgeTime();

        // i-frames
        Controller.GrantInvulnerability(Stats.DodgeInvincibilityDuration);

        Animator.PlayDodge();

        Debug.Log($"[DODGE] i-frames granted: {Stats.DodgeInvincibilityDuration}s");
    }

    public override void Update() {
        _dodgeTimeLeft -= Time.deltaTime;

        float t = _dodgeTimeLeft / Stats.DodgeDuration;
        float speed = Stats.DodgeSpeed * Mathf.SmoothStep(0f, 1f, t);
        Controller.MoveCharacter(Controller.DodgeDirection * speed * Time.deltaTime);

        // Прыжок отменяет рывок (на земле)
        if (Controller.WantsToJump && Controller.IsGrounded) {
            StateMachine.ChangeState(Factory.Jump);
            return;
        }

        if (_dodgeTimeLeft <= 0f) {
            if (Controller.IsGrounded)
                StateMachine.ChangeState(
                    Controller.HasMoveInput ? Factory.Run : Factory.Idle);
            else
                StateMachine.ChangeState(Factory.Fall);
        }
    }
}