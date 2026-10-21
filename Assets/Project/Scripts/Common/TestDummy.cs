using UnityEngine;

/// <summary>
/// Временный тестовый "враг" для проверки хитбокса.
/// Реализует IDamageable и принимает урон.
/// </summary>
public class TestDummy : MonoBehaviour, IDamageable {

    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    public bool IsAlive => _currentHealth > 0f;
    public Transform Transform => transform;

    private void Awake() {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage, Vector3 hitPoint) {
        _currentHealth = Mathf.Max(0f, _currentHealth - damage);
        Debug.Log($"[DUMMY] Took {damage} damage! HP: {_currentHealth}/{_maxHealth}");

        // Визуальная вспышка — если есть Renderer
        var renderer = GetComponent<Renderer>();
        if (renderer != null) {
            StartCoroutine(FlashRed(renderer));
        }
    }

    private System.Collections.IEnumerator FlashRed(Renderer renderer) {
        Color original = renderer.material.color;
        renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        renderer.material.color = original;
    }

    private void OnDrawGizmos() {
        // Показываем HP
        Gizmos.color = IsAlive ? Color.green : Color.gray;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}