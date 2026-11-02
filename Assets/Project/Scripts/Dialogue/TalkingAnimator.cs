using UnityEngine;

public class TalkingAnimator : MonoBehaviour {
    [Header("Identity")]
    [Tooltip("Имя этого персонажа (должно точно совпадать с Speaker Name в диалоге)")]
    [SerializeField] private string _characterName = "Дэшэн";

    [Header("Animation Settings")]
    [SerializeField] private string _talkTriggerName = "TalkGesture";

    [Tooltip("Шанс, что при начале реплики персонаж жестикулирует (0-100%)")]
    [Range(0, 100)]
    [SerializeField] private int _gestureChance = 50;

    public string CharacterName => _characterName;

    private Animator _animator;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    public void StartTalking() {
        int roll = Random.Range(0, 100);
        if (roll < _gestureChance) {
            if (_animator != null) {
                _animator.SetTrigger(_talkTriggerName);
            }
        }
    }

    public void StopTalking() {
        // Анимация доигрывает сама, ничего не сбрасываем
    }
}