using UnityEngine;

public class PlayerIdleState : PlayerState {

    public PlayerIdleState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f)
        : base(c, m, f) { }

    public override void Enter() {
        Controller.MoveDirection = Vector3.zero;
        Controller.ApplyMovement();
        Animator.SetSpeed(0f);
    }

    public override void Update() {
        if (Controller.WantsLightAttack) {
            StateMachine.ChangeState(Factory.LightAttack);
            return;
        }
        if (Controller.WantsHeavyAttack) {
            StateMachine.ChangeState(Factory.HeavyAttack);
            return;
        }
        if (Controller.WantsToDodge) {
            StateMachine.ChangeState(Factory.Dodge);
            return;
        }
        if (Controller.WantsToJump) {
            StateMachine.ChangeState(Factory.Jump);
            return;
        }
        if (!Controller.IsGrounded) {
            StateMachine.ChangeState(Factory.Fall);
            return;
        }
        if (Controller.HasMoveInput) {
            StateMachine.ChangeState(Factory.Run);
            return;
        }
    }
}