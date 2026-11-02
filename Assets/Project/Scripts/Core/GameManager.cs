using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private Transform _player;

    [Header("Respawn Settings")]
    [SerializeField] private Transform _defaultRespawnPoint;
    [SerializeField] private GameTextsData _textsData;

    private Transform _currentRespawnPoint;

    public Transform Player => _player;

    public event Action OnPlayerDetected;
    public event Action OnPlayerRespawn;

    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _currentRespawnPoint = _defaultRespawnPoint;
    }

    private void Start() {
        if (_player != null) {
            var health = _player.GetComponent<Health>();
            if (health != null) health.OnDied += HandlePlayerDeath;
        }

        if (PlayerProgress.Instance != null) {
            if (PlayerProgress.Instance.TryGetPlayerPosition(out Vector3 pos, out Quaternion rot)) {
                TeleportPlayer(pos, rot);
            } else {
                if (_defaultRespawnPoint != null) {
                    TeleportPlayer(_defaultRespawnPoint.position, _defaultRespawnPoint.rotation);
                }
            }
        }
    }

    private void OnDestroy() {
        if (_player != null) {
            var health = _player.GetComponent<Health>();
            if (health != null) health.OnDied -= HandlePlayerDeath;
        }
    }

    private void HandlePlayerDeath() {
        string text = _textsData != null ? _textsData.DeathText : "¬€ œŒ√»¡À»";
        RespawnPlayer(text);
    }

    public void SetRespawnPoint(Transform newPoint) {
        if (newPoint != null) _currentRespawnPoint = newPoint;
    }

    public void RespawnFromFall() {
        string text = _textsData != null ? _textsData.FallText : "œ¿ƒ≈Õ»≈ ¬ ¡≈«ƒÕ”";
        RespawnPlayer(text);
    }

    public void RespawnFromStealth() {
        string text = _textsData != null ? _textsData.DetectedText : "¬¿— «¿Ã≈“»À»!";
        RespawnPlayer(text);
    }

    public void RespawnPlayer(string fadeText = null) {
        if (_player == null || _currentRespawnPoint == null) return;

        OnPlayerDetected?.Invoke();

        if (fadeText == null) {
            fadeText = _textsData != null ? _textsData.DefaultRespawnText : "¬Œ«¬–¿Ÿ≈Õ»≈...";
        }

        var movement = _player.GetComponent<PlayerController>();
        var animator = _player.GetComponent<PlayerAnimator>();
        if (movement != null) movement.enabled = false;
        if (animator != null) animator.enabled = false;

        if (ScreenFaderUI.Instance != null) {
            ScreenFaderUI.Instance.PlayFade(fadeText, () => TeleportAndRevive(_currentRespawnPoint.position, _currentRespawnPoint.rotation));
        } else {
            TeleportAndRevive(_currentRespawnPoint.position, _currentRespawnPoint.rotation);
        }
    }

    private void TeleportAndRevive(Vector3 position, Quaternion rotation) {
        TeleportPlayer(position, rotation);

        var health = _player.GetComponent<Health>();
        if (health != null) health.Revive();

        FallZone[] allFallZones = FindObjectsOfType<FallZone>();
        foreach (var zone in allFallZones) {
            zone.ResetZone();
        }

        var movement = _player.GetComponent<PlayerController>();
        var animator = _player.GetComponent<PlayerAnimator>();
        if (movement != null) movement.enabled = true;
        if (animator != null) animator.enabled = true;

        OnPlayerRespawn?.Invoke();
    }

    private void TeleportPlayer(Vector3 position, Quaternion rotation) {
        var cc = _player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        _player.position = position;
        _player.rotation = rotation;

        if (cc != null) cc.enabled = true;
    }

    public void SavePlayerLocation() {
        if (_player == null || PlayerProgress.Instance == null) return;

        PlayerProgress.Instance.SetPlayerPosition(_player.position, _player.rotation);
        SaveSystem.SaveGame();
    }
}