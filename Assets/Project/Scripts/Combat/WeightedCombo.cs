using System;
using UnityEngine;

[Serializable]
public class WeightedCombo {

    public ComboData combo;
    [Min(0)] public float weight = 1f;
    [Min(0)] public float minDistance = 0f;
    [Min(0)] public float maxDistance = 100f;
}
