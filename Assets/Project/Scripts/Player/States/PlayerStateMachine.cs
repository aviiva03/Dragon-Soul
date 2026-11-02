public class PlayerStateMachine {
    public PlayerState CurrentState { get; private set; }

    public void Initialize(PlayerState startingState) {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(PlayerState newState) {
        if (newState == CurrentState) return;
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update() {
        CurrentState.Update();
    }
}