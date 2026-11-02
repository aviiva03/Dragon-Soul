using UnityEngine;

public class PlayerLandState : PlayerState {

    private float _landTime;
    private bool _hardLanding;

    public PlayerLandState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f)
        : base(c, m, f) { }

    public override void Enter() {
        _hardLanding = Controller.LastFallVelocity < -10f;
        _landTime = _hardLanding ? 0.25f : 0.12f;

        Controller.MoveDirection = Vector3.zero;
        Controller.ApplyMovement();
    }

    public override void Update() {
        _landTime -= Time.deltaTime;

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

        if (_landTime > 0f) return;

        if (Controller.WantsToJump && Controller.IsGrounded) {
            StateMachine.ChangeState(Factory.Jump);
            return;
        }
        if (Controller.HasMoveInput)
            StateMachine.ChangeState(Factory.Run);
        else
            StateMachine.ChangeState(Factory.Idle);
    }
}