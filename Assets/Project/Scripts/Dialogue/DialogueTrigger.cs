using UnityEngine;
using Cinemachine;

public class DialogueTrigger : MonoBehaviour {

    [Header("Dialogue Settings")]
    [SerializeField] private DialogueData _dialogue;

    [Header("Camera Settings")]
    [SerializeField] private CinemachineVirtualCamera _dialogueCamera;

    [Header("Player Teleport")]
    [Tooltip("Куда телепортировать игрока? (Если пусто — телепорта не будет)")]
    [SerializeField] private Transform _playerTeleportPoint;

    [Header("Trigger Settings")]
    [SerializeField] private bool _triggerOnlyOnce = true;
    [SerializeField] private bool _destroyAfterDialogue = false;
    [SerializeField] private bool _destroyParent = false;

    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        if (_triggerOnlyOnce && _hasTriggered) return;

        if (DialogueManager.Instance != null) {
            _hasTriggered = true;
            GetComponent<Collider>().enabled = false;

            GameObject objToDestroy = null;
            if (_destroyAfterDialogue) {
                objToDestroy = _destroyParent ? transform.root.gameObject : gameObject;
            }

            // Передаем точку телепорта в менеджер
            DialogueManager.Instance.StartDialogue(_dialogue, _dialogueCamera, objToDestroy, _playerTeleportPoint);
        }
    }
}