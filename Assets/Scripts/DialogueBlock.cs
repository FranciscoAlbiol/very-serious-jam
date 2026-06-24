using UnityEngine;

[System.Serializable]
public class DialogueBlock
{
    public int index;
    public DialogueLine[] lines;

    public bool hasChoice;
    public DialogueChoice choice;

    public bool continueToNextBlock;
    public int nextBlockIndex;
}
