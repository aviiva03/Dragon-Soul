using UnityEngine;

public class PlayerJumpState : PlayerState {

    public PlayerJumpState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f)
        : base(c, m, f) { }

    public override void Enter() {
        float jumpVelocity = Mathf.Sqrt(-2f * Stats.JumpHeight * Stats.Gravity);
        Controller.SetVerticalVelocity(jumpVelocity);
        Controller.ResetCoyoteTimer();
        Controller.ResetJumpBuffer();

        Animator.PlayJump();
    }

    public override void Update() {
        // Атака в воздухе не разрешена
        Controller.ApplyMovement();

        if (Controller.VerticalVelocity <= 0f) {
            StateMachine.ChangeState(Factory.Fall);
            return;
        }

        if (Controller.WantsToDodge) {
            StateMachine.ChangeState(Factory.Dodge);
        }
    }
}