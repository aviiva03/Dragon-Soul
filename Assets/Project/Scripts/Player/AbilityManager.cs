using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour {
    public static AbilityManager Instance { get; private set; }

    [Header("All Abilities")]
    [SerializeField] private AbilityData[] _allAbilities;
    [SerializeField] private Transform _spawnPoint;

    private Dictionary<AbilityData, float> _cooldowns = new Dictionary<AbilityData, float>();
    public List<AbilityData> UnlockedAbilities { get; private set; } = new List<AbilityData>();

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start() {
        foreach (var ability in _allAbilities) {
            _cooldowns[ability] = 0f;

            // Загружаем открытые способности из сейва
            if (PlayerProgress.Instance != null && PlayerProgress.Instance.IsAbilityUnlocked(ability.abilityID)) {
                UnlockedAbilities.Add(ability);
            }
        }

        // Обновляем UI после загрузки
        if (AbilityUIManager.Instance != null) {
            AbilityUIManager.Instance.RefreshUI();
        }
    }

    private void Update() {
        // Обновляем таймеры кулдаунов
        List<AbilityData> keys = new List<AbilityData>(_cooldowns.Keys);
        foreach (var ability in keys) {
            if (_cooldowns[ability] > 0) {
                _cooldowns[ability] -= Time.deltaTime;
                if (_cooldowns[ability] < 0) _cooldowns[ability] = 0;
            }
        }

        // Читаем ввод из Input System
        if (Keyboard.current != null) {
            // Способность Q (Атака Бию)
            if (Keyboard.current.qKey.wasPressedThisFrame) TryUseAbilityByKey("Q");
            // Способность E (Оглушение)
            if (Keyboard.current.eKey.wasPressedThisFrame) TryUseAbilityByKey("E");
            // Способность R (Щит)
            if (Keyboard.current.rKey.wasPressedThisFrame) TryUseAbilityByKey("R");
        }
    }

    public void UnlockAbility(string abilityID) {
        AbilityData ability = System.Array.Find(_allAbilities, a => a.abilityID == abilityID);
        if (ability != null && !UnlockedAbilities.Contains(ability)) {
            UnlockedAbilities.Add(ability);
            if (AbilityUIManager.Instance != null) AbilityUIManager.Instance.RefreshUI();
        }
    }

    private void TryUseAbilityByKey(string key) {
        AbilityData ability = UnlockedAbilities.Find(a => a.inputKey == key);
        if (ability != null) TryUseAbility(ability);
    }

    public float GetCooldownRemaining(AbilityData ability) => _cooldowns[ability];
    public float GetCooldownPercent(AbilityData ability) => _cooldowns[ability] / ability.cooldown;

    private void TryUseAbility(AbilityData ability) {
        if (_cooldowns[ability] > 0) return;

        if (ability.biyuPrefab != null) {
            Collider[] enemies = Physics.OverlapSphere(transform.position, ability.abilityRange);
            Transform nearestEnemy = null;
            float minDistance = float.MaxValue;

            foreach (var col in enemies) {
                if (col.CompareTag("Enemy")) {
                    float dist = Vector3.Distance(transform.position, col.transform.position);
                    if (dist < minDistance) {
                        minDistance = dist;
                        nearestEnemy = col.transform;
                    }
                }
            }

            if (nearestEnemy == null) {
                Debug.Log($"[Ability] {ability.abilityName}: Нет врагов поблизости.");
                return;
            }

            Vector3 spawnPos = transform.position - transform.forward * 3f + Vector3.up * 2f;
            if (_spawnPoint != null) spawnPos = _spawnPoint.position;

            GameObject biyuObj = Instantiate(ability.biyuPrefab, spawnPos, Quaternion.identity);

            BiyuAbilityInstance biyuLogic = biyuObj.GetComponent<BiyuAbilityInstance>();
            if (biyuLogic != null) biyuLogic.Setup(ability, nearestEnemy);
        } else {
            Debug.Log($"[Ability] {ability.abilityName} активирована! (Эффект пока не реализован)");
        }

        _cooldowns[ability] = ability.cooldown;
    }
}