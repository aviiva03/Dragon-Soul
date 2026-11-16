using System.Collections;
using UnityEngine;

public class BiyuAbilityInstance : MonoBehaviour {
    private AbilityData _data;
    private Transform _target;

    public void Setup(AbilityData data, Transform target) {
        _data = data;
        _target = target;
        StartCoroutine(AbilitySequence());
    }

    private IEnumerator AbilitySequence() {
        if (_target == null) {
            Destroy(gameObject);
            yield break;
        }

        // Летим к цели
        while (Vector3.Distance(transform.position, _target.position) > _data.attackDistance) {
            if (_target == null) {
                Destroy(gameObject);
                yield break;
            }

            Vector3 direction = (_target.position - transform.position).normalized;
            transform.position += direction * _data.flySpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
            yield return null;
        }

        // Удар
        transform.LookAt(new Vector3(_target.position.x, transform.position.y, _target.position.z));
        yield return new WaitForSeconds(0.2f);

        // Наносим основной урон
        if (_target != null && _target.TryGetComponent(out IDamageable damageable)) {
            damageable.TakeDamage(_data.damage, _target.position);
        }

        // --- НАКЛАДЫВАЕМ ДЕБАФФ ---
        if (_data.applyDebuff && _target != null) {
            // Ищем компонент BiyuDebuff на враге, если нет — создаем
            BiyuDebuff debuff = _target.GetComponent<BiyuDebuff>();
            if (debuff == null) {
                debuff = _target.gameObject.AddComponent<BiyuDebuff>();
            }
            debuff.Setup(_data.debuffDuration, _data.debuffDamagePercent);
        }

        yield return new WaitForSeconds(_data.lifeTime);
        Destroy(gameObject);
    }
}