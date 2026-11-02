using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable {
    [Header("Settings")]
    [SerializeField] private string _promptText = "Взаимодействовать";
    [SerializeField] private bool _interactOnlyOnce = true;

    [Header("Persistence")]
    [Tooltip("Уникальный ID (например, Chest_01). Если пусто — не сохраняется.")]
    [SerializeField] private string _uniqueID;

    [Tooltip("Удалить объект полностью после завершения взаимодействия? (Если нет — просто выключит коллайдер)")]
    [SerializeField] private bool _destroyObjectAfterInteraction = false;

    [Header("Action")]
    [SerializeField] private InteractAction _action;

    private bool _hasInteracted = false;

    private void Start() {
        if (!string.IsNullOrEmpty(_uniqueID) && PlayerProgress.Instance != null) {
            if (PlayerProgress.Instance.IsObjectDestroyed(_uniqueID)) {
                if (_destroyObjectAfterInteraction) {
                    Destroy(gameObject);
                } else {
                    DisableInteraction();
                }
            }
        }
    }

    public string GetPrompt() => _promptText;

    public bool CanInteract() {
        if (_interactOnlyOnce && _hasInteracted) return false;
        return true;
    }

    public void Interact() {
        if (!CanInteract()) return;

        _hasInteracted = true;
        DisableColliders(); // Выключаем ТОЛЬКО триггер на этом объекте

        if (_action != null) {
            _action.Execute();
        }
    }

    public void CompleteInteractionAndSave() {
        if (!string.IsNullOrEmpty(_uniqueID) && PlayerProgress.Instance != null) {
            PlayerProgress.Instance.RegisterDestroyedObject(_uniqueID);
        }

        if (_destroyObjectAfterInteraction) {
            Destroy(gameObject);
        }
    }

    private void DisableInteraction() {
        DisableColliders();
        _hasInteracted = true;
    }

    private void DisableColliders() {
        var col = GetComponent<Collider>();
        if (col != null) {
            col.enabled = false;
        }
    }
}