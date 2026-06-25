using UnityEngine;

public class DiceFace : MonoBehaviour
{
    public SpriteRenderer faceRenderer;
    public Sprite[] faceSprites;

    private int currentFace = -1;
    public int CurrentFace => currentFace;

    public void ShowFace(int value)
    {
        value = Mathf.Clamp(value, 1, 6);
        if (value == currentFace) return;
        currentFace = value;

        int idx = value - 1;
        if (faceRenderer != null && faceSprites != null && idx < faceSprites.Length)
            faceRenderer.sprite = faceSprites[idx];
    }
}
