using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _speakerNameText;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private GameObject _continueHint;

    [Header("Choices")]
    [SerializeField] private Transform _choicesContainer;
    [SerializeField] private GameObject _choiceButtonPrefab;

    [Header("Settings")]
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private float _typeSpeed = 0.03f;

    private bool _isTyping;
    private bool _advanceRequested;
    private bool _isWaitingForChoice;
    private int _selectedChoiceIndex = -1;

    private void Awake() {
        _canvasGroup.alpha = 0f;
        _continueHint.SetActive(false);
        _choicesContainer.gameObject.SetActive(false);
    }

    public IEnumerator ShowNodeRoutine(DialogueNode node) {
        gameObject.SetActive(true);

        _dialogueText.text = "";
        _continueHint.SetActive(false);
        _choicesContainer.gameObject.SetActive(false);
        ClearChoiceButtons();

        _advanceRequested = false;
        _isWaitingForChoice = false;
        _selectedChoiceIndex = -1;

        if (string.IsNullOrEmpty(node.speakerName)) {
            _speakerNameText.gameObject.SetActive(false);
        } else {
            _speakerNameText.gameObject.SetActive(true);
            _speakerNameText.text = node.speakerName;
        }

        yield return FadeRoutine(0f, 1f);

        _isTyping = true;
        foreach (char letter in node.text.ToCharArray()) {
            if (_advanceRequested) {
                _dialogueText.text = node.text;
                break;
            }
            _dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(_typeSpeed);
        }
        _isTyping = false;
        _dialogueText.text = node.text;

        if (node.choices != null && node.choices.Count > 0) {
            ShowChoices(node.choices);
            _isWaitingForChoice = true;

            // Ждем, пока не выберут кнопку
            while (_selectedChoiceIndex == -1) {
                yield return null;
            }

            Debug.Log($"[UI] Игрок выбрал вариант: {_selectedChoiceIndex}");
            _isWaitingForChoice = false;
            _choicesContainer.gameObject.SetActive(false);
        } else {
            _continueHint.SetActive(true);
            while (!_advanceRequested) {
                yield return null;
            }
        }

        yield return FadeRoutine(1f, 0f);
        gameObject.SetActive(false);
    }

    private void Update() {
        if (!gameObject.activeSelf) return;

        // Если ждем выбора кнопки — игнорируем клики и пробел
        if (_isWaitingForChoice) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            _advanceRequested = true;
        }
    }

    private void ShowChoices(List<DialogueChoice> choices) {
        _choicesContainer.gameObject.SetActive(true);

        for (int i = 0; i < choices.Count; i++) {
            int index = i;
            GameObject btnObj = Instantiate(_choiceButtonPrefab, _choicesContainer);

            TMP_Text btnText = btnObj.GetComponentInChildren<TMP_Text>();
            if (btnText != null) btnText.text = choices[i].choiceText;

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null) {
                btn.onClick.AddListener(() => {
                    _selectedChoiceIndex = index;
                    Debug.Log($"[UI] Кнопка нажата! Индекс: {index}");
                });
            } else {
                Debug.LogError("На префабе кнопки НЕТ компонента Button!");
            }
        }
    }

    private void ClearChoiceButtons() {
        foreach (Transform child in _choicesContainer) {
            Destroy(child.gameObject);
        }
    }

    public int GetSelectedChoiceIndex() => _selectedChoiceIndex;

    private IEnumerator FadeRoutine(float from, float to) {
        float elapsed = 0f;
        while (elapsed < _fadeDuration) {
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / _fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = to;
    }
}