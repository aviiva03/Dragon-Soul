using UnityEngine;

public class StoryTrigger : MonoBehaviour {
    private bool _triggered = false;

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        if (_triggered) return;

        _triggered = true;
        GetComponent<Collider>().enabled = false; // Отключаем, чтобы не сработал дважды

        if (StoryManager.Instance != null) {
            StoryManager.Instance.StartStage1();
        }
    }
}