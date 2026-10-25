using UnityEngine;

public class WorldStateObject : MonoBehaviour {
    [Tooltip("Уникальный ID (например, Wall_01). Не должен повторяться в игре!")]
    public string uniqueID;

    private void Start() {
        // При загрузке проверяем: если этот объект уже был уничтожен, убиваем его
        if (PlayerProgress.Instance != null && PlayerProgress.Instance.IsObjectDestroyed(uniqueID)) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Вызывается, когда объект нужно уничтожить (например, из StoryManager).
    /// </summary>
    public void DestroyAndSave() {
        if (PlayerProgress.Instance != null) {
            PlayerProgress.Instance.RegisterDestroyedObject(uniqueID);
        }
        Destroy(gameObject);
    }
}