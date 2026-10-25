using UnityEngine;

public class SaveLoader : MonoBehaviour {
    private void Start() {
        // При старте игры загружаем данные
        GameData loadedData = SaveSystem.LoadGame();

        if (PlayerProgress.Instance != null) {
            PlayerProgress.Instance.LoadFromData(loadedData);
        }
    }
}