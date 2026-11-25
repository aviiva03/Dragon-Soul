using System.Collections;
using UnityEngine;
using Cinemachine;

public class DialogueManager : MonoBehaviour {
    public static DialogueManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DialogueUI _dialogueUI;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CanvasGroup _hudCanvasGroup;

    [Header("Settings")]
    [SerializeField] private float _hudFadeDuration = 0.3f;
    [SerializeField] private float _cameraBlendTime = 1.0f;

    public bool IsDialogueActive { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void StartDialogue(DialogueData dialogue, CinemachineVirtualCamera dialogueCamera, GameObject objectToDestroy = null, Transform playerTeleportPoint = null, TalkingAnimator[] charactersToAnimate = null) {
        if (IsDialogueActive) return;
        StartCoroutine(DialogueRoutine(dialogue, dialogueCamera, objectToDestroy, playerTeleportPoint, true, charactersToAnimate));
    }

    public void StartCinematicDialogue(DialogueData dialogue, TalkingAnimator[] charactersToAnimate = null) {
        if (IsDialogueActive) return;
        StartCoroutine(DialogueRoutine(dialogue, null, null, null, false, charactersToAnimate));
    }

    private IEnumerator DialogueRoutine(DialogueData dialogue, CinemachineVirtualCamera dialogueCamera, GameObject objectToDestroy, Transform playerTeleportPoint, bool isStandardDialogue, TalkingAnimator[] charactersToAnimate) {
        IsDialogueActive = true;

        if (_playerController != null) {
            _playerController.ForceStopForCinematic();
            _playerController.enabled = false;
        }

        // Телепорт игрока
        Vector3 originalPosition = Vector3.zero;
        Quaternion originalRotation = Quaternion.identity;
        CharacterController cc = null;

        if (isStandardDialogue && playerTeleportPoint != null && _playerController != null) {
            originalPosition = _playerController.transform.position;
            originalRotation = _playerController.transform.rotation;

            cc = _playerController.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            _playerController.transform.position = playerTeleportPoint.position;
            _playerController.transform.rotation = playerTeleportPoint.rotation;

            if (cc != null) cc.enabled = true;
        }

        if (isStandardDialogue) {
            yield return FadeHUD(1f, 0f, _hudFadeDuration);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // ПЕРЕКЛЮЧАЕМ КАМЕРУ (только через контроллер!)
        CinemachineVirtualCamera currentCamera = dialogueCamera;

        if (currentCamera != null && DialogueCameraController.Instance != null) {
            DialogueCameraController.Instance.SwitchToDialogueCamera(currentCamera, _cameraBlendTime);
            yield return new WaitForSeconds(_cameraBlendTime);
        }

        string currentNodeId = dialogue.startNodeId;

        while (!string.IsNullOrEmpty(currentNodeId) && currentNodeId != "END") {
            DialogueNode node = dialogue.GetNode(currentNodeId);
            if (node == null) break;

            // Смена камеры по имени (тоже только через контроллер)
            if (!string.IsNullOrEmpty(node.overrideCameraName)) {
                GameObject camObj = GameObject.Find(node.overrideCameraName);
                if (camObj != null) {
                    CinemachineVirtualCamera newCam = camObj.GetComponent<CinemachineVirtualCamera>();
                    if (newCam != null && newCam != currentCamera) {
                        currentCamera = newCam;
                        DialogueCameraController.Instance.SwitchToDialogueCamera(currentCamera, _cameraBlendTime);
                        yield return new WaitForSeconds(_cameraBlendTime);
                    }
                }
            } else if (string.IsNullOrEmpty(node.overrideCameraName) && currentCamera != dialogueCamera && dialogueCamera != null) {
                currentCamera = dialogueCamera;
                DialogueCameraController.Instance.SwitchToDialogueCamera(currentCamera, _cameraBlendTime);
                yield return new WaitForSeconds(_cameraBlendTime);
            }

            // Жестикуляция
            if (charactersToAnimate != null) {
                foreach (var character in charactersToAnimate) {
                    if (character != null) character.StopTalking();
                }
                foreach (var character in charactersToAnimate) {
                    if (character != null && character.CharacterName == node.speakerName) {
                        character.StartTalking();
                        break;
                    }
                }
            }

            yield return _dialogueUI.ShowNodeRoutine(node);

            if (node.choices != null && node.choices.Count > 0) {
                int choiceIndex = _dialogueUI.GetSelectedChoiceIndex();
                currentNodeId = node.choices[choiceIndex].nextNodeId;
            } else {
                currentNodeId = node.nextNodeId;
            }
        }

        if (charactersToAnimate != null) {
            foreach (var character in charactersToAnimate) {
                if (character != null) character.StopTalking();
            }
        }

        // ВОЗВРАЩАЕМ КАМЕРУ
        if (DialogueCameraController.Instance != null) {
            DialogueCameraController.Instance.ReturnToPlayerCamera(_cameraBlendTime);
            yield return new WaitForSeconds(_cameraBlendTime);
        }

        if (isStandardDialogue && playerTeleportPoint != null && _playerController != null) {
            if (cc != null) cc.enabled = false;
            _playerController.transform.position = originalPosition;
            _playerController.transform.rotation = originalRotation;
            if (cc != null) cc.enabled = true;
        }

        if (_playerController != null) _playerController.enabled = true;

        if (isStandardDialogue) {
            yield return FadeHUD(0f, 1f, _hudFadeDuration);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!string.IsNullOrEmpty(dialogue.dialogueID) && PlayerProgress.Instance != null) {
            PlayerProgress.Instance.RegisterDialogueCompleted(dialogue.dialogueID);
        }

        IsDialogueActive = false;
        if (objectToDestroy != null) Destroy(objectToDestroy);
    }

    private IEnumerator FadeHUD(float from, float to, float duration) {
        if (_hudCanvasGroup == null) yield break;
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            _hudCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        _hudCanvasGroup.alpha = to;
    }
}