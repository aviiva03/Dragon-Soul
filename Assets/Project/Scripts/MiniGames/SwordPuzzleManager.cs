using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPuzzleManager : MonoBehaviour {
    public static SwordPuzzleManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float _snapDistance = 150f;
    [SerializeField] private GameObject _assembledSwordImage;

    [Header("UI")]
    [SerializeField] private Transform _piecesParent;
    [SerializeField] private Transform _slotsParent;
    [SerializeField] private GameObject _puzzlePanel;

    [Header("Camera")]
    [SerializeField] private CinemachineFreeLook _playerCamera; // Перетащи сюда камеру игрока

    private CinemachineBrain _brain; // Добавим мозг камеры

    public event Action OnPuzzleCompleted;

    private List<PuzzlePiece> _pieces = new List<PuzzlePiece>();
    private List<PuzzleSlot> _slots = new List<PuzzleSlot>();
    private int _piecesPlaced = 0;
    private int _totalPieces = 0;

    public bool IsPuzzleActive => _puzzlePanel != null && _puzzlePanel.activeSelf;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        // Находим CinemachineBrain на главной камере
        if (Camera.main != null) _brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    private void Start() {
        _pieces.AddRange(_piecesParent.GetComponentsInChildren<PuzzlePiece>());
        _slots.AddRange(_slotsParent.GetComponentsInChildren<PuzzleSlot>());

        _totalPieces = _pieces.Count;
        _piecesPlaced = 0;
        if (_assembledSwordImage != null) _assembledSwordImage.SetActive(false);

        if (_puzzlePanel != null) _puzzlePanel.SetActive(false);
    }

    public void StartPuzzle() {
        if (_puzzlePanel != null) _puzzlePanel.SetActive(true);
        _piecesPlaced = 0;

        foreach (var piece in _pieces) piece.ResetPiece();
        foreach (var slot in _slots) slot.ResetSlot();

        if (_assembledSwordImage != null) _assembledSwordImage.SetActive(false);

        // --- БЛОКИРУЕМ КАМЕРУ ---
        if (_brain != null) _brain.enabled = false; // Замораживаем мозг Cinemachine
        if (_playerCamera != null) _playerCamera.enabled = false; // Отключаем управление камерой

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update() {
        if (_puzzlePanel == null || !_puzzlePanel.activeSelf) return;
        if (_piecesPlaced >= _totalPieces) return;

        CheckForSnapping();
    }

    private void CheckForSnapping() {
        foreach (var piece in _pieces) {
            if (piece.isPlaced) continue;

            foreach (var slot in _slots) {
                if (slot.isFilled) continue;

                if (piece.pieceId == slot.slotId) {
                    float distance = Vector2.Distance(piece.transform.position, slot.transform.position);

                    if (distance < _snapDistance) {
                        piece.SnapToSlot(slot);
                        slot.FillSlot();
                        _piecesPlaced++;
                        CheckWinCondition();
                        return;
                    }
                }
            }
        }
    }

    private void CheckWinCondition() {
        if (_piecesPlaced >= _totalPieces) {
            Debug.Log("[Puzzle] Меч собран!");
            if (_assembledSwordImage != null) _assembledSwordImage.SetActive(true);
            StartCoroutine(FinishPuzzleRoutine());
        }
    }

    private IEnumerator FinishPuzzleRoutine() {
        yield return new WaitForSeconds(1.5f);

        if (_puzzlePanel != null) _puzzlePanel.SetActive(false);

        // --- ВОЗВРАЩАЕМ КАМЕРУ ---
        if (_playerCamera != null) _playerCamera.enabled = true;
        if (_brain != null) _brain.enabled = true; // Включаем обратно

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnPuzzleCompleted?.Invoke();
    }
}