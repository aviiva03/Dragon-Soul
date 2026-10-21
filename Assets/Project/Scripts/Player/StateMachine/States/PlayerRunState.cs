using UnityEngine;

public class PlayerRunState : PlayerState {

    public PlayerRunState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f)
        : base(c, m, f) { }

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
        if (!Controller.HasMoveInput) {
            StateMachine.ChangeState(Factory.Idle);
            return;
        }

        // Спринт: тратим стамину только здесь
        if (Controller.WantsToSprint) {
            Controller.ConsumeStamina(Stats.StaminaDrainPerSec * Time.deltaTime);
        }

        Controller.ApplyMovement();
    }
}