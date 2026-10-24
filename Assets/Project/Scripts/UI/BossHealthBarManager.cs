using System.Collections.Generic;
using UnityEngine;

public class BossHealthBarManager : MonoBehaviour {
    public static BossHealthBarManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Transform _container; // Пустышка, куда будут добавляться плашки
    [SerializeField] private GameObject _healthBarPrefab; // Префаб одной плашки

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void RegisterBoss(Health bossHealth, string bossName) {
        if (_healthBarPrefab == null || _container == null) return;

        // Создаем новую плашку для этого босса
        GameObject barObj = Instantiate(_healthBarPrefab, _container);
        BossHealthBarUI barUI = barObj.GetComponent<BossHealthBarUI>();

        if (barUI != null) {
            barUI.Setup(bossHealth, bossName);
        }
    }
}