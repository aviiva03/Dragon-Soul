public interface IInteractable {
    string GetPrompt(); // Текст подсказки ("Подобрать", "Открыть")
    bool CanInteract(); // Можно ли сейчас это сделать?
    void Interact();    // Само действие
}