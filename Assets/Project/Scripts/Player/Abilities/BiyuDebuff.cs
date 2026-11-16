using System.Collections;
using UnityEngine;

public class BiyuDebuff : MonoBehaviour {
    private Health _enemyHealth;
    private float _damagePerTick;
    private float _duration;
    private float _tickInterval = 1f; // Урон раз в секунду

    public void Setup(float duration, float damagePercentPerTick) {
        _enemyHealth = GetComponent<Health>();
        if (_enemyHealth == null) return;

        _duration = duration;

        // Урон = процент от МАКСИМАЛЬНОГО здоровья
        _damagePerTick = _enemyHealth.Max * (damagePercentPerTick / 100f);

        // Если дебафф уже висит — просто обновляем таймер
        BiyuDebuff existingDebuff = GetComponent<BiyuDebuff>();
        if (existingDebuff != null && existingDebuff != this) {
            Destroy(existingDebuff); // Пересоздаем заново
        }

        StartCoroutine(DebuffRoutine());
    }

    private IEnumerator DebuffRoutine() {
        float timer = 0f;

        while (timer < _duration) {
            // Ждем 1 секунду
            yield return new WaitForSeconds(_tickInterval);

            // Если враг уже умер, прекращаем
            if (_enemyHealth == null || !_enemyHealth.IsAlive) {
                Destroy(this);
                yield break;
            }

            // Наносим урон
            int damage = Mathf.RoundToInt(_damagePerTick);
            _enemyHealth.TakeDamage(damage, transform.position);

            Debug.Log($"[Debuff] Бию нанесла {damage} урона (5% от макс. HP). Осталось: {_duration - timer - 1} сек.");

            timer += _tickInterval;
        }

        Destroy(this);
    }
}