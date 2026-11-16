using System.Collections;
using UnityEngine;

public class StoryManager : MonoBehaviour {
    public static StoryManager Instance { get; private set; }

    [Header("Stealth Guards To Remove")]
    [SerializeField] private GameObject[] _stealthGuardsToRemove;

    [Header("Stage 1: Small Enemies")]
    [SerializeField] private GameObject[] _smallEnemies;

    [Header("Wall Destruction")]
    [SerializeField] private GameObject _wallParent;

    [Header("Stage 2: Bosses")]
    [SerializeField] private GameObject[] _bosses;
    [SerializeField] private string[] _bossIDs;

    private int _enemiesAlive = 0;
    private int _bossesAlive = 0;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start() {
        if (PlayerProgress.Instance == null) return;

        // Если арена пройдена (боссы мертвы) - скрываем стену и ничего не спавним
        if (PlayerProgress.Instance.IsArenaCleared) {
            if (_wallParent != null) _wallParent.SetActive(false);
            return;
        }

        // Если арена НЕ пройдена, стена должна быть на месте (возвращаем её при перезаходе)
        if (_wallParent != null) _wallParent.SetActive(true);
    }

    public void StartStage1() {
        if (PlayerProgress.Instance != null && PlayerProgress.Instance.IsArenaCleared) return;

        ClearStealthArea();

        // Спавним мелких врагов
        foreach (var enemy in _smallEnemies) {
            if (enemy != null) {
                enemy.SetActive(true);
                _enemiesAlive++;
                var health = enemy.GetComponent<Health>();
                if (health != null) {
                    health.Revive(); // Восстанавливаем здоровье мелким
                    health.OnDied -= OnSmallEnemyDied;
                    health.OnDied += OnSmallEnemyDied;
                }
            }
        }
    }

    private void OnSmallEnemyDied() {
        _enemiesAlive--;
        if (_enemiesAlive <= 0) {
            StartCoroutine(BreakWallRoutine());
        }
    }

    private IEnumerator BreakWallRoutine() {
        // Просто скрываем стену ВИЗУАЛЬНО. НЕ сохраняем это в сейв!
        if (_wallParent != null) {
            _wallParent.SetActive(false);
        }

        yield return new WaitForSeconds(1f);

        SpawnBosses();
    }

    public void SpawnBosses() {
        _bossesAlive = 0;

        if (BossHealthBarManager.Instance != null) {
            BossHealthBarManager.Instance.ClearAllBars();
        }

        for (int i = 0; i < _bosses.Length; i++) {
            GameObject boss = _bosses[i];
            string bossID = (i < _bossIDs.Length) ? _bossIDs[i] : boss.name;

            if (boss != null) {
                boss.SetActive(true);
                _bossesAlive++;

                var health = boss.GetComponent<Health>();
                if (health != null) {
                    health.Revive(); // Восстанавливаем здоровье боссам

                    health.OnDied -= () => OnBossDied(bossID);
                    health.OnDied += () => OnBossDied(bossID);

                    if (BossHealthBarManager.Instance != null) {
                        BossHealthBarManager.Instance.RegisterBoss(health, boss.name);
                    }
                }

                var bossController = boss.GetComponent<BossController>();
                if (bossController != null) bossController.ForceCombat();
            }
        }
    }

    private void OnBossDied(string bossID) {
        _bossesAlive--;
        if (PlayerProgress.Instance != null) PlayerProgress.Instance.RegisterBossDefeated(bossID);

        // Вот ТУТ мы сохраняем, что арена пройдена (боссы мертвы)
        if (_bossesAlive <= 0) {
            if (PlayerProgress.Instance != null) PlayerProgress.Instance.SetArenaCleared();
            if (_wallParent != null) _wallParent.SetActive(false);
        }
    }

    public void ResetArena() {
        if (PlayerProgress.Instance == null || PlayerProgress.Instance.IsArenaCleared) return;

        if (BossHealthBarManager.Instance != null) {
            BossHealthBarManager.Instance.ClearAllBars();
        }

        foreach (var boss in _bosses) {
            if (boss != null) boss.SetActive(false);
        }
        _bossesAlive = 0;
    }

    public void RespawnBossesOnly() {
        if (PlayerProgress.Instance == null || PlayerProgress.Instance.IsArenaCleared) return;

        // Если стена скрыта (мы дошли до боссов), то спавним боссов
        if (_wallParent == null || !_wallParent.activeSelf) {
            SpawnBosses();
        }
    }

    private void ClearStealthArea() {
        if (_stealthGuardsToRemove != null) {
            foreach (var guard in _stealthGuardsToRemove) {
                if (guard != null) {
                    var stateObj = guard.GetComponent<WorldStateObject>();
                    if (stateObj != null) stateObj.DestroyAndSave();
                    else Destroy(guard);
                }
            }
        }
    }
}