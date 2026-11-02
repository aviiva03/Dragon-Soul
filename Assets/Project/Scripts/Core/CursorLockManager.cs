using UnityEngine;

public class CursorLockManager : MonoBehaviour {

    [SerializeField] private bool _lockOnStart = true;

    private void Start() {
        if (_lockOnStart) LockCursor();
    }

    private void Update() {
        // Ќе трогаем курсор во врем€ диалогов или пазлов
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive) return;
        if (SwordPuzzleManager.Instance != null && SwordPuzzleManager.Instance.IsPuzzleActive) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) UnlockCursor();
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked) LockCursor();
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