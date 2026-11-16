using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossHealthBarUI : MonoBehaviour {
    [Header("UI Elements")]
    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _bossNameText;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Settings")]
    [SerializeField] private float _fadeSpeed = 5f;
    [SerializeField] private float _showDistance = 15f; // Дистанция, на которой появляется плашка

    private Health _bossHealth;
    private Transform _bossTransform;
    private Transform _playerTransform;
    private bool _isVisible = false;
    private Coroutine _fadeRoutine;

    public void Setup(Health health, string bossName, Transform bossTransform, Transform playerTransform) {
        _bossHealth = health;
        _bossNameText.text = bossName;
        _bossTransform = bossTransform;
        _playerTransform = playerTransform;

        _bossHealth.OnHealthChanged += UpdateBar;
        _bossHealth.OnDied += HandleDeath;

        UpdateBar(_bossHealth.Current, _bossHealth.Max);

        _canvasGroup.alpha = 0f;
        _isVisible = false;
    }

    private void OnDestroy() {
        if (_bossHealth != null) {
            _bossHealth.OnHealthChanged -= UpdateBar;
            _bossHealth.OnDied -= HandleDeath;
        }
    }

    private void Update() {
        if (_bossTransform == null || _playerTransform == null) return;

        // Проверяем дистанцию до СВОЕГО босса
        float distance = Vector3.Distance(_playerTransform.position, _bossTransform.position);
        SetVisibility(distance <= _showDistance);
    }

    public void SetVisibility(bool visible) {
        if (_isVisible == visible) return;
        _isVisible = visible;

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeRoutine(visible ? 1f : 0f));
    }

    private IEnumerator FadeRoutine(float targetAlpha) {
        while (Mathf.Abs(_canvasGroup.alpha - targetAlpha) > 0.01f) {
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, targetAlpha, Time.unscaledDeltaTime * _fadeSpeed);
            yield return null;
        }
        _canvasGroup.alpha = targetAlpha;
    }

    private void UpdateBar(int current, int max) {
        float percent = max > 0 ? (float)current / max : 0f;
        _fillImage.fillAmount = percent;
    }

    private void HandleDeath() {
        SetVisibility(false);
        Destroy(gameObject, 1.5f);
    }
}