using System.Collections;
using UnityEngine;
using Cinemachine;
using TMPro;
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
    [Tooltip("Точка, куда телепортируется игрок для разговора (лицом к дракону)")]
    [SerializeField] private Transform _playerTeleportPoint; // НОВОЕ ПОЛЕ

    private bool _isPlaying = false;

    public override void Execute() {
        if (_isPlaying) return;
        _isPlaying = true;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null) {
            player.ForceStopForCinematic();
            player.enabled = false;
        }

        StartCoroutine(ChestRoutine(player));
    }

    private IEnumerator ChestRoutine(PlayerController player) {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 1. Анимация сундука (телепорт лучше делать до или во время нее)
        if (_chestAnimator != null && !string.IsNullOrEmpty(_openTriggerName)) {
            _chestAnimator.SetTrigger(_openTriggerName);
            yield return new WaitForSeconds(1.0f);
        } else {
            yield return new WaitForSeconds(1.0f);
        }

        // 2. Артефакт
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

        // 3. Включаем дракона Бию
        if (_biuModel != null) {
            _biuModel.SetActive(true);
        }

        // 4. Запускаем диалог (и передаем точку телепорта!)
        if (DialogueManager.Instance != null && _memoryDialogue != null) {
            TalkingAnimator[] characters = null;
            if (_biuAnimator != null) {
                characters = new TalkingAnimator[] { _biuAnimator };
            }

            // Передаем _playerTeleportPoint в 4-й параметр!
            DialogueManager.Instance.StartDialogue(_memoryDialogue, _memoryCamera, null, _playerTeleportPoint, characters);

            while (DialogueManager.Instance.IsDialogueActive) {
                yield return null;
            }
        }

        // 5. Выключаем дракона
        if (_biuModel != null) {
            _biuModel.SetActive(false);
        }

        // 6. Возвращаем управление
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (player != null) player.enabled = true;

        _isPlaying = false;
    }
}