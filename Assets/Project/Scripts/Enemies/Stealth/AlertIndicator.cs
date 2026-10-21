using UnityEngine;

public class AlertIndicator : MonoBehaviour {

    [SerializeField] private GameObject _questionMark;
    [SerializeField] private GameObject _exclamationMark;

    public void ShowQuestion() {
        if (_questionMark != null) _questionMark.SetActive(true);
        if (_exclamationMark != null) _exclamationMark.SetActive(false);
    }

    public void ShowExclamation() {
        if (_questionMark != null) _questionMark.SetActive(false);
        if (_exclamationMark != null) _exclamationMark.SetActive(true);
    }

    public void HideAll() {
        if (_questionMark != null) _questionMark.SetActive(false);
        if (_exclamationMark != null) _exclamationMark.SetActive(false);
    }
}
