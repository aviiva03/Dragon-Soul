using System.Collections;
using UnityEngine;

public class TravelAction : InteractAction {

    [Header("Travel Settings")]
    [Tooltip("Куда телепортировать игрока на новом острове")]
    [SerializeField] private Transform _destinationPoint;

    [Header("UI / Text")]
    [SerializeField] private string _travelText = "ПУТЕШЕСТВИЕ НА РУБИНОВЫЙ ОСТРОВ";
    [SerializeField] private float _fadeDuration = 1f;

    private bool _isTraveling = false;

    public override void Execute() {
        if (_isTraveling) return;
        _isTraveling = true;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null) {
            player.ForceStopForCinematic();
            player.enabled = false;
        }

        StartCoroutine(TravelRoutine(player));
    }

    private IEnumerator TravelRoutine(PlayerController player) {
        // 1. Блокируем управление камерой и курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 2. Затемняем экран (используем универсальный ScreenFaderUI)
        if (ScreenFaderUI.Instance != null) {
            // Нам нужно знать, когда экран станет черным, чтобы телепортировать
            bool isScreenBlack = false;

            ScreenFaderUI.Instance.PlayFade(_travelText, () => {
                isScreenBlack = true; // Экран полностью черный

                // Телепортируем игрока
                if (player != null && _destinationPoint != null) {
                    var cc = player.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;

                    player.transform.position = _destinationPoint.position;
                    player.transform.rotation = _destinationPoint.rotation;

                    if (cc != null) cc.enabled = true;
                }
            });

            // Ждем, пока экран не начнет светлеть
            while (!isScreenBlack) {
                yield return null;
            }
            // Ждем, пока экран снова станет прозрачным (чтобы завершить переход)
            yield return new WaitForSeconds(1.5f);
        }

        // 3. Возвращаем управление
        if (player != null) player.enabled = true;

        _isTraveling = false;

        // Сохраняем игру после перехода
        if (SaveSystem.Instance != null) {
            SaveSystem.Instance.SaveGame();
        }
    }
}