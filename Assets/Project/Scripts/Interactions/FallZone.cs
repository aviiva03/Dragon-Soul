using UnityEngine;

public class FallZone : MonoBehaviour {

    [Tooltip("Ќа какой высоте по Y находитс€ дно пропасти")]
    [SerializeField] private float _fallYThreshold = -10f;

    private bool _hasTriggered = false;

    private void Update() {
        if (_hasTriggered) return;

        if (GameManager.Instance != null && GameManager.Instance.Player != null) {
            if (GameManager.Instance.Player.position.y < _fallYThreshold) {
                _hasTriggered = true;
                GameManager.Instance.RespawnFromFall();
            }
        }
    }

    public void ResetZone() {
        _hasTriggered = false;
    }
}