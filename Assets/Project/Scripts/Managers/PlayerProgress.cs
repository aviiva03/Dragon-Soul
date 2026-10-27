using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress : MonoBehaviour {
    public static PlayerProgress Instance { get; private set; }

    public bool HasSword { get; private set; } = false;
    public event Action OnSwordObtained;

    private List<string> _destroyedObjects = new List<string>();
    private List<string> _defeatedBosses = new List<string>();
    private List<string> _completedDialogues = new List<string>();

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

    // --- ÓÍÈ×ÒÎÆÅÍÍÛÅ ÎÁÚÅÊÒÛ ---
    public bool IsObjectDestroyed(string id) => _destroyedObjects.Contains(id);
    public void RegisterDestroyedObject(string id) {
        if (!_destroyedObjects.Contains(id)) { _destroyedObjects.Add(id); SaveSystem.SaveGame(); }
    }

    // --- ÓÁÈÒÛÅ ÁÎÑÑÛ ---
    public bool IsBossDefeated(string bossID) => _defeatedBosses.Contains(bossID);
    public void RegisterBossDefeated(string bossID) {
        if (!_defeatedBosses.Contains(bossID)) { _defeatedBosses.Add(bossID); SaveSystem.SaveGame(); }
    }

    // --- ÏĞÎÑËÓØÀÍÍÛÅ ÄÈÀËÎÃÈ ---
    public bool IsDialogueCompleted(string dialogueID) => _completedDialogues.Contains(dialogueID);
    public void RegisterDialogueCompleted(string dialogueID) {
        if (!_completedDialogues.Contains(dialogueID)) {
            _completedDialogues.Add(dialogueID);
            Debug.Log($"[Progress] Äèàëîã {dialogueID} çàïèñàí â ïğîãğåññ.");
            SaveSystem.SaveGame();
        }
    }

    // --- ÑÎÕĞÀÍÅÍÈÅ / ÇÀÃĞÓÇÊÀ ---
    public GameData GetSaveData() {
        GameData data = new GameData();
        data.hasSword = HasSword;
        data.destroyedObjectIDs = _destroyedObjects;
        data.defeatedBossIDs = _defeatedBosses;
        data.completedDialogueIDs = _completedDialogues; // Ñîõğàíÿåì äèàëîãè
        return data;
    }

    public void LoadFromData(GameData data) {
        HasSword = data.hasSword;
        _destroyedObjects = data.destroyedObjectIDs ?? new List<string>();
        _defeatedBosses = data.defeatedBossIDs ?? new List<string>();
        _completedDialogues = data.completedDialogueIDs ?? new List<string>(); // Çàãğóæàåì äèàëîãè
        if (HasSword) OnSwordObtained?.Invoke();
    }
}