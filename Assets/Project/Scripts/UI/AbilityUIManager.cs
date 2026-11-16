using System.Collections.Generic;
using UnityEngine;

public class AbilityUIManager : MonoBehaviour {
    public static AbilityUIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform _container;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private AbilityManager _abilityManager;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void RefreshUI() {
        // Очищаем старые иконки
        foreach (Transform child in _container) {
            Destroy(child.gameObject);
        }

        // Создаем новые для всех открытых способностей
        if (_abilityManager != null) {
            foreach (var ability in _abilityManager.UnlockedAbilities) {
                GameObject slotObj = Instantiate(_slotPrefab, _container);
                AbilitySlotUI slotUI = slotObj.GetComponent<AbilitySlotUI>();
                if (slotUI != null) slotUI.Setup(ability, _abilityManager);
            }
        }
    }
}