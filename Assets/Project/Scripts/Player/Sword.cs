using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour {

    [Header("Appearance")]
    [SerializeField] private float _appearDuration = 0.15f;
    [SerializeField] private float _disappearDuration = 0.2f;
    [SerializeField] private AnimationCurve _appearCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve _disappearCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _appearSound;
    [SerializeField] private AudioClip _disappearSound;

    private Vector3 _originalScale;
    private Coroutine _scaleRoutine;

    private void Awake() {
        _originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false); // Меч скрыт до получения
    }

    /// <summary>
    /// Показать меч с анимацией появления.
    /// </summary>
    public void Appear() {
        if (gameObject.activeSelf) return;

        gameObject.SetActive(true);

        if (_audioSource != null && _appearSound != null)
            _audioSource.PlayOneShot(_appearSound);

        if (_scaleRoutine != null)
            StopCoroutine(_scaleRoutine);

        _scaleRoutine = StartCoroutine(ScaleRoutine(
            Vector3.zero, _originalScale, _appearDuration, _appearCurve));
    }

    /// <summary>
    /// Скрыть меч с анимацией исчезновения.
    /// </summary>
    public void Disappear() {
        if (!gameObject.activeSelf) return;

        if (_audioSource != null && _disappearSound != null)
            _audioSource.PlayOneShot(_disappearSound);

        if (_scaleRoutine != null)
            StopCoroutine(_scaleRoutine);

        _scaleRoutine = StartCoroutine(ScaleRoutine(
            transform.localScale, Vector3.zero, _disappearDuration, _disappearCurve,
            hideOnComplete: true));
    }

    private IEnumerator ScaleRoutine(Vector3 from, Vector3 to, float duration,
                                    AnimationCurve curve, bool hideOnComplete = false) {
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curved = curve.Evaluate(t);
            transform.localScale = Vector3.LerpUnclamped(from, to, curved);
            yield return null;
        }

        transform.localScale = to;

        if (hideOnComplete)
            gameObject.SetActive(false);

        _scaleRoutine = null;
    }
}