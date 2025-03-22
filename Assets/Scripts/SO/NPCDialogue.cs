using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "Dialogue/NPCDialogue")]
public class NPCDialogue : ScriptableObject
{
    public DialogueLine[] dialogueLines;

    public DialogueLine DialogueLine
    {
        get => default;
        set
        {
        }
    }

    //通过ID获取指定台词
    public DialogueLine GetDialogueById(int id)
    {
        foreach (var line in dialogueLines)
        {
            if (line.id == id)
            {
                return line;
            }
        }
        return null;
    }
}

[System.Serializable]
public class DialogueLine
{
    public int id;
    [TextArea(2, 5)]
    public string text;
}
