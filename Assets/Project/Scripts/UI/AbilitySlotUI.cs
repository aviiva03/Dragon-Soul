using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitySlotUI : MonoBehaviour {
    [Header("UI Elements")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _keyText;
    [SerializeField] private Image _cooldownOverlay;
    [SerializeField] private TMP_Text _cooldownText;

    private AbilityData _ability;
    private AbilityManager _manager;

    public void Setup(AbilityData ability, AbilityManager manager) {
        _ability = ability;
        _manager = manager;

        _iconImage.sprite = ability.icon;
        _keyText.text = ability.inputKey;

        _cooldownOverlay.gameObject.SetActive(false);
        _cooldownText.gameObject.SetActive(false);
    }

    private void Update() {
        if (_ability == null || _manager == null) return;

        float remaining = _manager.GetCooldownRemaining(_ability);

        if (remaining > 0) {
            _cooldownOverlay.gameObject.SetActive(true);
            _cooldownText.gameObject.SetActive(true);

            _cooldownOverlay.fillAmount = _manager.GetCooldownPercent(_ability);
            _cooldownText.text = Mathf.CeilToInt(remaining).ToString();
        } else {
            _cooldownOverlay.gameObject.SetActive(false);
            _cooldownText.gameObject.SetActive(false);
        }
    }
}