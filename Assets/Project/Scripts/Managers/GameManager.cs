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

    // ÂÎÇÂĞÀÙÀÅÌ ÑÎÁÛÒÈÅ, ×ÒÎÁÛ ÍÈ×ÅÃÎ ÍÅ ÑËÎÌÀÒÜ
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
    }

    private void OnDestroy() {
        if (_player != null) {
            var health = _player.GetComponent<Health>();
            if (health != null) health.OnDied -= HandlePlayerDeath;
        }
    }

    private void HandlePlayerDeath() {
        string text = _textsData != null ? _textsData.DeathText : "ÂÛ ÏÎÃÈÁËÈ";
        RespawnPlayer(text);
    }

    public void SetRespawnPoint(Transform newPoint) {
        if (newPoint != null) _currentRespawnPoint = newPoint;
    }

    public void RespawnFromFall() {
        string text = _textsData != null ? _textsData.FallText : "ÏÀÄÅÍÈÅ Â ÁÅÇÄÍÓ";
        RespawnPlayer(text);
    }

    public void RespawnFromStealth() {
        string text = _textsData != null ? _textsData.DetectedText : "ÂÀÑ ÇÀÌÅÒÈËÈ!";
        RespawnPlayer(text);
    }

    public void RespawnPlayer(string fadeText = null) {
        if (_player == null || _currentRespawnPoint == null) return;

        // Âûçûâàåì ñòàğîå ñîáûòèå (äëÿ òåõ ñêğèïòîâ, êîòîğûå íà íåãî ïîäïèñàíû)
        OnPlayerDetected?.Invoke();

        if (fadeText == null) {
            fadeText = _textsData != null ? _textsData.DefaultRespawnText : "ÂÎÇÂĞÀÙÅÍÈÅ...";
        }

        var movement = _player.GetComponent<PlayerController>();
        var animator = _player.GetComponent<PlayerAnimator>();
        if (movement != null) movement.enabled = false;
        if (animator != null) animator.enabled = false;

        if (ScreenFaderUI.Instance != null) {
            ScreenFaderUI.Instance.PlayFade(fadeText, TeleportAndRevive);
        } else {
            TeleportAndRevive();
        }
    }

    private void TeleportAndRevive() {
        var cc = _player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        _player.position = _currentRespawnPoint.position;
        _player.rotation = _currentRespawnPoint.rotation;

        if (cc != null) cc.enabled = true;

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
}