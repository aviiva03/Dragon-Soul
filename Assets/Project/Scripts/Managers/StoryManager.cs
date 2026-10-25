using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour {
    public static StoryManager Instance { get; private set; }

    [Header("Stage 1: Small Enemies")]
    [SerializeField] private GameObject[] _smallEnemies;
    [SerializeField] private GameObject[] _wallCubes;

    [Header("Stage 2: Bosses")]
    [SerializeField] private GameObject[] _bosses;

    [Header("Stage 3: Path")]
    [SerializeField] private GameObject _bridgeBarrier;

    private int _enemiesAlive = 0;
    private int _bossesAlive = 0;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void StartStage1() {
        Debug.Log("[Story] Этап 1: Спавн мелких врагов");
        foreach (var enemy in _smallEnemies) {
            if (enemy != null) {
                enemy.SetActive(true);
                _enemiesAlive++;
                var health = enemy.GetComponent<Health>();
                if (health != null) health.OnDied += OnSmallEnemyDied;
            }
        }
    }

    private void OnSmallEnemyDied() {
        _enemiesAlive--;
        if (_enemiesAlive <= 0) {
            Debug.Log("[Story] Все мелкие враги убиты! Разрушаем стену.");
            StartCoroutine(BreakWallRoutine());
        }
    }

    private IEnumerator BreakWallRoutine() {
        foreach (var cube in _wallCubes) {
            if (cube != null) cube.SetActive(false);
        }

        yield return new WaitForSeconds(1f);

        Debug.Log("[Story] Этап 2: Спавн боссов");
        foreach (var boss in _bosses) {
            if (boss != null) {
                boss.SetActive(true);
                _bossesAlive++;

                var health = boss.GetComponent<Health>();
                if (health != null) {
                    health.OnDied += OnBossDied;
                    if (BossHealthBarManager.Instance != null) {
                        BossHealthBarManager.Instance.RegisterBoss(health, boss.name);
                    }
                }

                var bossController = boss.GetComponent<BossController>();
                if (bossController != null) {
                    // ПРОСТО ПЕРЕКЛЮЧАЕМ В БОЙ
                    bossController.ForceCombat();
                }
            }
        }
    }

    private void OnBossDied() {
        _bossesAlive--;
        if (_bossesAlive <= 0) {
            Debug.Log("[Story] Боссы побеждены! Путь открыт.");
            OpenPath();
        }
    }

    private void OpenPath() {
        if (_bridgeBarrier != null) _bridgeBarrier.SetActive(false);
    }
}