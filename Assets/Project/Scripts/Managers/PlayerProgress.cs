using System;
using UnityEngine;

public class PlayerProgress : MonoBehaviour {
    public static PlayerProgress Instance { get; private set; }

    public bool HasSword { get; private set; } = false;

    public event Action OnSwordObtained;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GiveSword() {
        if (HasSword) return;
        HasSword = true;
        OnSwordObtained?.Invoke();

        // Сохраняем прогресс сразу как получили меч!
        SaveSystem.SaveGame();
    }

    // --- ДЛЯ СИСТЕМЫ СОХРАНЕНИЯ ---

    // Собираем данные для сохранения
    public GameData GetSaveData() {
        GameData data = new GameData();
        data.hasSword = HasSword;
        return data;
    }

    // Восстанавливаем данные при загрузке
    public void LoadFromData(GameData data) {
        HasSword = data.hasSword;

        if (HasSword) {
            OnSwordObtained?.Invoke();
        }
    }
}