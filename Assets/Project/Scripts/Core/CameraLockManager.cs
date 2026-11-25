using UnityEngine;
using Cinemachine;

public class CameraLockManager : MonoBehaviour {
    public static CameraLockManager Instance { get; private set; }

    [SerializeField] private CinemachineFreeLook _playerCamera;
    private int _lockCount = 0;

    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start() {
        if (_playerCamera == null) _playerCamera = FindObjectOfType<CinemachineFreeLook>();
    }

    public void LockCamera() {
        _lockCount++;
        if (_lockCount == 1) {
            // 1. Выключаем управление игрока
            if (_playerCamera != null) _playerCamera.enabled = false;

            // 2. ВЫКЛЮЧАЕМ ВСЕ ВИРТУАЛЬНЫЕ КАМЕРЫ В СЦЕНЕ
            CinemachineVirtualCamera[] allCams = FindObjectsOfType<CinemachineVirtualCamera>();
            foreach (var cam in allCams) {
                cam.enabled = false; // Вот это спасет от перебросов!
                cam.Priority = 5;
            }

            // 3. Камера игрока снова становится главной
            if (_playerCamera != null) {
                _playerCamera.Priority = 10;
            }
        }
    }

    public void UnlockCamera() {
        _lockCount--;
        if (_lockCount <= 0) {
            _lockCount = 0;
            if (_playerCamera != null) _playerCamera.enabled = true;
        }
    }
}