public class PlayerStateFactory {

    public PlayerIdleState Idle { get; }
    public PlayerRunState Run { get; }
    public PlayerJumpState Jump { get; }
    public PlayerFallState Fall { get; }
    public PlayerDodgeState Dodge { get; }
    public PlayerLandState Land { get; }
    public PlayerLightAttackState LightAttack { get; }
    public PlayerHeavyAttackState HeavyAttack { get; }

    public PlayerStateFactory(PlayerController controller, PlayerStateMachine machine) {
        Idle = new PlayerIdleState(controller, machine, this);
        Run = new PlayerRunState(controller, machine, this);
        Jump = new PlayerJumpState(controller, machine, this);
        Fall = new PlayerFallState(controller, machine, this);
        Dodge = new PlayerDodgeState(controller, machine, this);
        Land = new PlayerLandState(controller, machine, this);
        LightAttack = new PlayerLightAttackState(controller, machine, this);
        HeavyAttack = new PlayerHeavyAttackState(controller, machine, this);
    }
}