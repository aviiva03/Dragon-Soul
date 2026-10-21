using UnityEngine;

[CreateAssetMenu(fileName = "Phase", menuName = "Boss/Phase")]
public class BossPhase : ScriptableObject {

    [Header("Identity")]
    public string phaseName = "Phase 1";

    [Header("Health Range")]
    [Range(0f, 1f)]
    [Tooltip("Ќижн€€ граница фазы Ч дол€ от maxHP (0 = смерть, 1 = полное HP)")]
    public float lowerHealthPercent = 0f;
    [Range(0f, 1f)]
    [Tooltip("¬ерхн€€ граница фазы Ч дол€ от maxHP")]
    public float upperHealthPercent = 1f;

    [Header("Combat")]
    [Tooltip(" омбо, доступные в этой фазе")]
    public WeightedCombo[] availableCombos;
    [Tooltip("ћножитель скорости передвижени€")]
    public float speedMultiplier = 1f;
    [Tooltip("ћножитель глобального кулдауна между атаками")]
    public float cooldownMultiplier = 1f;
}
