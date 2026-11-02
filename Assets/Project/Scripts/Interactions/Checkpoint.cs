using UnityEngine;

public class Checkpoint : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null) {
            GameManager.Instance.SetRespawnPoint(transform);

            GameManager.Instance.SavePlayerLocation();
            Debug.Log("[Checkpoint] Игра сохранена на чекпоинте.");

            GetComponent<Collider>().enabled = false;
        }
    }
}