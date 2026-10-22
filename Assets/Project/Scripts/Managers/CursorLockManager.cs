using UnityEngine;

public class CursorLockManager : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] private bool _lockOnStart = true;

    private void Start() {
        if (_lockOnStart)
            LockCursor();
    }

    private void Update() {
        PlayerController pc = FindObjectOfType<PlayerController>();
        if (pc != null && !pc.enabled) return; // ≈сли игрок отключен (катсцена) Ч ничего не делаем!

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive) return;

        // Esc Ч освободить курсор
        if (Input.GetKeyDown(KeyCode.Escape)) {
            UnlockCursor();
        }

        //  лик по экрану Ч снова захватить (если игра не на паузе)
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