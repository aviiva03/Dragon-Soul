using Cinemachine;
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

        // »нициализаци€ Ч начальное значение
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
        Transform targetCamera = null;

        var brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null && brain.ActiveVirtualCamera != null) {
            targetCamera = brain.ActiveVirtualCamera.VirtualCameraGameObject.transform;
        } else if (Camera.main != null) {
            targetCamera = Camera.main.transform;
        }

        if (targetCamera != null) {
            transform.rotation = targetCamera.rotation;
        }
    }

    private void UpdateBar(int current, int max) {
        if (_fillImage == null) return;
        _fillImage.fillAmount = max > 0 ? (float)current / max : 0f;
    }

    private void HandleDeath() {
        // —крыть плашку при смерти
        gameObject.SetActive(false);
    }
}