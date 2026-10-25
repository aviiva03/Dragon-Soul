using UnityEngine;

public class SaveDeleter : MonoBehaviour {

    [Header("Временная кнопка для тестов")]
    [SerializeField] private bool _deleteSaveNow = false;

    private void Update() {
        // Если галочка стоит — удаляем сейв и снимаем галочку
        if (_deleteSaveNow) {
            _deleteSaveNow = false;
            SaveSystem.DeleteSave();
            Debug.Log("[SaveDeleter] Сохранение успешно удалено!");
        }
    }
}