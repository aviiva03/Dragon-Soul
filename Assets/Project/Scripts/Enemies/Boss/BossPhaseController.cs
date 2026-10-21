using System;
using UnityEngine;

public class BossPhaseController : MonoBehaviour {

    [SerializeField] private Health _health;
    [SerializeField] private BossPhase[] _phases;

    private BossPhase _currentPhase;

    public BossPhase CurrentPhase => _currentPhase;
    public event Action<BossPhase> OnPhaseChanged;

    private void Start() {
        if (_health == null || _phases == null || _phases.Length == 0)
            return;

        _health.OnHealthChanged += HandleHealthChanged;
        UpdatePhase(_health.Current, _health.Max);
    }

    private void OnDestroy() {
        if (_health != null)
            _health.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int current, int max) {
        UpdatePhase(current, max);
    }

    private void UpdatePhase(int current, int max) {
        float percent = (float)current / max;
        BossPhase newPhase = FindPhase(percent);

        if (newPhase == null || newPhase == _currentPhase) return;

        _currentPhase = newPhase;
        Debug.Log($"Фаза босса: {_currentPhase.phaseName}");
        OnPhaseChanged?.Invoke(_currentPhase);
    }

    private BossPhase FindPhase(float percent) {
        foreach (var phase in _phases) {
            if (percent <= phase.upperHealthPercent && percent > phase.lowerHealthPercent)
                return phase;
        }

        BossPhase lowest = _phases[0];
        foreach (var phase in _phases) {
            if (phase.lowerHealthPercent < lowest.lowerHealthPercent)
                lowest = phase;
        }
        return lowest;
    }
}