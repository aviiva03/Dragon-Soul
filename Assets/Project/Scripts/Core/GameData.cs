using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {
    public bool hasSword;

    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
    public float playerRotY;

    public List<string> destroyedObjectIDs = new List<string>();
    public List<string> defeatedBossIDs = new List<string>();
    public List<string> completedDialogueIDs = new List<string>();

    public Vector3 PlayerPosition {
        get => new Vector3(playerPosX, playerPosY, playerPosZ);
        set { playerPosX = value.x; playerPosY = value.y; playerPosZ = value.z; }
    }

    public Quaternion PlayerRotation {
        get => Quaternion.Euler(0f, playerRotY, 0f);
        set => playerRotY = value.eulerAngles.y;
    }
}