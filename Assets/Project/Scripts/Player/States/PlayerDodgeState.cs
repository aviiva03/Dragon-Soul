using UnityEngine;

public class PlayerDodgeState : PlayerState {

    private float _dodgeTimeLeft;

    public PlayerDodgeState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f)
        : base(c, m, f) { }

    public override void Enter() {
        _dodgeTimeLeft = Stats.DodgeDuration;

        Vector3 direction = Controller.GetCameraRelativeMoveDirection();
        if (direction.sqrMagnitude < 0.01f)
            direction = Controller.transform.forward;

        Controller.SetDodgeDirection(direction.normalized);
        Controller.ConsumeStamina(Stats.DodgeStaminaCost);
        Controller.MarkDodgeTime();

        Controller.GrantInvulnerability(Stats.DodgeInvincibilityDuration);
        Animator.PlayDodge();
    }

    public override void Update() {
        _dodgeTimeLeft -= Time.deltaTime;

        float t = _dodgeTimeLeft / Stats.DodgeDuration;
        float speed = Stats.DodgeSpeed * Mathf.SmoothStep(0f, 1f, t);
        Controller.MoveCharacter(Controller.DodgeDirection * speed * Time.deltaTime);

        if (Controller.WantsToJump && Controller.IsGrounded) {
            StateMachine.ChangeState(Factory.Jump);
            return;
        }

        if (_dodgeTimeLeft <= 0f) {
            if (Controller.IsGrounded)
                StateMachine.ChangeState(Controller.HasMoveInput ? Factory.Run : Factory.Idle);
            else
                StateMachine.ChangeState(Factory.Fall);
        }
    }
}