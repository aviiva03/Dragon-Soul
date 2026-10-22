using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable {
    [Header("Settings")]
    [SerializeField] private string _promptText = "Взаимодействовать";
    [SerializeField] private bool _interactOnlyOnce = true;

    [Header("Action")]
    [Tooltip("Какой скрипт запустить? Перетащи сюда объект со скриптом-наследником InteractAction")]
    [SerializeField] private InteractAction _action;

    private bool _hasInteracted = false;

    public string GetPrompt() => _promptText;

    public bool CanInteract() {
        if (_interactOnlyOnce && _hasInteracted) return false;
        return true;
    }

    public void Interact() {
        if (!CanInteract()) return;

        _hasInteracted = true;

        if (_action != null) {
            _action.Execute();
        } else {
            Debug.LogWarning($"[{name}] Не назначен InteractAction!");
        }
    }
}