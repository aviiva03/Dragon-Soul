using UnityEngine;

public class CursorLockManager : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] private bool _lockOnStart = true;

    private void Start() {
        if (_lockOnStart) LockCursor();
    }

    private void Update() {
        // 1. Если идет диалог/катсцена - не трогаем курсор
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive) return;

        // 2. Если открыт пазл - не трогаем курсор
        if (SwordPuzzleManager.Instance != null && SwordPuzzleManager.Instance.IsPuzzleActive) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        // 3. Обычная логика
        if (Input.GetKeyDown(KeyCode.Escape)) {
            UnlockCursor();
        }

        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked) {
            LockCursor();
        }
    }

    public void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}