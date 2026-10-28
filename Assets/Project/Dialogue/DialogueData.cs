using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject {

    [Header("Identity")]
    [Tooltip("Уникальный ID диалога (например, Dragon_Intro, Temple_Memory)")]
    public string dialogueID;

    [Header("Settings")]
    public string startNodeId;

    [Header("Nodes")]
    public List<DialogueNode> nodes = new List<DialogueNode>();

    public DialogueNode GetNode(string id) {
        return nodes.Find(n => n.nodeId == id);
    }
}

[Serializable]
public class DialogueNode {
    [Header("Node Info")]
    public string nodeId;
    public string speakerName;

    [Header("Text")]
    [TextArea(3, 8)]
    public string text;

    [Header("Cinematic (Optional)")]
    [Tooltip("Точное имя камеры в иерархии")]
    public string overrideCameraName;

    [Header("Choices & Flow")]
    public List<DialogueChoice> choices = new List<DialogueChoice>();
    public string nextNodeId;
}

[Serializable]
public class DialogueChoice {
    [Tooltip("Текст, который увидит игрок на кнопке")]
    public string choiceText;

    [Tooltip("ID узла, на который перейдет диалог при выборе")]
    public string nextNodeId;
}