using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DetectionScreenUI : MonoBehaviour {

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _holdDuration = 1.0f;

    private void Awake() {
        if (_canvasGroup != null)
            _canvasGroup.alpha = 0f;
    }

    private void Start() {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerDetected += Show;
    }

    private void OnDestroy() {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerDetected -= Show;
    }

    private void Show() {
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine() {
        yield return Fade(0f, 1f, _fadeDuration);
        yield return new WaitForSeconds(_holdDuration);
        yield return Fade(1f, 0f, _fadeDuration);
    }

    private IEnumerator Fade(float from, float to, float duration) {
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        _canvasGroup.alpha = to;
    }
}
