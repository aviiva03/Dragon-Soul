using System.Collections;
using UnityEngine;
using Cinemachine;
using TMPro;

public class SwordShardsAction : InteractAction {

    [Header("Scene Objects")]
    [SerializeField] private GameObject _pastSceneRoot;
    [SerializeField] private CinemachineVirtualCamera _pastCamera;

    [Header("Sword Models")]
    [SerializeField] private GameObject _wholeSwordModel;
    [SerializeField] private GameObject _shardsModel;

    [Header("Characters")]
    [SerializeField] private TalkingAnimator _fatherAnim;
    [SerializeField] private TalkingAnimator _dashenYoungAnim;

    [Header("UI")]
    [SerializeField] private CanvasGroup _fadeGroup;
    [SerializeField] private CanvasGroup _filterGroup;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private CanvasGroup _hudGroup;
    [SerializeField] private GameObject _abilityBarUI;
    [SerializeField] private DialogueData _memoryDialogue;

    [Header("Settings")]
    [SerializeField] private float _fadeTime = 1f;
    [SerializeField] private float _titleHoldTime = 2f;

    private bool _isPlaying = false;
    private CinemachineBrain _brain;

    public override void Execute() {
        if (_isPlaying) return;
        _isPlaying = true;

        PlayerController player = FindObjectOfType<PlayerController>();

        if (player != null) {
            player.ForceStopForCinematic();
            player.enabled = false;
        }

        if (Camera.main != null) _brain = Camera.main.GetComponent<CinemachineBrain>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(CinematicSequence(player));
    }

    private IEnumerator CinematicSequence(PlayerController player) {
        SetPlayerVisible(player, false);

        if (_hudGroup != null) _hudGroup.alpha = 0f;
        if (_abilityBarUI != null) _abilityBarUI.SetActive(false);

        // 1. Затемняем экран
        yield return FadeUI(_fadeGroup, 0f, 1f, _fadeTime);

        if (_pastSceneRoot != null) _pastSceneRoot.SetActive(true);
        if (_wholeSwordModel != null) _wholeSwordModel.SetActive(true);
        if (_shardsModel != null) _shardsModel.SetActive(false);

        if (_filterGroup != null) {
            _filterGroup.gameObject.SetActive(true);
            _filterGroup.alpha = 1f;
        }

        // Включаем камеру прошлого
        if (DialogueCameraController.Instance != null) {
            DialogueCameraController.Instance.SetCameraInstant(_pastCamera);
        }

        yield return null;

        // --- ВОТ ГЛАВНОЕ: ОТКЛЮЧАЕМ BRAIN, ЧТОБЫ ОН НЕ ДВИГАЛСЯ ---
        if (_brain != null) _brain.enabled = false;

        _titleText.text = "Прошлое...";
        _titleText.gameObject.SetActive(true);

        yield return FadeUI(_fadeGroup, 1f, 0f, _fadeTime);
        yield return new WaitForSeconds(_titleHoldTime);

        _titleText.gameObject.SetActive(false);

        // 2. Диалог (камера зафиксирована)
        TalkingAnimator[] talkers = new TalkingAnimator[] { _fatherAnim, _dashenYoungAnim };
        DialogueManager.Instance.StartCinematicDialogue(_memoryDialogue, talkers);

        while (DialogueManager.Instance.IsDialogueActive) {
            yield return null;
        }

        if (_pastSceneRoot != null) _pastSceneRoot.SetActive(false);

        // 3. Пазл (камера зафиксирована, Brain выключен)
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

        // 4. Затемняем экран для возврата
        yield return FadeUI(_fadeGroup, 0f, 1f, _fadeTime);

        if (_wholeSwordModel != null) _wholeSwordModel.SetActive(false);
        if (_shardsModel != null) _shardsModel.SetActive(true);
        if (_filterGroup != null) _filterGroup.gameObject.SetActive(false);

        // 5. Включаем Brain обратно и возвращаем камеру
        if (_brain != null) _brain.enabled = true;

        if (DialogueCameraController.Instance != null) {
            DialogueCameraController.Instance.ReturnToPlayerCamera(0f);
        }

        // --- НОВОЕ: ВКЛЮЧАЕМ ИГРОКА И ЖДЕМ КАДР ---
        SetPlayerVisible(player, true);
        if (player != null) {
            player.enabled = true;
            if (player.Sword != null) {
                player.Sword.enabled = true;
                player.Sword.ShowOnPickup();
            }
        }

        yield return null; // Ждем кадр, чтобы камера встала на место и отрендерила игрока
        yield return null; // Еще один кадр на всякий случай

        _titleText.text = "Возвращение в настоящее...";
        _titleText.gameObject.SetActive(true);

        // 6. Открываем экран (теперь игрок уже виден)
        yield return FadeUI(_fadeGroup, 1f, 0f, _fadeTime);
        yield return new WaitForSeconds(_titleHoldTime);

        _titleText.gameObject.SetActive(false);

        if (_hudGroup != null) _hudGroup.alpha = 1f;
        if (_abilityBarUI != null) _abilityBarUI.SetActive(true);

        if (PlayerProgress.Instance != null) PlayerProgress.Instance.GiveSword();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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