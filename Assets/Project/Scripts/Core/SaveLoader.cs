using UnityEngine;

public class SaveLoader : MonoBehaviour {
    private void Awake() {
        if (PlayerProgress.Instance != null) {
            GameData loadedData = SaveSystem.LoadGame();
            PlayerProgress.Instance.LoadFromData(loadedData);
        }
    }
}