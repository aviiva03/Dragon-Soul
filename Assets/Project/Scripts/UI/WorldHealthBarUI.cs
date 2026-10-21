using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBarUI : MonoBehaviour {

    [SerializeField] private Health _health;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Transform _cameraTransform;

    private void Start() {
        if (_health == null) {
            Debug.LogError($"[{name}] _health не назначен!");
            return;
        }

        if (_cameraTransform == null && Camera.main != null)
            _cameraTransform = Camera.main.transform;

        _health.OnHealthChanged += UpdateBar;
        _health.OnDied += HandleDeath;

        // Инициализация — начальное значение
        if (_fillImage != null && _health.Max > 0)
            _fillImage.fillAmount = (float)_health.Current / _health.Max;
    }

    private void OnDestroy() {
        if (_health != null) {
            _health.OnHealthChanged -= UpdateBar;
            _health.OnDied -= HandleDeath;
        }
    }

    private void LateUpdate() {
        // Поворачиваем плашку к камере (billboard)
        if (_cameraTransform != null) {
            transform.LookAt(
                transform.position + _cameraTransform.forward,
                _cameraTransform.up);
        }
    }

    private void UpdateBar(int current, int max) {
        if (_fillImage == null) return;
        _fillImage.fillAmount = max > 0 ? (float)current / max : 0f;
    }

    private void HandleDeath() {
        // Скрыть плашку при смерти
        gameObject.SetActive(false);
    }
}