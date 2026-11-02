using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour {
    public static InteractionPromptUI Instance { get; private set; }

    [SerializeField] private GameObject _promptPanel;
    [SerializeField] private TMP_Text _promptText;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        Hide();
    }

    public void Show(string text) {
        _promptText.text = $"[F] {text}";
        _promptPanel.SetActive(true);
    }

    public void Hide() {
        _promptPanel.SetActive(false);
    }
}