using System.Collections;
using UnityEngine;
using Cinemachine;
using TMPro;

public class SwordShardsAction : InteractAction {

    [Header("Scene Objects")]
    [SerializeField] private GameObject _pastSceneRoot;
    [SerializeField] private CinemachineVirtualCamera _pastCamera;

    [Header("Characters")]
    [SerializeField] private TalkingAnimator _fatherAnim;
    [SerializeField] private TalkingAnimator _dashenYoungAnim;

    [Header("UI")]
    [SerializeField] private CanvasGroup _fadeGroup;
    [SerializeField] private CanvasGroup _filterGroup;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private CanvasGroup _hudGroup;
    [SerializeField] private DialogueData _memoryDialogue;

    [Header("Settings")]
    [SerializeField] private float _fadeTime = 1f;
    [SerializeField] private float _titleHoldTime = 2f;

    private bool _isPlaying = false;
    private CinemachineFreeLook _playerCam;
    private CinemachineBrain _brain;

    public override void Execute() {
        if (_isPlaying) return;
        _isPlaying = true;

        PlayerController player = FindObjectOfType<PlayerController>();

        if (player != null) {
            player.ForceStopForCinematic();
            player.enabled = false;
        }

        _playerCam = FindObjectOfType<CinemachineFreeLook>();

        if (Camera.main != null) {
            _brain = Camera.main.GetComponent<CinemachineBrain>();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(CinematicSequence(player));
    }

    private IEnumerator CinematicSequence(PlayerController player) {
        SetPlayerVisible(player, false);
        if (_hudGroup != null) _hudGroup.alpha = 0f;

        // 1. Затемняем экран (вход в прошлое)
        yield return FadeUI(_fadeGroup, 0f, 1f, _fadeTime);

        if (_pastSceneRoot != null) _pastSceneRoot.SetActive(true);
        if (_filterGroup != null) {
            _filterGroup.gameObject.SetActive(true);
            _filterGroup.alpha = 1f;
        }

        if (_playerCam != null) _playerCam.enabled = false;

        CinemachineVirtualCamera[] allCams = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var c in allCams) c.enabled = false;

        if (_pastCamera != null) {
            _pastCamera.enabled = true;
            _pastCamera.Priority = 50;
        }

        if (_brain != null && _pastCamera != null) {
            _brain.m_DefaultBlend.m_Time = 0f;
            _brain.enabled = true;

            yield return null;

            Camera.main.transform.position = _pastCamera.transform.position;
            Camera.main.transform.rotation = _pastCamera.transform.rotation;

            yield return null;
        }

        // Показываем текст "Прошлое..."
        _titleText.text = "Прошлое...";
        _titleText.gameObject.SetActive(true);

        yield return new WaitForSeconds(_titleHoldTime);

        _titleText.gameObject.SetActive(false);
        yield return FadeUI(_fadeGroup, 1f, 0f, _fadeTime);

        // Диалог в прошлом
        TalkingAnimator[] talkers = new TalkingAnimator[] { _fatherAnim, _dashenYoungAnim };
        DialogueManager.Instance.StartCinematicDialogue(_memoryDialogue, talkers);

        while (DialogueManager.Instance.IsDialogueActive) {
            yield return null;
        }

        if (_pastSceneRoot != null) _pastSceneRoot.SetActive(false);

        // Мини-игра (Пазл)
        bool puzzleFinished = false;
        if (SwordPuzzleManager.Instance != null) {
            SwordPuzzleManager.Instance.OnPuzzleCompleted += () => puzzleFinished = true;
            SwordPuzzleManager.Instance.StartPuzzle();
        } else {
            puzzleFinished = true;
        }

        while (!puzzleFinished) {
            yield return null;
        }

        // ==========================================
        // ЭТАП 4: ВОЗВРАЩЕНИЕ (С БЛОКИРОВКОЙ КАМЕРЫ)
        // ==========================================

        // 1. Сначала плавно затемняем экран (пока пазл закрылся, но фильтр еще висит)
        yield return FadeUI(_fadeGroup, 0f, 1f, _fadeTime);

        // 2. Пока экран черный — убираем фильтр и прошлое
        if (_filterGroup != null) _filterGroup.gameObject.SetActive(false);

        // 3. Блокируем мозг камеры, чтобы она не дернулась
        if (_brain != null) _brain.enabled = false;

        // 4. Меняем камеры (возвращаем камеру игрока)
        if (_pastCamera != null) _pastCamera.enabled = false;
        if (_playerCam != null) {
            _playerCam.enabled = true;
            _playerCam.Priority = 10;
        }

        // 5. Включаем мозг и принудительно ставим его на камеру игрока (экран ЧЕРНЫЙ, игрок не видит рывка)
        if (_brain != null) {
            _brain.m_DefaultBlend.m_Time = 0f;
            _brain.enabled = true;

            yield return null;

            if (_playerCam != null && Camera.main != null) {
                Camera.main.transform.position = _playerCam.transform.position;
                Camera.main.transform.rotation = _playerCam.transform.rotation;
            }

            yield return null;
        }

        // 6. Показываем текст "Возвращение..."
        _titleText.text = "Возвращение в настоящее...";
        _titleText.gameObject.SetActive(true);

        yield return new WaitForSeconds(_titleHoldTime);
        _titleText.gameObject.SetActive(false);

        // 7. Возвращаем игрока (он снова на сцене, но управление пока выключено)
        SetPlayerVisible(player, true);
        if (_hudGroup != null) _hudGroup.alpha = 1f;

        if (player != null) {
            player.enabled = true;
            if (player.Sword != null) {
                player.Sword.enabled = true;
                player.Sword.ShowOnPickup();
            }
        }

        if (PlayerProgress.Instance != null) PlayerProgress.Instance.GiveSword();

        // 8. ТОЛЬКО ТЕПЕРЬ убираем черный экран (игрок начинает видеть игру и может крутить камеру)
        yield return FadeUI(_fadeGroup, 1f, 0f, _fadeTime);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Завершаем взаимодействие
        var interactable = GetComponent<InteractableObject>();
        if (interactable != null) {
            interactable.CompleteInteractionAndSave();
        } else {
            Destroy(gameObject);
        }
    }

    private void SetPlayerVisible(PlayerController player, bool visible) {
        if (player == null) return;
        var renderers = player.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers) {
            if (r.gameObject.name != "Sword") r.enabled = visible;
        }
    }

    private IEnumerator FadeUI(CanvasGroup group, float from, float to, float duration) {
        if (group == null) yield break;
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        group.alpha = to;
    }
}