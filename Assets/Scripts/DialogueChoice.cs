using UnityEngine.Events;

[System.Serializable]
public class DialogueChoice
{
    public string label;
    public UnityEvent onChosen;
    public int nextLineIndex = -1;
}
