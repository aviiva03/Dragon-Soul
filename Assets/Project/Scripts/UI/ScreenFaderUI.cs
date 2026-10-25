using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class ScreenFaderUI : MonoBehaviour {
    public static ScreenFaderUI Instance { get; private set; }

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _text;

    [Header("Settings")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _holdDuration = 1.0f;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        if (_canvasGroup != null) _canvasGroup.alpha = 0f;
        if (_text != null) _text.gameObject.SetActive(false);
    }

    public void PlayFade(string text, Action onScreenFullyBlack = null) {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(text, onScreenFullyBlack));
    }

    private IEnumerator FadeRoutine(string text, Action onScreenFullyBlack) {
        yield return FadeAlpha(0f, 1f, _fadeDuration);

        if (!string.IsNullOrEmpty(text) && _text != null) {
            _text.text = text;
            _text.gameObject.SetActive(true);
        }

        onScreenFullyBlack?.Invoke();

        yield return new WaitForSecondsRealtime(_holdDuration);

        if (_text != null) _text.gameObject.SetActive(false);

        yield return FadeAlpha(1f, 0f, _fadeDuration);
    }

    private IEnumerator FadeAlpha(float from, float to, float duration) {
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        _canvasGroup.alpha = to;
    }
}