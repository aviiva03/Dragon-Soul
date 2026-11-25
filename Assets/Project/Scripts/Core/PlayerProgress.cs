using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress : MonoBehaviour {
    public static PlayerProgress Instance { get; private set; }

    public bool HasSword { get; private set; } = false;
    public bool HasDragonClaw { get; private set; } = false;
    public bool IsArenaCleared { get; private set; } = false;

    public event Action OnSwordObtained;

    private List<string> _destroyedObjects = new List<string>();
    private List<string> _defeatedBosses = new List<string>();
    private List<string> _completedDialogues = new List<string>();
    private List<string> _unlockedAbilities = new List<string>();

    private Vector3 _savedPlayerPosition;
    private Quaternion _savedPlayerRotation;
    private bool _hasSavedPosition = false;

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

    public void GiveDragonClaw() {
        if (HasDragonClaw) return;
        HasDragonClaw = true;
        UnlockAbility("Biyu_Attack");
        SaveSystem.SaveGame();
    }

    // --- ¿–≈Õ¿ ---
    public void SetArenaCleared() {
        if (IsArenaCleared) return;
        IsArenaCleared = true;
        SaveSystem.SaveGame();
    }

    // --- ”Õ»◊“Œ∆≈ÕÕ€≈ Œ¡⁄≈ “€ ---
    public bool IsObjectDestroyed(string id) => _destroyedObjects.Contains(id);
    public void RegisterDestroyedObject(string id) {
        if (!_destroyedObjects.Contains(id)) { _destroyedObjects.Add(id); SaveSystem.SaveGame(); }
    }

    // --- ”¡»“€≈ ¡Œ——€ ---
    public bool IsBossDefeated(string bossID) => _defeatedBosses.Contains(bossID);
    public void RegisterBossDefeated(string bossID) {
        if (!_defeatedBosses.Contains(bossID)) { _defeatedBosses.Add(bossID); SaveSystem.SaveGame(); }
    }

    // --- œ–Œ—À”ÿ¿ÕÕ€≈ ƒ»¿ÀŒ√» ---
    public bool IsDialogueCompleted(string dialogueID) => _completedDialogues.Contains(dialogueID);
    public void RegisterDialogueCompleted(string dialogueID) {
        if (!_completedDialogues.Contains(dialogueID)) {
            _completedDialogues.Add(dialogueID);
            SaveSystem.SaveGame();
        }
    }

    // --- —œŒ—Œ¡ÕŒ—“» ¡»ﬁ ---
    public void UnlockAbility(string abilityID) {
        if (!_unlockedAbilities.Contains(abilityID)) {
            _unlockedAbilities.Add(abilityID);

            if (AbilityManager.Instance != null) {
                AbilityManager.Instance.UnlockAbility(abilityID);
            }

            SaveSystem.SaveGame();
        }
    }

    public bool IsAbilityUnlocked(string abilityID) => _unlockedAbilities.Contains(abilityID);

    // --- œŒ«»÷»ﬂ »√–Œ ¿ ---
    public void SetPlayerPosition(Vector3 position, Quaternion rotation) {
        _savedPlayerPosition = position;
        _savedPlayerRotation = rotation;
        _hasSavedPosition = true;
    }

    public bool TryGetPlayerPosition(out Vector3 position, out Quaternion rotation) {
        position = _savedPlayerPosition;
        rotation = _savedPlayerRotation;
        return _hasSavedPosition;
    }

    // --- —Œ’–¿Õ≈Õ»≈ / «¿√–”« ¿ ---
    public GameData GetSaveData() {
        GameData data = new GameData();
        data.hasSword = HasSword;
        data.hasDragonClaw = HasDragonClaw;
        data.isArenaCleared = IsArenaCleared;
        data.destroyedObjectIDs = _destroyedObjects;
        data.defeatedBossIDs = _defeatedBosses;
        data.completedDialogueIDs = _completedDialogues;
        data.unlockedAbilityIDs = _unlockedAbilities;

        if (_hasSavedPosition) {
            data.PlayerPosition = _savedPlayerPosition;
            data.PlayerRotation = _savedPlayerRotation;
        }
        return data;
    }

    public void LoadFromData(GameData data) {
        HasSword = data.hasSword;
        HasDragonClaw = data.hasDragonClaw;
        IsArenaCleared = data.isArenaCleared;
        _destroyedObjects = data.destroyedObjectIDs ?? new List<string>();
        _defeatedBosses = data.defeatedBossIDs ?? new List<string>();
        _completedDialogues = data.completedDialogueIDs ?? new List<string>();
        _unlockedAbilities = data.unlockedAbilityIDs ?? new List<string>();

        if (data.playerPosX != 0f || data.playerPosY != 0f || data.playerPosZ != 0f) {
            _savedPlayerPosition = data.PlayerPosition;
            _savedPlayerRotation = data.PlayerRotation;
            _hasSavedPosition = true;
        } else {
            _hasSavedPosition = false;
        }

        if (HasSword) OnSwordObtained?.Invoke();
    }
}