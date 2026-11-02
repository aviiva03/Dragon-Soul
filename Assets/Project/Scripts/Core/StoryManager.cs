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
    [SerializeField] private string _wallID = "Wall_01";

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
        // ѕровер€ем, сломана ли стена при загрузке игры
        if (PlayerProgress.Instance != null && PlayerProgress.Instance.IsObjectDestroyed(_wallID)) {
            if (_wallParent != null) _wallParent.SetActive(false);
        }
    }

    public void StartStage1() {
        if (PlayerProgress.Instance != null && PlayerProgress.Instance.IsObjectDestroyed(_wallID)) return;

        ClearStealthArea();

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
            StartCoroutine(BreakWallRoutine());
        }
    }

    private IEnumerator BreakWallRoutine() {
        if (_wallParent != null) {
            var wallState = _wallParent.GetComponent<WorldStateObject>();
            if (wallState != null) {
                wallState.DestroyAndSave();
            } else {
                _wallParent.SetActive(false);
                if (PlayerProgress.Instance != null) PlayerProgress.Instance.RegisterDestroyedObject(_wallID);
            }
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < _bosses.Length; i++) {
            GameObject boss = _bosses[i];
            string bossID = (i < _bossIDs.Length) ? _bossIDs[i] : boss.name;

            if (boss != null) {
                if (PlayerProgress.Instance != null && PlayerProgress.Instance.IsBossDefeated(bossID)) continue;

                boss.SetActive(true);
                _bossesAlive++;

                var health = boss.GetComponent<Health>();
                if (health != null) {
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