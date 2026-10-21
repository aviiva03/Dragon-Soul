using System.Collections.Generic;
using UnityEngine;

public class SimpleAttackSelector : MonoBehaviour {

    [SerializeField] private WeightedCombo[] _availableCombos;

    private ComboData _lastSelected;

    public ComboData SelectCombo(float distance) {
        if (_availableCombos == null || _availableCombos.Length == 0)
            return null;

        var candidates = new List<WeightedCombo>();
        float total = 0f;

        foreach (var entry in _availableCombos) {
            if (entry.combo == null) continue;
            if (distance < entry.minDistance || distance > entry.maxDistance) continue;
            if (entry.combo == _lastSelected) continue;

            candidates.Add(entry);
            total += entry.weight;
        }

        if (candidates.Count == 0) {
            foreach (var entry in _availableCombos) {
                if (entry.combo == null) continue;
                if (distance < entry.minDistance || distance > entry.maxDistance) continue;

                candidates.Add(entry);
                total += entry.weight;
            }
        }

        if (candidates.Count == 0 || total <= 0f) return null;

        float roll = Random.Range(0f, total);
        float accumulated = 0f;

        foreach (var entry in candidates) {
            accumulated += entry.weight;
            if (roll <= accumulated) {
                _lastSelected = entry.combo;
                return entry.combo;
            }
        }

        var last = candidates[candidates.Count - 1];
        _lastSelected = last.combo;
        return last.combo;
    }

    public void ResetLastSelected() {
        _lastSelected = null;
    }
}
