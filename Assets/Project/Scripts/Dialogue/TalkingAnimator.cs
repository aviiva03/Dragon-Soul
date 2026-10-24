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
    private bool _isCurrentlyTalking = false;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Вызывается DialogueManager'ом, когда начинается реплика этого персонажа.
    /// </summary>
    public void StartTalking() {
        _isCurrentlyTalking = true;

        // Бросаем кубик: жестикулировать или нет?
        int roll = Random.Range(0, 100);
        if (roll < _gestureChance) {
            if (_animator != null) {
                _animator.SetTrigger(_talkTriggerName);
            }
        }
    }

    /// <summary>
    /// Вызывается, когда реплика переходит к другому персонажу или диалог заканчивается.
    /// </summary>
    public void StopTalking() {
        _isCurrentlyTalking = false;
        // Мы НЕ сбрасываем триггер и не останавливаем анимацию. 
        // Если жест сейчас играет, он просто доиграет сам и вернется в Idle.
    }
}