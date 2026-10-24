using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour {
    public static StoryManager Instance { get; private set; }

    [Header("Stage 1: Small Enemies")]
    [SerializeField] private GameObject[] _smallEnemies; // Массив мелких врагов (изначально выключены)
    [SerializeField] private GameObject[] _wallCubes; // Кубы, из которых состоит стена

    [Header("Stage 2: Bosses")]
    [SerializeField] private GameObject[] _bosses; // Два босса (изначально выключены)

    [Header("Stage 3: Path")]
    [SerializeField] private GameObject _bridgeBarrier; // Невидимая стена или мост, который исчезнет

    private int _enemiesAlive = 0;
    private int _bossesAlive = 0;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // Вызывается из скрипта осколков (SwordShardsAction) в конце кат-сцены
    public void StartStage1() {
        Debug.Log("[Story] Этап 1: Спавн мелких врагов");

        foreach (var enemy in _smallEnemies) {
            if (enemy != null) {
                enemy.SetActive(true);
                _enemiesAlive++;

                // Подписываемся на смерть каждого врага
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
        // 1. Разрушаем стену (можно добавить эффект взрыва или просто выключить)
        foreach (var cube in _wallCubes) {
            if (cube != null) {
                // Тут можно добавить Rigidbody и силу, чтобы они разлетелись
                cube.SetActive(false);
            }
        }

        yield return new WaitForSeconds(1f); // Небольшая пауза

        // 2. Спавним боссов
        Debug.Log("[Story] Этап 2: Спавн боссов");
        foreach (var boss in _bosses) {
            if (boss != null) {
                boss.SetActive(true);
                _bossesAlive++;

                var health = boss.GetComponent<Health>();
                if (health != null) {
                    health.OnDied += OnBossDied;

                    // РЕГИСТРИРУЕМ ПЛАШКУ ДЛЯ ЭТОГО БОССА
                    if (BossHealthBarManager.Instance != null) {
                        BossHealthBarManager.Instance.RegisterBoss(health, boss.name);
                    }
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
        if (_bridgeBarrier != null) {
            _bridgeBarrier.SetActive(false);
        }
    }
}