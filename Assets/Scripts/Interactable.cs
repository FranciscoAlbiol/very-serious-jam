using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Camera interactableCamera;

    private Renderer[] renderers;
    private Color[] originalColors;
    public DialogueTrigger trigger;

    public string minigame_to_start;

    void Start()
    {
        
    }

    public void SetHighlight(bool on)
    {
        
    }
}
