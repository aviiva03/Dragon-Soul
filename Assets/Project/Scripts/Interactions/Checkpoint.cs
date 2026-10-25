using UnityEngine;

public class Checkpoint : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;

        // Меняем точку респавна в GameManager на этот объект
        if (GameManager.Instance != null) {
            GameManager.Instance.SetRespawnPoint(transform);

            // Отключаем коллайдер, чтобы чекпоинт не срабатывал постоянно
            GetComponent<Collider>().enabled = false;
        }
    }
}