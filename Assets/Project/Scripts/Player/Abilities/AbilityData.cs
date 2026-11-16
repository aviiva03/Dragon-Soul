using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Game/Ability Data")]
public class AbilityData : ScriptableObject {
    [Header("Identity")]
    public string abilityName = "Dragon Claw";
    public string abilityID = "Biyu_Attack";
    public Sprite icon;

    [Header("Input")]
    public string inputKey = "Q"; // Для отображения на UI

    [Header("Stats")]
    public float cooldown = 10f;
    public float abilityRange = 15f;

    [Header("Biyu Debuff")]
    public bool applyDebuff = true;
    public float debuffDuration = 3f;
    [Tooltip("Процент урона от максимального здоровья врага (за тик)")]
    public float debuffDamagePercent = 5f; // 5% в секунду

    [Header("Biyu Settings")]
    public GameObject biyuPrefab; // Префаб капсулы (если null — способность без модельки)
    public int damage = 20;
    public float flySpeed = 15f;
    public float attackDistance = 2.5f;
    public float lifeTime = 2f;
}