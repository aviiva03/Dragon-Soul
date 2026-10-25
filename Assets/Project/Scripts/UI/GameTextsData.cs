using UnityEngine;

[CreateAssetMenu(fileName = "GameTexts", menuName = "Game/Texts Data")]
public class GameTextsData : ScriptableObject {
    [Header("Death & Respawn")]
    public string DeathText = "бш онцхакх";
    public string FallText = "оюдемхе б аегдмс";

    [Header("Stealth")]
    public string DetectedText = "бюя гюлерхкх!";

    [Header("Other")]
    public string DefaultRespawnText = "бнгбпюыемхе...";
}