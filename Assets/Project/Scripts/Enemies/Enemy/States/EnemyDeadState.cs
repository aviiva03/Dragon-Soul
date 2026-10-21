using UnityEngine;

public class EnemyDeadState : State {

    private readonly EnemyController _enemy;
    private float _timer;
    private const float DeathDelay = 3f;

    public EnemyDeadState(EnemyController enemy) {
        _enemy = enemy;
    }

    public override void Enter() {
        // Останавливаем всё
        _enemy.StopMoving();
        _enemy.Agent.enabled = false;

        // Отключаем коллайдеры (чтобы труп не блокировал)
        var colliders = _enemy.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
            col.enabled = false;

        // Отключаем Animator-параметры движения
        _enemy.Animator.SetBool("isIdle", false);
        _enemy.Animator.SetBool("isWalking", false);
        _enemy.Animator.SetBool("isRunning", false);

        // Триггер смерти
        _enemy.Animator.SetTrigger("death");

        _timer = 0f;
    }

    public override void Update() {
        _timer += Time.deltaTime;

        if (_timer >= DeathDelay) {
            Object.Destroy(_enemy.gameObject);
        }
    }

    public override void Exit() { }
}