using System.Collections.Generic;

[System.Serializable]
public class GameData {
    // Здесь мы перечисляем только то, что нужно сохранить между сессиями
    public bool hasSword;

    // Пример на будущее: сохранение чекпоинтов
    public string currentCheckpointID;

    // Пример на будущее: убитые боссы
    public List<string> defeatedBosses = new List<string>();
}