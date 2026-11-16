using System.Collections.Generic;
using UnityEngine;

public class BossHealthBarManager : MonoBehaviour {
    public static BossHealthBarManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Transform _container;
    [SerializeField] private GameObject _healthBarPrefab;
    [SerializeField] private float _showDistance = 15f;

    private Transform _playerTransform;
    private List<BossHealthBarUI> _activeBars = new List<BossHealthBarUI>();

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerTransform = player.transform;
    }

    public void RegisterBoss(Health bossHealth, string bossName) {
        if (_healthBarPrefab == null || _container == null) return;

        GameObject barObj = Instantiate(_healthBarPrefab, _container);
        BossHealthBarUI barUI = barObj.GetComponent<BossHealthBarUI>();

        if (barUI != null) {
            // Передаем Transform босса (компонент Health висит на нем же)
            barUI.Setup(bossHealth, bossName, bossHealth.transform, _playerTransform);
            _activeBars.Add(barUI);
        }
    }

    public void ClearAllBars() {
        if (_container == null) return;

        _activeBars.Clear();
        for (int i = _container.childCount - 1; i >= 0; i--) {
            Destroy(_container.GetChild(i).gameObject);
        }
    }
}