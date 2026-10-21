using System;
using UnityEngine;

public class PlayerProgress : MonoBehaviour {
    public static PlayerProgress Instance { get; private set; }

    // --- Флаги прогресса ---
    public bool HasSword { get; private set; } = false;

    // В будущем сюда добавятся:
    // public bool HasKilledBoss = false;
    // public int CollectedStones = 0;
    // public bool UnlockedTempleDoor = false;

    // --- События (чтобы интерфейс мог реагировать) ---
    public event Action OnSwordObtained;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Если ты переходишь между сценами, этот объект не должен удаляться
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Вызывается, когда игрок получает меч (например, из скрипта награды).
    /// </summary>
    public void GiveSword() {
        if (HasSword) return; // Защита от повторной выдачи

        HasSword = true;
        Debug.Log("[PlayerProgress] Меч получен!");

        OnSwordObtained?.Invoke();
    }
}