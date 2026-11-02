using UnityEngine;

[CreateAssetMenu(fileName = "Combo", menuName = "Boss/Combo Data")]
public class ComboData : ScriptableObject {

    [Header("Identity")]
    public string comboName = "New Combo";

    [Header("Steps")]
    [Tooltip("Атаки в порядке исполнения")]
    public AttackData[] steps;

    [Header("Recovery")]
    [Tooltip("Пауза между шагами комбо")]
    public float betweenStepsDelay = 0.3f;
    [Tooltip("Пауза после завершения всего комбо")]
    public float afterComboDelay = 1.0f;

    [Header("Preferred Distance")]
    [Tooltip("Дистанция, с которой комбо лучше всего работает")]
    public float preferredDistance = 5f;
}
