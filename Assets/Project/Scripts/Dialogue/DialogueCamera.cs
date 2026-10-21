using System.Collections;
using UnityEngine;
using Cinemachine;

public class DialogueCamera : MonoBehaviour {

    [Header("References")]
    [SerializeField] private CinemachineFreeLook _playerCamera; // Наша обычная камера
    [SerializeField] private CinemachineVirtualCamera _dialogueCamera; // Камера для диалогов

    private CinemachineBrain _brain;

    private void Awake() {
        // Получаем Brain с главной камеры, чтобы менять blending время
        _brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    /// <summary>
    /// Включает камеру диалога.
    /// </summary>
    public void ActivateDialogueCamera() {
        // Повышаем приоритет камеры диалога (Cinemachine переключится на неё)
        _dialogueCamera.Priority = 20;
        _playerCamera.Priority = 10;
    }

    /// <summary>
    /// Возвращает управление камере игрока.
    /// </summary>
    public void DeactivateDialogueCamera() {
        // Понижаем приоритет
        _dialogueCamera.Priority = 5;
        _playerCamera.Priority = 10;
    }

    /// <summary>
    /// Мгновенно переключает камеру (для старта диалога).
    /// </summary>
    public void SetCutInstant(bool isCut) {
        if (_brain != null)
            _brain.m_DefaultBlend.m_Time = isCut ? 0f : 1.5f; // 0 = без плавности, 1.5 = плавный полет
    }
}