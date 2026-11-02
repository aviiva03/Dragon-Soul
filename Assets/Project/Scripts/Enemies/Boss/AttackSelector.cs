using System.Collections.Generic;
using UnityEngine;

public class AttackSelector : MonoBehaviour {

    [SerializeField] private BossPhaseController _phaseController;
    private ComboData _lastSelected;

    public ComboData SelectCombo(float distance) {

        if (_phaseController == null || _phaseController.CurrentPhase == null)
            return null;

        var combos = _phaseController.CurrentPhase.availableCombos;
        var candidates = new List<WeightedCombo>();
        float total = 0f;

        // Проход 1 — по дистанции, исключая последнее
        foreach (var entry in combos) {
            if (entry.combo == null) continue;
            if (distance < entry.minDistance || distance > entry.maxDistance) continue;
            if (entry.combo == _lastSelected) continue;

            candidates.Add(entry);
            total += entry.weight;
        }

        // Проход 2 — разрешаем последнее
        if (candidates.Count == 0) {
            total = 0f;

            foreach (var entry in combos) {
                if (entry.combo == null) continue;
                if (distance < entry.minDistance || distance > entry.maxDistance) continue;

                candidates.Add(entry);
                total += entry.weight;
            }
        }

        if (candidates.Count == 0 || total <= 0f) return null;

        // Взвешенный рандом
        float roll = Random.Range(0f, total);
        float accumulated = 0f;

        foreach (var entry in candidates) {
            accumulated += entry.weight;
            if (roll <= accumulated) {
                _lastSelected = entry.combo;
                return entry.combo;
            }
        }

        // Float-страховка
        var last = candidates[candidates.Count - 1];
        _lastSelected = last.combo;
        return last.combo;
    }

    public void ResetLastSelected() {
        _lastSelected = null;
    }
}