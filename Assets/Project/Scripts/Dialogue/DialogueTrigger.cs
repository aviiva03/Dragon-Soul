using UnityEngine;
using Cinemachine;

public class DialogueTrigger : MonoBehaviour {

    [Header("Dialogue Settings")]
    [SerializeField] private DialogueData _dialogue;

    [Header("Camera Settings")]
    [SerializeField] private CinemachineVirtualCamera _dialogueCamera;

    [Header("Player Teleport")]
    [SerializeField] private Transform _playerTeleportPoint;

    [Header("Trigger Settings")]
    [SerializeField] private bool _triggerOnlyOnce = true;
    [SerializeField] private bool _destroyAfterDialogue = false;
    [SerializeField] private bool _destroyParent = false;

    [Header("Characters Animation")]
    [SerializeField] private TalkingAnimator[] _charactersToAnimate;

    private bool _hasTriggered = false;

    private void Start() {
        if (_dialogue == null) return;

        // При старте проверяем: если диалог уже был прослушан, отключаем триггер
        if (PlayerProgress.Instance != null && !string.IsNullOrEmpty(_dialogue.dialogueID)) {
            if (PlayerProgress.Instance.IsDialogueCompleted(_dialogue.dialogueID)) {
                if (_destroyAfterDialogue) {
                    Destroy(_destroyParent ? transform.root.gameObject : gameObject);
                } else {
                    var col = GetComponent<Collider>();
                    if (col != null) col.enabled = false;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        if (_dialogue == null) return;
        if (_triggerOnlyOnce && _hasTriggered) return;

        if (PlayerProgress.Instance != null && !string.IsNullOrEmpty(_dialogue.dialogueID)) {
            if (PlayerProgress.Instance.IsDialogueCompleted(_dialogue.dialogueID)) {
                var col = GetComponent<Collider>();
                if (col != null) col.enabled = false;
                return;
            }
        }

        if (DialogueManager.Instance != null) {
            _hasTriggered = true;

            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            GameObject objToDestroy = null;
            if (_destroyAfterDialogue) {
                objToDestroy = _destroyParent ? transform.root.gameObject : gameObject;
            }

            if (_dialogueCamera != null) {
                DialogueManager.Instance.StartDialogue(_dialogue, _dialogueCamera, objToDestroy, _playerTeleportPoint, _charactersToAnimate);
            } else {
                Debug.LogWarning($"[DialogueTrigger] У диалога {_dialogue.name} нет камеры!");
            }
        }
    }
}