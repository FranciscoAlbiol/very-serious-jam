using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    public SpriteRenderer characterRenderer;
    public Sprite characterSprite;
    [TextArea(2, 5)]
    public string text;
    public AudioClip voiceLine;
    public UnityEvent onLineFinished;
}
