using System.Collections;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.UI;

public class SwordShardsAction : InteractAction {

    [Header("Scene Objects")]
    [SerializeField] private GameObject _pastSceneRoot;
    [SerializeField] private CinemachineVirtualCamera _pastCamera;

    [Header("UI")]
    [SerializeField] private CanvasGroup _fadeGroup;
    [SerializeField] private CanvasGroup _filterGroup;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private CanvasGroup _hudGroup;
    [SerializeField] private DialogueData _memoryDialogue;

    [Header("Settings")]
    [Tooltip("Время плавного затемнения экрана")]
    [SerializeField] private float _fadeTime = 1f;

    [Tooltip("Сколько секунд висит текст на экране (Прошлое / Возвращение)")]
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
        _brain = Camera.main.GetComponent<CinemachineBrain>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(CinematicSequence(player));
    }

    private IEnumerator CinematicSequence(PlayerController player) {
        // ==========================================
        // ЭТАП 1: ВХОД В ПРОШЛОЕ
        // ==========================================
        SetPlayerVisible(player, false);
        if (_hudGroup != null) _hudGroup.alpha = 0f;

        // 1. Плавно затемняем
        yield return FadeUI(_fadeGroup, 0f, 1f, _fadeTime);

        // 2. Пока экран черный — включаем сцену и фильтр
        if (_pastSceneRoot != null) _pastSceneRoot.SetActive(true);
        if (_filterGroup != null) {
            _filterGroup.gameObject.SetActive(true);
            _filterGroup.alpha = 1f;
        }

        // 3. МАГИЯ КАМЕРЫ
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

        // 4. Показываем текст
        _titleText.text = "Прошлое...";
        _titleText.gameObject.SetActive(true);

        // 5. Ждем
        yield return new WaitForSeconds(_titleHoldTime);

        _titleText.gameObject.SetActive(false);
        yield return FadeUI(_fadeGroup, 1f, 0f, _fadeTime);

        // ==========================================
        // ЭТАП 2: ДИАЛОГ
        // ==========================================
        DialogueManager.Instance.StartCinematicDialogue(_memoryDialogue);

        while (DialogueManager.Instance.IsDialogueActive) {
            yield return null;
        }

        // ==========================================
        // ЭТАП 3: МИНИ-ИГРА
        // ==========================================
        Debug.Log("[Shards] Запуск мини-игры...");

        bool puzzleFinished = false;

        // Включаем панель пазла
        if (SwordPuzzleManager.Instance != null) {
            // Подписываемся на событие окончания пазла
            SwordPuzzleManager.Instance.OnPuzzleCompleted += () => {
                puzzleFinished = true;
            };

            SwordPuzzleManager.Instance.StartPuzzle();
        } else {
            Debug.LogError("SwordPuzzleManager не найден на сцене!");
            puzzleFinished = true; // Пропускаем, если сломалось
        }

        // Ждем, пока игрок не соберет меч
        while (!puzzleFinished) {
            yield return null;
        }

        Debug.Log("[Shards] Мини-игра успешно завершена!");

        // ==========================================
        // ЭТАП 4: ВОЗВРАЩЕНИЕ
        // ==========================================
        yield return FadeUI(_fadeGroup, 0f, 1f, _fadeTime);

        if (_pastSceneRoot != null) _pastSceneRoot.SetActive(false);
        if (_filterGroup != null) _filterGroup.gameObject.SetActive(false);

        if (_pastCamera != null) _pastCamera.enabled = false;
        if (_playerCam != null) _playerCam.enabled = true;

        if (_playerCam != null) _playerCam.Priority = 10;

        if (_brain != null) {
            _brain.m_DefaultBlend.m_Time = 0f;
            _brain.enabled = true;

            yield return null;

            if (_playerCam != null) {
                Camera.main.transform.position = _playerCam.transform.position;
                Camera.main.transform.rotation = _playerCam.transform.rotation;
            }

            yield return null;
        }

        _titleText.text = "Возвращение в настоящее...";
        _titleText.gameObject.SetActive(true);

        yield return new WaitForSeconds(_titleHoldTime);
        _titleText.gameObject.SetActive(false);

        SetPlayerVisible(player, true);
        if (_hudGroup != null) _hudGroup.alpha = 1f;

        if (player != null) {
            player.enabled = true;
            if (player.Sword != null) player.Sword.enabled = true;
        }

        if (PlayerProgress.Instance != null) PlayerProgress.Instance.GiveSword();

        yield return FadeUI(_fadeGroup, 1f, 0f, _fadeTime);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Destroy(gameObject);
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