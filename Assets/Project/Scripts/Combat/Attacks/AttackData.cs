using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Boss/Attack Data")]
public class AttackData : ScriptableObject {

    [Header("Identity")]
    public string attackName = "New Attack";

    [Header("Animation")]
    public string animationTrigger = "attack";

    [Header("Timing")]
    [Tooltip("¬рем€ замаха Ч сколько игрок может реагировать")]
    public float windupTime = 0.6f;
    [Tooltip("¬рем€ удара Ч момент нанесени€ урона")]
    public float strikeTime = 0.15f;
    [Tooltip("¬рем€ восстановлени€ Ч окно у€звимости")]
    public float recoveryTime = 0.5f;

    [Header("Damage")]
    public int damage = 15;
    public float range = 2.5f;
    [Tooltip("”гол поражени€. 360 = круг, 90 = конус спереди")]
    [Range(0f, 360f)] public float hitAngle = 360f;

    [Header("Movement During Attack")]
    [Tooltip("ƒистанци€, на которую босс прыгает вперЄд в момент удара")]
    public float forwardJumpDistance = 0f;
    [Tooltip("≈сли true Ч прыжок в сторону игрока, а не вперЄд")]
    public bool jumpToPlayer = false;
}
