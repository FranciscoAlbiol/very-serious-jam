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

    public void AdvanceAndRestart()
    {
        currentIndex++;

        bool hasNext = false;
        foreach (DialogueLine line in lines)
        {
            if (line.dialogueIndex == currentIndex)
            {
                hasNext = true;
                break;
            }
        }

        if (hasNext)
        {
            DialogueManager.instance.StartDialogue(lines, currentIndex, this);
        }
        else
        {
            PlayerInteraction interaction = FindFirstObjectByType<PlayerInteraction>();
            if (interaction != null)
                interaction.EndInteraction();
        }
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
