using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    public Sprite characterSprite;
    [TextArea(2, 5)]
    public string text;
    public AudioClip voiceLine;
}
