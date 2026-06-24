using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpinningWheel : MonoBehaviour
{
    [Header("UI Elements")]
    public Image option2Image;
    public RectTransform wheelTransform;

    [Header("Settings")]
    [Range(0f, 1f)] 
    public float option2Chance = 0.1f;
    public float spinDuration = 3f;

    private bool isSpinning = false;

    void Start()
    {
        UpdateWheelVisuals();
        StartSpin();
    }

    // Call this whenever you change the percentage dynamically
    public void UpdateWheelVisuals()
    {
        if (option2Image != null)
        {
            option2Image.fillAmount = option2Chance;
        }
    }

    public void StartSpin()
    {
        if (!isSpinning)
        {
            StartCoroutine(SpinRoutine());
        }
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;
        
        float elapsed = 0f;
        // Base random rotations + a completely random final angle
        float randomFinalAngle = Random.Range(0f, 360f);
        float totalRotation = (360f * 5) + randomFinalAngle; // 5 full loops + extra

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            // Smoothly slow down over time (Easing out)
            float t = elapsed / spinDuration;
            t = t * t * (3f - 2f * t); 

            float currentAngle = Mathf.Lerp(0f, totalRotation, t);
            wheelTransform.localEulerAngles = new Vector3(0, 0, currentAngle);
            
            yield return null;
        }

        isSpinning = false;
        DetermineWinner();
    }

    private void DetermineWinner()
    {
        // Get the final angle normalized between 0 and 360 degrees
        float finalAngle = wheelTransform.localEulerAngles.z % 360f;

        // Unity rotates counter-clockwise. If your Fill Origin is "Top",
        // Option 2 covers the angles moving counter-clockwise from the top.
        float option2AngleSize = option2Chance * 360f;

        if (finalAngle <= option2AngleSize)
        {
            Debug.Log("Option 2 Wins! (10% side)");
        }
        else
        {
            Debug.Log("Option 1 Wins! (90% side)");
        }
    }
}