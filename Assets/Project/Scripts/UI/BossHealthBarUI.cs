using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBarUI : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Health _bossHealth;
    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _bossNameText;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Settings")]
    [SerializeField] private string _bossName = "Wandering Undead";
    [SerializeField] private bool _hideWhenFull = false;
    [SerializeField] private float _fadeDuration = 0.4f;

    private void Start() {
        if (_bossHealth == null) {
            Debug.LogError($"[{name}] _bossHealth не назначен!");
            return;
        }

        _bossHealth.OnHealthChanged += UpdateBar;
        _bossHealth.OnDied += HandleDeath;

        if (_bossNameText != null)
            _bossNameText.text = _bossName;

        // Начальное значение
        UpdateBar(_bossHealth.Current, _bossHealth.Max);

        // Опционально — скрыть, пока полное HP
        if (_hideWhenFull && _canvasGroup != null)
            _canvasGroup.alpha = 0f;
        else if (_canvasGroup != null)
            _canvasGroup.alpha = 1f;
    }

    private void OnDestroy() {
        if (_bossHealth != null) {
            _bossHealth.OnHealthChanged -= UpdateBar;
            _bossHealth.OnDied -= HandleDeath;
        }
    }

    private void UpdateBar(int current, int max) {
        if (_fillImage == null) return;

        float percent = max > 0 ? (float)current / max : 0f;
        _fillImage.fillAmount = percent;

        // Показать при первом уроне
        if (_hideWhenFull && _canvasGroup != null && percent < 1f)
            _canvasGroup.alpha = 1f;
    }

    private void HandleDeath() {
        if (_canvasGroup != null)
            StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut() {
        float elapsed = 0f;
        float startAlpha = _canvasGroup.alpha;

        while (elapsed < _fadeDuration) {
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
    }
}