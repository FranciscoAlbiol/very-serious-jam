using UnityEngine.Events;

[System.Serializable]
public class DialogueChoiceOption
{
    public string label;
    public UnityEvent onChosen;
    public int nextIndex = -1;
}

[System.Serializable]
public class DialogueChoice
{
    public DialogueChoiceOption optionA;
    public DialogueChoiceOption optionB;
}
