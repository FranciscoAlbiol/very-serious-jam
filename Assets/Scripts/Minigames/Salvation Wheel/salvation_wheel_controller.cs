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

    [Header("Audio Clip")]
    public AudioSource wheelAudioSource;
    public AudioClip win_soundClip;
    public AudioClip roll_soundClip;
    public AudioClip lose_soundClip;

    private bool isSpinning = false;

    public static SpinningWheel Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (wheelAudioSource == null) {
            wheelAudioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void UpdateWheelVisuals()
    {
        if (option2Image != null)
        {
            option2Image.fillAmount = option2Chance;
        }
    }

    public void StartSpin()
    {
        UpdateWheelVisuals();

        if (!isSpinning)
        {
            wheelAudioSource.clip = roll_soundClip;
            wheelAudioSource.Play();
            StartCoroutine(SpinRoutine());
        }
    }

    private IEnumerator SpinRoutine()
    {
        
        isSpinning = true;
        
        float elapsed = 0f;
        float randomFinalAngle = Random.Range(0f, 360f);
        float totalRotation = (360f * 5) + randomFinalAngle; // 5 full loops + extra

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spinDuration;
            t = t * t * (3f - 2f * t); 

            float currentAngle = Mathf.Lerp(0f, totalRotation, t);
            wheelTransform.localEulerAngles = new Vector3(0, 0, currentAngle);
            
            yield return null;
        }

        isSpinning = false;
        wheelAudioSource.Stop();
        DetermineWinner();
    }

    private void DetermineWinner()
{
    float rawAngle = wheelTransform.localEulerAngles.z % 360f;
    if (rawAngle < 0) rawAngle += 360f;
    
    Debug.Log("Final calculated angle: " + rawAngle);

    float option2AngleSize = option2Chance * 360f;
    Debug.Log("Salvation target size: 0 to " + option2AngleSize);

    if (rawAngle <= option2AngleSize)
    {
        Debug.Log("Salvation !!!");
        wheelAudioSource.clip = win_soundClip;
        wheelAudioSource.Play();
    }
    else
    {
        Debug.Log("No salvation :((");
        wheelAudioSource.clip = lose_soundClip;
        wheelAudioSource.Play();
    }
}
}