using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Camera interactableCamera;

    private Renderer[] renderers;
    private Color[] originalColors;
    public DialogueTrigger trigger;

    void Start()
    {
        trigger = GetComponent<DialogueTrigger>();
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;
    }

    public void SetHighlight(bool on)
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = on ? Color.white : originalColors[i];
    }
}
