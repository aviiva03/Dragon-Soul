using UnityEngine;
using Cinemachine;

public class DialogueTrigger : MonoBehaviour {

    [Header("Dialogue Settings")]
    [SerializeField] private DialogueData _dialogue;

    [Header("Camera Settings")]
    [Tooltip("Камера для этого диалога (если нужна)")]
    [SerializeField] private CinemachineVirtualCamera _dialogueCamera;

    [Header("Player Teleport")]
    [Tooltip("Куда телепортировать игрока для кат-сцены? (Если пусто — телепорта не будет)")]
    [SerializeField] private Transform _playerTeleportPoint;

    [Header("Trigger Settings")]
    [SerializeField] private bool _triggerOnlyOnce = true;
    [SerializeField] private bool _destroyAfterDialogue = false;
    [SerializeField] private bool _destroyParent = false;

    [Header("Characters Animation")]
    [Tooltip("Перетащи сюда персонажей (Отца, Дэшэна), если они должны жестикулировать")]
    [SerializeField] private TalkingAnimator[] _charactersToAnimate;

    private bool _hasTriggered = false;

    private void Start() {
        // При старте проверяем: если диалог уже был прослушан в этой сессии сохранения,
        // то нам вообще не нужно, чтобы этот триггер срабатывал.
        if (PlayerProgress.Instance != null && !string.IsNullOrEmpty(_dialogue.dialogueID)) {
            if (PlayerProgress.Instance.IsDialogueCompleted(_dialogue.dialogueID)) {
                // Если объект должен был удалиться после диалога — удаляем его сразу
                if (_destroyAfterDialogue) {
                    Destroy(_destroyParent ? transform.root.gameObject : gameObject);
                } else {
                    // Иначе просто отключаем коллайдер
                    var col = GetComponent<Collider>();
                    if (col != null) col.enabled = false;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        if (_triggerOnlyOnce && _hasTriggered) return;

        if (DialogueManager.Instance != null) {
            _hasTriggered = true;

            // Отключаем коллайдер, чтобы игрок не вызвал триггер повторно, пока идет диалог
            GetComponent<Collider>().enabled = false;

            GameObject objToDestroy = null;
            if (_destroyAfterDialogue) {
                objToDestroy = _destroyParent ? transform.root.gameObject : gameObject;
            }

            // Запускаем диалог. Если камера не задана, можно добавить логику для кат-сцены
            if (_dialogueCamera != null) {
                DialogueManager.Instance.StartDialogue(
                    _dialogue,
                    _dialogueCamera,
                    objToDestroy,
                    _playerTeleportPoint,
                    _charactersToAnimate
                );
            } else {
                // Если это кат-сцена без стандартной камеры (например, через SwordShardsAction)
                // То лучше вызывать её отдельным скриптом, а не через триггер.
                Debug.LogWarning($"[DialogueTrigger] У диалога {_dialogue.name} нет камеры!");
            }
        }
    }
}