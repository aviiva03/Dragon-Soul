using System.Collections;
using UnityEngine;
using Cinemachine;

public class DialogueManager : MonoBehaviour {
    public static DialogueManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DialogueUI _dialogueUI;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CanvasGroup _hudCanvasGroup;
    [SerializeField] private CinemachineFreeLook _playerCamera;

    [Header("Settings")]
    [SerializeField] private float _hudFadeDuration = 0.3f;

    public bool IsDialogueActive { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // Добавили Transform playerTeleportPoint
    public void StartDialogue(DialogueData dialogue, CinemachineVirtualCamera dialogueCamera, GameObject objectToDestroy = null, Transform playerTeleportPoint = null) {
        if (IsDialogueActive) return;
        StartCoroutine(DialogueRoutine(dialogue, dialogueCamera, objectToDestroy, playerTeleportPoint));
    }

    private IEnumerator DialogueRoutine(DialogueData dialogue, CinemachineVirtualCamera dialogueCamera, GameObject objectToDestroy, Transform playerTeleportPoint) {
        IsDialogueActive = true;

        // --- 1. СОХРАНЯЕМ СТАРУЮ ПОЗИЦИЮ И ТЕЛЕПОРТИРУЕМ ---
        Vector3 originalPosition = Vector3.zero;
        Quaternion originalRotation = Quaternion.identity;
        CharacterController cc = null;

        if (_playerController != null) {
            _playerController.ForceStopForCinematic();
            _playerController.enabled = false;

            // Запоминаем, где игрок был
            originalPosition = _playerController.transform.position;
            originalRotation = _playerController.transform.rotation;

            // Если указана точка телепорта — переносим игрока
            if (playerTeleportPoint != null) {
                cc = _playerController.GetComponent<CharacterController>();

                // Отключаем коллайдер игрока, чтобы телепорт сработал
                if (cc != null) cc.enabled = false;

                _playerController.transform.position = playerTeleportPoint.position;
                _playerController.transform.rotation = playerTeleportPoint.rotation;

                if (cc != null) cc.enabled = true;
            }
        }

        // 2. Скрываем HUD
        yield return FadeHUD(1f, 0f, _hudFadeDuration);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. Включаем камеру диалога
        if (dialogueCamera != null) {
            Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = 1.5f;
            dialogueCamera.Priority = 20;
            _playerCamera.Priority = 10;
            yield return new WaitForSeconds(1.5f); // Ждем полета
        }

        // 4. Проходим по узлам
        string currentNodeId = dialogue.startNodeId;

        while (!string.IsNullOrEmpty(currentNodeId) && currentNodeId != "END") {
            DialogueNode node = dialogue.GetNode(currentNodeId);
            if (node == null) {
                Debug.LogError($"Узел {currentNodeId} не найден!");
                break;
            }

            yield return _dialogueUI.ShowNodeRoutine(node);

            if (node.choices != null && node.choices.Count > 0) {
                int choiceIndex = _dialogueUI.GetSelectedChoiceIndex();
                currentNodeId = node.choices[choiceIndex].nextNodeId;
            } else {
                currentNodeId = node.nextNodeId;
            }
        }

        // 5. Выключаем камеру диалога
        if (dialogueCamera != null) {
            dialogueCamera.Priority = 5;
            _playerCamera.Priority = 10;
            yield return new WaitForSeconds(1.5f);
        }

        // --- 6. ВОЗВРАЩАЕМ ИГРОКА НА МЕСТО ---
        if (_playerController != null) {
            if (playerTeleportPoint != null && cc != null) {
                cc.enabled = false;
                _playerController.transform.position = originalPosition;
                _playerController.transform.rotation = originalRotation;
                cc.enabled = true;
            }
            _playerController.enabled = true;
        }

        yield return FadeHUD(0f, 1f, _hudFadeDuration);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsDialogueActive = false;

        if (objectToDestroy != null) {
            Destroy(objectToDestroy);
        }
    }

    private IEnumerator FadeHUD(float from, float to, float duration) {
        if (_hudCanvasGroup == null) yield break;
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            _hudCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        _hudCanvasGroup.alpha = to;
    }
}