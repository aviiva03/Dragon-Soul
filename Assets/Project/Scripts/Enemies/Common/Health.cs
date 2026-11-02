using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable {

    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;
    public event Action<int, Vector3> OnDamaged; // Для эффектов попадания

    public bool IsAlive => _currentHealth > 0;
    public int Current => _currentHealth;
    public int Max => _maxHealth;

    private void Awake() {
        _currentHealth = _maxHealth;
    }

    private void Start() {
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void TakeDamage(int amount, Vector3 hitPoint) {
        if (!IsAlive) return;
        if (amount <= 0) return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        OnDamaged?.Invoke(amount, hitPoint);

        if (!IsAlive) {
            OnDied?.Invoke();
        }
    }

    public void Heal(int amount) {
        if (!IsAlive) return;

        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void Revive() {
        _currentHealth = _maxHealth;
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }
}