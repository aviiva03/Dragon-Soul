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

    // --- 1. ОБЫЧНЫЙ ДИАЛОГ (С телепортом и плавной камерой) ---
    public void StartDialogue(DialogueData dialogue, CinemachineVirtualCamera dialogueCamera, GameObject objectToDestroy = null, Transform playerTeleportPoint = null) {
        if (IsDialogueActive) return;
        StartCoroutine(DialogueRoutine(dialogue, dialogueCamera, objectToDestroy, playerTeleportPoint, true));
    }

    // --- 2. ДИАЛОГ ДЛЯ КАТ-СЦЕНЫ (Без телепорта, без управления камерой, без HUD) ---
    public void StartCinematicDialogue(DialogueData dialogue) {
        if (IsDialogueActive) return;
        StartCoroutine(DialogueRoutine(dialogue, null, null, null, false));
    }

    private IEnumerator DialogueRoutine(DialogueData dialogue, CinemachineVirtualCamera dialogueCamera, GameObject objectToDestroy, Transform playerTeleportPoint, bool isStandardDialogue) {
        IsDialogueActive = true;

        if (_playerController != null) {
            _playerController.ForceStopForCinematic();
            _playerController.enabled = false;
        }

        // --- ТЕЛЕПОРТ (только для обычных диалогов) ---
        Vector3 originalPosition = Vector3.zero;
        Quaternion originalRotation = Quaternion.identity;
        CharacterController cc = null;

        if (isStandardDialogue && playerTeleportPoint != null && _playerController != null) {
            originalPosition = _playerController.transform.position;
            originalRotation = _playerController.transform.rotation;

            cc = _playerController.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            _playerController.transform.position = playerTeleportPoint.position;
            _playerController.transform.rotation = playerTeleportPoint.rotation;

            if (cc != null) cc.enabled = true;
        }

        // --- ЗАТЕМНЕНИЕ HUD (только для обычных) ---
        if (isStandardDialogue) {
            yield return FadeHUD(1f, 0f, _hudFadeDuration);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // --- КАМЕРА ---
        if (dialogueCamera != null) {
            CinemachineVirtualCamera[] allCameras = FindObjectsOfType<CinemachineVirtualCamera>();
            foreach (var cam in allCameras) cam.enabled = false;

            if (_playerCamera != null) _playerCamera.enabled = false;

            dialogueCamera.enabled = true;
            dialogueCamera.Priority = 50;

            // Для обычного диалога — плавно (1.5 сек). Для кат-сцены — мгновенно (0).
            Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = isStandardDialogue ? 1.5f : 0f;

            if (isStandardDialogue) yield return new WaitForSeconds(1.5f);
            else yield return null;
        }

        // --- УЗЛЫ ДИАЛОГА ---
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

        // --- ВОЗВРАТ КАМЕРЫ ---
        if (dialogueCamera != null) {
            dialogueCamera.enabled = false;

            if (_playerCamera != null) {
                _playerCamera.enabled = true;
                _playerCamera.Priority = 10;
            }

            Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = isStandardDialogue ? 1.5f : 0f;
            if (isStandardDialogue) yield return new WaitForSeconds(1.5f);
            else yield return null;
        }

        // --- ТЕЛЕПОРТ ОБРАТНО (только для обычных) ---
        if (isStandardDialogue && playerTeleportPoint != null && _playerController != null) {
            if (cc != null) cc.enabled = false;
            _playerController.transform.position = originalPosition;
            _playerController.transform.rotation = originalRotation;
            if (cc != null) cc.enabled = true;
        }

        if (_playerController != null) _playerController.enabled = true;

        // --- ВОЗВРАТ HUD ---
        // Для кат-сцены HUD включает сам SwordShardsAction, чтобы не было задержек
        if (isStandardDialogue) {
            yield return FadeHUD(0f, 1f, _hudFadeDuration);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        IsDialogueActive = false;
        if (objectToDestroy != null) Destroy(objectToDestroy);
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