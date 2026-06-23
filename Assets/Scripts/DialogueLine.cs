using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueLine
{
    public int dialogueIndex;
    public string characterName;
    public SpriteRenderer characterRenderer;
    public Sprite characterSprite;
    [TextArea(2, 5)]
    public string text;
    public AudioClip voiceLine;
    public UnityEvent onLineShown;

    public bool isChoice;
    public DialogueChoice[] choices;

    public bool continueToNextIndex;
}
