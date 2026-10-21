using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject {
    public string startNodeId;
    public List<DialogueNode> nodes = new List<DialogueNode>();

    public DialogueNode GetNode(string id) {
        return nodes.Find(n => n.nodeId == id);
    }
}

[Serializable]
public class DialogueNode {
    public string nodeId;
    public string speakerName;
    [TextArea(3, 8)]
    public string text;
    public List<DialogueChoice> choices = new List<DialogueChoice>();
    public string nextNodeId;
}

[Serializable]
public class DialogueChoice {
    public string choiceText;
    public string nextNodeId;
}