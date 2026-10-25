using System.IO;
using UnityEngine;

public static class SaveSystem {

    private static string SavePath => Application.persistentDataPath + "/savegame.json";

    public static void SaveGame() {
        if (PlayerProgress.Instance == null) return;

        // 1. Получаем данные
        GameData data = PlayerProgress.Instance.GetSaveData();

        // 2. Переводим в красивый JSON
        string json = JsonUtility.ToJson(data, true);

        // 3. Записываем в файл
        File.WriteAllText(SavePath, json);

        Debug.Log($"[SaveSystem] Игра сохранена в: {SavePath}");
    }

    public static bool HasSaveGame() {
        return File.Exists(SavePath);
    }

    public static GameData LoadGame() {
        if (!HasSaveGame()) {
            Debug.Log("[SaveSystem] Файл сохранения не найден. Начинаем новую игру.");
            return new GameData(); // Возвращаем пустые данные
        }

        // 1. Читаем файл
        string json = File.ReadAllText(SavePath);

        // 2. Переводим из JSON в объект
        GameData data = JsonUtility.FromJson<GameData>(json);

        Debug.Log("[SaveSystem] Игра загружена.");
        return data;
    }

    // Метод для удаления сейва (например, для кнопки "Новая игра")
    public static void DeleteSave() {
        if (HasSaveGame()) {
            File.Delete(SavePath);
            Debug.Log("[SaveSystem] Сохранение удалено.");
        }
    }
}