using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class ChestAction : InteractAction {

    [Header("Chest Settings")]
    [SerializeField] private Animator _chestAnimator;
    [SerializeField] private string _openTriggerName = "Open";

    [Header("Artifact UI")]
    [SerializeField] private CanvasGroup _artifactPanel;
    [SerializeField] private Image _artifactImage;
    [SerializeField] private Sprite _artifactSprite;
    [SerializeField] private float _artifactShowTime = 2.5f;

    [Header("Biyu Settings")]
    [SerializeField] private GameObject _biuModel;
    [SerializeField] private TalkingAnimator _biuAnimator;

    [Header("Dialogue")]
    [SerializeField] private DialogueData _memoryDialogue;
    [SerializeField] private CinemachineVirtualCamera _memoryCamera;

    [Header("Player Position")]
    [SerializeField] private Transform _playerTeleportPoint;

    private bool _isPlaying = false;

    public override void Execute() {
        if (_isPlaying) return;
        _isPlaying = true;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null) {
            player.ForceStopForCinematic();
            player.enabled = false;
        }

        // БЛОКИРУЕМ КАМЕРУ СРАЗУ, как только открыли сундук
        if (CameraLockManager.Instance != null) CameraLockManager.Instance.LockCamera();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(ChestRoutine(player));
    }

    private IEnumerator ChestRoutine(PlayerController player) {
        // 1. Анимация сундука
        if (_chestAnimator != null && !string.IsNullOrEmpty(_openTriggerName)) {
            _chestAnimator.SetTrigger(_openTriggerName);
            yield return new WaitForSeconds(1.0f);
        } else {
            yield return new WaitForSeconds(1.0f);
        }

        // 2. Артефакт (камера на игроке, но заблокирована)
        if (_artifactPanel != null) {
            if (_artifactImage != null && _artifactSprite != null) _artifactImage.sprite = _artifactSprite;

            _artifactPanel.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < 0.5f) {
                elapsed += Time.deltaTime;
                _artifactPanel.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            _artifactPanel.alpha = 1f;

            yield return new WaitForSeconds(_artifactShowTime);

            elapsed = 0f;
            while (elapsed < 0.5f) {
                elapsed += Time.deltaTime;
                _artifactPanel.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                yield return null;
            }
            _artifactPanel.gameObject.SetActive(false);
        }

        // 3. Телепорт к дракону
        if (_playerTeleportPoint != null && player != null) {
            var cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = _playerTeleportPoint.position;
            player.transform.rotation = _playerTeleportPoint.rotation;

            if (cc != null) cc.enabled = true;
        }

        // 4. Плавно переключаем на камеру дракона
        if (_memoryCamera != null && DialogueCameraController.Instance != null) {
            DialogueCameraController.Instance.SwitchToDialogueCamera(_memoryCamera, 1.0f);
            yield return new WaitForSeconds(1.0f);
        }

        // 5. Включаем дракона
        if (_biuModel != null) _biuModel.SetActive(true);

        // 6. Диалог
        if (DialogueManager.Instance != null && _memoryDialogue != null) {
            TalkingAnimator[] characters = null;
            if (_biuAnimator != null) characters = new TalkingAnimator[] { _biuAnimator };

            DialogueManager.Instance.StartDialogue(_memoryDialogue, _memoryCamera, null, null, characters);

            while (DialogueManager.Instance.IsDialogueActive) {
                yield return null;
            }
        }

        // 7. Выключаем дракона
        if (_biuModel != null) _biuModel.SetActive(false);

        // 8. Возвращаем камеру игроку
        if (DialogueCameraController.Instance != null) {
            DialogueCameraController.Instance.ReturnToPlayerCamera(1.0f);
            yield return new WaitForSeconds(1.0f);
        }

        // 9. ВЫДАЕМ СПОСОБНОСТЬ
        if (PlayerProgress.Instance != null) {
            PlayerProgress.Instance.GiveDragonClaw();
        }
        if (AbilityManager.Instance != null) {
            AbilityManager.Instance.UnlockAbility("Biyu_Attack");
        }

        var interactable = GetComponent<InteractableObject>();
        if (interactable != null) interactable.CompleteInteractionAndSave();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (player != null) player.enabled = true;

        // РАЗБЛОКИРУЕМ КАМЕРУ В САМОМ КОНЦЕ
        if (CameraLockManager.Instance != null) CameraLockManager.Instance.UnlockCamera();

        _isPlaying = false;
    }
}