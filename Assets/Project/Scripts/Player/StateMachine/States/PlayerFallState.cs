using UnityEngine;

public class PlayerFallState : PlayerState {

    public PlayerFallState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f)
        : base(c, m, f) { }

    public override void Enter() {
        // акнйхпсел онбнпнр опх оюдемхх
        Controller.CanRotate = false;
    }

    public override void Update() {
        Controller.ApplyMovement();

        if (Controller.IsGrounded) {
            Controller.SetLastFallVelocity(Controller.VerticalVelocity);
            StateMachine.ChangeState(Factory.Land);
            return;
        }

        if (Controller.WantsToDodge) {
            StateMachine.ChangeState(Factory.Dodge);
        }
    }

    public override void Exit() {
        Controller.CanRotate = true;
    }
}