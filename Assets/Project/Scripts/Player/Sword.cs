using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour {

    [Header("References")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _appearSound;
    [SerializeField] private AudioClip _disappearSound;

    [Header("Fade Settings")]
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private float _idleTimeToDisappear = 3f;

    private Renderer[] _renderers;
    private MaterialPropertyBlock _mpb;
    private Coroutine _fadeRoutine;
    private Coroutine _idleTimerRoutine;

    private int _colorPropertyId = -1;

    private void Awake() {
        _renderers = GetComponentsInChildren<Renderer>();
        _mpb = new MaterialPropertyBlock();

        if (_renderers.Length > 0) {
            Material mat = _renderers[0].sharedMaterial;
            if (mat.HasProperty("_BaseColor")) _colorPropertyId = Shader.PropertyToID("_BaseColor");
            else if (mat.HasProperty("_Color")) _colorPropertyId = Shader.PropertyToID("_Color");
            else Debug.LogError("[Sword] Не удалось найти свойство цвета в материале! (Нужен _BaseColor или _Color)");
        }

        SetAlpha(0f);
    }

    public void Appear() {
        if (_idleTimerRoutine != null) StopCoroutine(_idleTimerRoutine);

        if (_audioSource != null && _appearSound != null)
            _audioSource.PlayOneShot(_appearSound);

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeRoutine(1f));
    }

    public void ShowOnPickup() {
        Appear();
        AttackFinished();
    }

    public void AttackFinished() {
        if (_idleTimerRoutine != null) StopCoroutine(_idleTimerRoutine);
        _idleTimerRoutine = StartCoroutine(IdleTimerRoutine());
    }

    private IEnumerator IdleTimerRoutine() {
        yield return new WaitForSeconds(_idleTimeToDisappear);
        Disappear();
    }

    public void Disappear() {
        if (_audioSource != null && _disappearSound != null)
            _audioSource.PlayOneShot(_disappearSound);

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeRoutine(0f));
    }

    private IEnumerator FadeRoutine(float targetAlpha) {
        if (_colorPropertyId == -1) {
            gameObject.SetActive(targetAlpha > 0.5f);
            yield break;
        }

        float startAlpha = GetAlpha();
        float elapsed = 0f;

        while (elapsed < _fadeDuration) {
            elapsed += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / _fadeDuration);
            SetAlpha(currentAlpha);
            yield return null;
        }

        SetAlpha(targetAlpha);
        _fadeRoutine = null;
    }

    private float GetAlpha() {
        if (_renderers.Length > 0 && _colorPropertyId != -1) {
            _renderers[0].GetPropertyBlock(_mpb);
            return _mpb.GetColor(_colorPropertyId).a;
        }
        return 0f;
    }

    private void SetAlpha(float alpha) {
        if (_colorPropertyId == -1) return;

        foreach (var r in _renderers) {
            if (r == null) continue;

            r.GetPropertyBlock(_mpb);
            Color c = r.sharedMaterial.GetColor(_colorPropertyId);
            c.a = alpha;
            _mpb.SetColor(_colorPropertyId, c);
            r.SetPropertyBlock(_mpb);
        }
    }
}