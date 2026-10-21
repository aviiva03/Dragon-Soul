using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Health _health;
    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _text;

    [Header("Settings")]
    [SerializeField] private bool _showText = true;

    private void Start() {
        if (_health == null) {
            Debug.LogError($"[{name}] _health не назначен!");
            return;
        }

        _health.OnHealthChanged += UpdateBar;
        _health.OnDied += HandleDeath;

        // Начальное значение
        UpdateBar(_health.Current, _health.Max);
    }

    private void OnDestroy() {
        if (_health != null) {
            _health.OnHealthChanged -= UpdateBar;
            _health.OnDied -= HandleDeath;
        }
    }

    private void UpdateBar(int current, int max) {
        if (_fillImage != null)
            _fillImage.fillAmount = max > 0 ? (float)current / max : 0f;

        if (_showText && _text != null)
            _text.text = $"{current} / {max}";
    }

    private void HandleDeath() {
        Debug.Log("Игрок умер!");
    }
}