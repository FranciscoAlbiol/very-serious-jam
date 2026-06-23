using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueLine[] lines;

    private int currentIndex = 0;

    public void StartDialogue()
    {
        DialogueManager.instance.StartDialogue(lines, currentIndex);
    }

    public void SetIndex(int index)
    {
        currentIndex = index;
    }

    public int GetIndex()
    {
        return currentIndex;
    }
}
