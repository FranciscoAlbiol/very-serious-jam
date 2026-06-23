using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueLine[] lines;

    private int currentIndex = 0;
    private HashSet<int> seenIndices = new HashSet<int>();

    public void StartDialogue()
    {
        DialogueManager.instance.StartDialogue(lines, currentIndex, this);
    }

    public void SetIndex(int index)
    {
        currentIndex = index;
    }

    public int GetIndex()
    {
        return currentIndex;
    }

    public void MarkIndexSeen(int index)
    {
        seenIndices.Add(index);
    }

    public bool HasSeenIndex(int index)
    {
        return seenIndices.Contains(index);
    }
}
