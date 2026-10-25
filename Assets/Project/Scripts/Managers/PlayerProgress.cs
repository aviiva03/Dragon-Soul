using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress : MonoBehaviour {
    public static PlayerProgress Instance { get; private set; }

    public bool HasSword { get; private set; } = false;
    public event Action OnSwordObtained;

    private List<string> _destroyedObjects = new List<string>();
    private List<string> _defeatedBosses = new List<string>();

    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GiveSword() {
        if (HasSword) return;
        HasSword = true;
        OnSwordObtained?.Invoke();
        SaveSystem.SaveGame();
    }

    // --- УНИЧТОЖЕННЫЕ ОБЪЕКТЫ ---
    public bool IsObjectDestroyed(string id) => _destroyedObjects.Contains(id);
    public void RegisterDestroyedObject(string id) {
        if (!_destroyedObjects.Contains(id)) {
            _destroyedObjects.Add(id);
            SaveSystem.SaveGame();
        }
    }

    // --- УБИТЫЕ БОССЫ ---
    public bool IsBossDefeated(string bossID) => _defeatedBosses.Contains(bossID);

    public void RegisterBossDefeated(string bossID) {
        if (!_defeatedBosses.Contains(bossID)) {
            _defeatedBosses.Add(bossID);
            Debug.Log($"[Progress] Босс {bossID} побежден и сохранен.");
            SaveSystem.SaveGame(); // Сохраняем прогресс
        }
    }

    // --- СОХРАНЕНИЕ / ЗАГРУЗКА ---
    public GameData GetSaveData() {
        GameData data = new GameData();
        data.hasSword = HasSword;
        data.destroyedObjectIDs = _destroyedObjects;
        data.defeatedBossIDs = _defeatedBosses; // Сохраняем боссов
        return data;
    }

    public void LoadFromData(GameData data) {
        HasSword = data.hasSword;
        _destroyedObjects = data.destroyedObjectIDs ?? new List<string>();
        _defeatedBosses = data.defeatedBossIDs ?? new List<string>(); // Загружаем боссов
        if (HasSword) OnSwordObtained?.Invoke();
    }
}