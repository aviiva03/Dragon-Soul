public interface IInteractable {
    string GetPrompt();
    bool CanInteract();
    void Interact();
}