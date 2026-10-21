using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance { get; private set; }

    [Header("player")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _playerStartPoint;

    public Transform Player => _player;

    public event Action OnPlayerRespawn;
    public event Action OnPlayerDetected;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RespawnPlayer() {
        if (_player == null || _playerStartPoint == null)
            return;

        OnPlayerDetected?.Invoke();

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine() {
        var movement = _player.GetComponent<PlayerController>();
        var animator = _player.GetComponent<PlayerAnimator>();
        if (movement != null && animator != null) {
            movement.enabled = false;
            animator.enabled = false;
        }

        yield return new WaitForSecondsRealtime(1.5f);

        var controller = _player.GetComponent<CharacterController>();
        bool hadcontroller = controller != null;
        if (hadcontroller)
            controller.enabled = false;

        _player.position = _playerStartPoint.position;
        _player.rotation = _playerStartPoint.rotation;

        if (hadcontroller)
            controller.enabled = true;

        OnPlayerRespawn?.Invoke();
        Debug.Log("Игрок респавнится");

        if (movement != null && animator != null) {
            movement.enabled = true;
            animator.enabled = true;
        }
    }
}
