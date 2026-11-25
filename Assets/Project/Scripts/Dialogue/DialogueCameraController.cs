using UnityEngine;
using Cinemachine;

public class DialogueCameraController : MonoBehaviour {
    public static DialogueCameraController Instance { get; private set; }

    [SerializeField] private CinemachineFreeLook _playerCamera;
    private CinemachineVirtualCamera _activeDialogueCamera;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start() {
        if (_playerCamera == null) _playerCamera = FindObjectOfType<CinemachineFreeLook>();
    }

    public void SwitchToDialogueCamera(CinemachineVirtualCamera newCamera, float blendTime) {
        if (newCamera == null) return;

        // Выключаем все виртуальные камеры
        CinemachineVirtualCamera[] allCams = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var cam in allCams) {
            if (cam != newCamera) {
                cam.Priority = 5;
                cam.enabled = false;
            }
        }

        _activeDialogueCamera = newCamera;

        if (_playerCamera != null) {
            _playerCamera.Priority = 5;
        }

        _activeDialogueCamera.enabled = true;
        _activeDialogueCamera.Priority = 100;

        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null) brain.m_DefaultBlend.m_Time = blendTime;
    }

    public void SetCameraInstant(CinemachineVirtualCamera newCamera) {
        SwitchToDialogueCamera(newCamera, 0f);
    }

    public void ReturnToPlayerCamera(float blendTime) {
        // Выключаем камеру диалога
        if (_activeDialogueCamera != null) {
            _activeDialogueCamera.Priority = 5;
            _activeDialogueCamera.enabled = false;
            _activeDialogueCamera = null;
        }

        // Выключаем все остальные камеры
        CinemachineVirtualCamera[] allCams = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var cam in allCams) {
            cam.enabled = false;
            cam.Priority = 5;
        }

        // Включаем камеру игрока
        if (_playerCamera != null) {
            _playerCamera.enabled = true;
            _playerCamera.Priority = 100; // Даем высокий приоритет
        }

        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null) brain.m_DefaultBlend.m_Time = blendTime;
    }
}