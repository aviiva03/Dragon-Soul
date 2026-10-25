using System.Collections.Generic;

[System.Serializable]
public class GameData {
    public bool hasSword;
    public string currentCheckpointID;
    public List<string> destroyedObjectIDs = new List<string>();
    public List<string> defeatedBossIDs = new List<string>();
}