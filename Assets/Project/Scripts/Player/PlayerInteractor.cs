using UnityEngine;

public class PlayerInteractor : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private float _interactRange = 2.5f;
    [SerializeField] private LayerMask _interactLayer;

    private IInteractable _currentInteractable;
    private PlayerController _controller;

    private void Start() {
        _controller = GetComponent<PlayerController>();
    }

    private void Update() {
        FindInteractable();

        if (_currentInteractable != null && _controller != null && _controller.WantsToInteract) {
            _currentInteractable.Interact();

            if (InteractionPromptUI.Instance != null)
                InteractionPromptUI.Instance.Hide();

            _currentInteractable = null;
        }
    }

    private void FindInteractable() {
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 direction = transform.forward;

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