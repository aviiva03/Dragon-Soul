using System;
using UnityEngine;

public class PlayerProgress : MonoBehaviour {
    public static PlayerProgress Instance { get; private set; }

    public bool HasSword { get; private set; } = true;

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
        Debug.Log("[PlayerProgress] Меч получен! Атака разблокирована.");
        OnSwordObtained?.Invoke();
    }
}