using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBarUI : MonoBehaviour {

    [SerializeField] private Health _health;
    [SerializeField] private Image _fillImage;

    private void Start() {
        if (_health == null) {
            Debug.LogError($"[{name}] _health не назначен!");
            return;
        }

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
        // Поворачиваем плашку лицом к камере (Billboard)
        if (Camera.main != null) {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }

    private void UpdateBar(int current, int max) {
        if (_fillImage == null) return;
        _fillImage.fillAmount = max > 0 ? (float)current / max : 0f;
    }

    private void HandleDeath() {
        // Скрываем плашку при смерти, чтобы не висела над трупом
        gameObject.SetActive(false);
    }
}