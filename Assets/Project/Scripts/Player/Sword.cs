using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour {

    [Header("References")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _appearSound;
    [SerializeField] private AudioClip _disappearSound;

    [Header("Positions (Sockets)")]
    [SerializeField] private Transform _handSocket;
    [SerializeField] private Transform _backSocket;

    [Header("Settings")]
    [SerializeField] private float _idleTimeToSheath = 3f;

    private Coroutine _sheathRoutine;
    private bool _isInHand = false;
    private bool _hasSword = false; // Есть ли меч у игрока

    private void Start() {
        // Проверяем прогресс: если меч еще не получен — полностью скрываем его
        if (PlayerProgress.Instance != null && !PlayerProgress.Instance.HasSword) {
            gameObject.SetActive(false);
            _hasSword = false;
            return;
        }

        // Если меч уже есть (загрузились с сейва) — ставим за спину
        _hasSword = true;
        EquipSword();
    }

    // Вызывается, когда мы получаем меч (из кат-сцены)
    public void ShowOnPickup() {
        _hasSword = true;
        gameObject.SetActive(true); // Включаем объект

        // Сначала появляется в руке
        Appear();
        AttackFinished();
    }

    private void EquipSword() {
        gameObject.SetActive(true);
        if (_backSocket != null) {
            transform.SetParent(_backSocket);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        _isInHand = false;
    }

    public void Appear() {
        if (!_hasSword) return; // Если меча нет, ничего не делаем

        if (_sheathRoutine != null) StopCoroutine(_sheathRoutine);

        if (_audioSource != null && _appearSound != null)
            _audioSource.PlayOneShot(_appearSound);

        if (_handSocket != null) {
            transform.SetParent(_handSocket);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        _isInHand = true;
    }

    public void AttackFinished() {
        if (!_hasSword || !_isInHand) return;

        if (_sheathRoutine != null) StopCoroutine(_sheathRoutine);
        _sheathRoutine = StartCoroutine(SheathTimerRoutine());
    }

    private IEnumerator SheathTimerRoutine() {
        yield return new WaitForSeconds(_idleTimeToSheath);

        if (_audioSource != null && _disappearSound != null)
            _audioSource.PlayOneShot(_disappearSound);

        if (_backSocket != null) {
            transform.SetParent(_backSocket);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        _isInHand = false;
        _sheathRoutine = null;
    }
}