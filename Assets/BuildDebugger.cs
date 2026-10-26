using UnityEngine;

public class BuildDebugger : MonoBehaviour {
    private void Awake() {
        Debug.Log("[BUILD] Игра запустилась! Awake.");
        Application.logMessageReceived += HandleLog;
    }

    private void Start() {
        Debug.Log("[BUILD] Start прошел. TimeScale: " + Time.timeScale);
    }

    private void HandleLog(string logString, string stackTrace, LogType type) {
        if (type == LogType.Exception || type == LogType.Error) {
            Debug.LogError($"[КРИТИЧЕСКАЯ ОШИБКА В БИЛДЕ]: {logString}\n{stackTrace}");
        }
    }
}