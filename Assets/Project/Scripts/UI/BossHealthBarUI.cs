using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBarUI : MonoBehaviour {
    [Header("UI Elements")]
    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _bossNameText;
    [SerializeField] private CanvasGroup _canvasGroup;

    private Health _bossHealth;

    public void Setup(Health health, string bossName) {
        _bossHealth = health;
        _bossNameText.text = bossName;

        _bossHealth.OnHealthChanged += UpdateBar;
        _bossHealth.OnDied += HandleDeath;

        UpdateBar(_bossHealth.Current, _bossHealth.Max);
        _canvasGroup.alpha = 0f; // Начинаем с прозрачного

        // Плавное появление
        StartCoroutine(FadeIn());
    }

    private void OnDestroy() {
        if (_bossHealth != null) {
            _bossHealth.OnHealthChanged -= UpdateBar;
            _bossHealth.OnDied -= HandleDeath;
        }
    }

    private void UpdateBar(int current, int max) {
        float percent = max > 0 ? (float)current / max : 0f;
        _fillImage.fillAmount = percent;
    }

    private void HandleDeath() {
        StartCoroutine(FadeOutAndDestroy());
    }

    private System.Collections.IEnumerator FadeIn() {
        float t = 0;
        while (t < 0.3f) {
            t += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / 0.3f);
            yield return null;
        }
        _canvasGroup.alpha = 1f;
    }

    private System.Collections.IEnumerator FadeOutAndDestroy() {
        float t = 0;
        while (t < 0.4f) {
            t += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / 0.4f);
            yield return null;
        }
        Destroy(gameObject);
    }
}