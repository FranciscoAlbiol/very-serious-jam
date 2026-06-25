using UnityEngine;

public class DiceFace : MonoBehaviour
{
    private static readonly Quaternion[] faceRotations = new Quaternion[6]
    {
        Quaternion.Euler(0,   0,   180),
        Quaternion.Euler(0,   0,   -90),
        Quaternion.Euler(0, 0,   0),
        Quaternion.Euler(0, 0,   90),
        Quaternion.Euler(-90, 0,  180),
        Quaternion.Euler(90, 0, 180),
    };

    private int currentFace = -1;
    public int CurrentFace => currentFace;

    public void ShowFace(int value)
    {
        value = Mathf.Clamp(value, 1, 6);
        currentFace = value;
        transform.localRotation = faceRotations[value - 1];
    }

    public Quaternion GetFaceRotation(int value)
    {
        value = Mathf.Clamp(value, 1, 6);
        return faceRotations[value - 1];
    }
}
