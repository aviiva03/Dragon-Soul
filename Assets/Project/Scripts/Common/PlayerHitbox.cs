using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour {

    [Header("Config")]
    [SerializeField] private float _radius = 0.6f;
    [SerializeField] private LayerMask _hitLayers;

    [Header("Debug")]
    [SerializeField] private bool _drawGizmos = true;

    private bool _isActive;
    private float _currentDamage;
    private readonly HashSet<IDamageable> _alreadyHit = new HashSet<IDamageable>();

    public bool IsActive => _isActive;
    public float Radius => _radius;

    public void Activate(float damage, Vector3 hitDirection, float radius = -1f) {
        if (radius > 0f) _radius = radius;

        _isActive = true;
        _currentDamage = damage;
        _alreadyHit.Clear();

        PerformHitCheck();
    }

    public void Deactivate() {
        _isActive = false;
        _alreadyHit.Clear();
    }

    private void PerformHitCheck() {
        Collider[] hits = Physics.OverlapSphere(transform.position, _radius, _hitLayers, QueryTriggerInteraction.Collide);

        foreach (var col in hits) {
            IDamageable damageable = FindDamageable(col);
            if (damageable == null || !damageable.IsAlive || _alreadyHit.Contains(damageable)) continue;

            _alreadyHit.Add(damageable);
            Vector3 hitPoint = col.ClosestPoint(transform.position);
            int intDamage = Mathf.RoundToInt(_currentDamage);
            
            damageable.TakeDamage(intDamage, hitPoint);
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
        if (!_drawGizmos) return;

        Gizmos.color = _isActive ? new Color(1f, 0f, 0f, 0.7f) : new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, _radius);

        if (_isActive) {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}