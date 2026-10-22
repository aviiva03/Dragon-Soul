using UnityEngine;

public class PlayerInteractor : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private float _interactRange = 2.5f;
    [SerializeField] private LayerMask _interactLayer;

    private IInteractable _currentInteractable;

    private void Update() {
        FindInteractable();

        if (_currentInteractable != null && Input.GetKeyDown(KeyCode.E)) {
            _currentInteractable.Interact();

            // Защита от null
            if (InteractionPromptUI.Instance != null)
                InteractionPromptUI.Instance.Hide();

            _currentInteractable = null;
        }
    }

    private void FindInteractable() {
        Vector3 origin = transform.position + Vector3.up * 1.0f; // Опустил высоту на уровень груди
        Vector3 direction = transform.forward;

        // Рисуем красный луч в окне Scene, чтобы видеть, куда он летит
        Debug.DrawRay(origin, direction * _interactRange, Color.red);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, _interactRange, _interactLayer)) {

            if (hit.collider.TryGetComponent(out IInteractable interactable)) {
                if (interactable.CanInteract()) {
                    if (_currentInteractable != interactable) {
                        _currentInteractable = interactable;
                        InteractionPromptUI.Instance.Show(interactable.GetPrompt());
                    }
                    return;
                }
            }
        }

        if (_currentInteractable != null) {
            _currentInteractable = null;
            InteractionPromptUI.Instance.Hide();
        }
    }
}