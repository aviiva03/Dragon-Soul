using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaminaBarUI : MonoBehaviour {

    [Header("References")]
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _text;

    [Header("Settings")]
    [SerializeField] private bool _showText = true;

    private void Update() {
        if (_stats == null) return;

        float current = _stats.CurrentStamina;
        float max = _stats.MaxStamina;
        float percent = max > 0 ? current / max : 0f;

        if (_fillImage != null)
            _fillImage.fillAmount = percent;

        if (_showText && _text != null)
            _text.text = $"{Mathf.RoundToInt(current)} / {Mathf.RoundToInt(max)}";
    }
}