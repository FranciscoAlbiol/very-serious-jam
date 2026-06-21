using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueLine[] lines;

    public void StartDialogue()
    {
        DialogueManager.instance.StartDialogue(lines);
    }
}
