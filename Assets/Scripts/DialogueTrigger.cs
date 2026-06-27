using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueBlock[] blocks;
    public bool isSpecial;
    public int currentIndex = 0;
    private HashSet<int> seenIndices = new HashSet<int>();

    public void StartDialogue(int index)
    {
        DialogueManager.instance.StartBlock(blocks, index, this);
    }

    public void StartDialogue()
    {
        if (blocks == null || blocks.Length == 0) return;
        DialogueManager.instance.StartBlock(blocks, currentIndex, this);
    }

    public DialogueBlock GetBlock(int index)
    {
        foreach (DialogueBlock block in blocks)
            if (block.index == index) return block;
        return null;
    }

    public void SetIndex(int index) => currentIndex = index;

    public int GetIndex() => currentIndex;

    public void MarkIndexSeen(int index) => seenIndices.Add(index);

    public bool HasSeenIndex(int index) => seenIndices.Contains(index);
}
