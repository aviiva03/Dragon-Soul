using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour {

    [Header("Config")]
    [SerializeField] private float _radius = 0.6f;
    [SerializeField] private LayerMask _hitLayers;

    [Header("Debug")]
    [SerializeField] private bool _debugMode = true;

    private bool _isActive;
    private float _currentDamage;
    private Vector3 _hitDirection;

    private readonly HashSet<IDamageable> _alreadyHit = new HashSet<IDamageable>();

    public bool IsActive => _isActive;
    public float Radius => _radius;

    public void Activate(float damage, Vector3 hitDirection, float radius = -1f) {
        if (radius > 0f) _radius = radius;

        _isActive = true;
        _currentDamage = damage;
        _hitDirection = hitDirection.normalized;
        _alreadyHit.Clear();

        PerformHitCheck();

        if (_debugMode)
            Debug.Log($"[HITBOX] Activated, damage={damage}, dir={_hitDirection}");
    }

    public void Deactivate() {
        _isActive = false;
        _alreadyHit.Clear();
    }

    private void PerformHitCheck() {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            _radius,
            _hitLayers,
            QueryTriggerInteraction.Collide);

        foreach (var col in hits) {
            IDamageable damageable = FindDamageable(col);
            if (damageable == null) continue;
            if (!damageable.IsAlive) continue;
            if (_alreadyHit.Contains(damageable)) continue;

            _alreadyHit.Add(damageable);

            Vector3 hitPoint = col.ClosestPoint(transform.position);

            // ¬Œ“ √À¿¬ÕŒ≈ »—œ–¿¬À≈Õ»≈:
            // œË‚Ó‰ËÏ float damage Í int (Health ËÒÔÓÎ¸ÁÛÂÚ int)
            int intDamage = Mathf.RoundToInt(_currentDamage);
            damageable.TakeDamage(intDamage, hitPoint);

            if (_debugMode)
                Debug.Log($"[HITBOX] Hit for {intDamage} to {col.name}");
        }
    }

    private IDamageable FindDamageable(Collider col) {
        if (col.TryGetComponent<IDamageable>(out var d)) return d;

        Transform t = col.transform.parent;
        int depth = 0;
        while (t != null && depth < 5) {
            if (t.TryGetComponent<IDamageable>(out d)) return d;
            t = t.parent;
            depth++;
        }
        return null;
    }

    private void OnDrawGizmos() {
        if (!_debugMode) return;

        Gizmos.color = _isActive
            ? new Color(1f, 0f, 0f, 0.7f)
            : new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, _radius);

        if (_isActive) {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}