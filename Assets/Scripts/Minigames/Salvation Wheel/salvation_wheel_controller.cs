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
    public int fullLoops = 5;

    public int moolah;

    private bool isSpinning = false;

    public static SpinningWheel Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Start(){
        option2Image.fillAmount = option2Chance;
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
        if(GameManager.Instance.current_money <= moolah) return;
        GameManager.Instance.current_money -= moolah;
        if (BuffManager.Instance != null)
            option2Chance = Mathf.Clamp01(option2Chance + BuffManager.Instance.GetWheelLuckBonus());

        UpdateWheelVisuals();

        if (!isSpinning)
            StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        bool playerWins = Random.value < option2Chance;

        float landingAngle;
        if (playerWins)
        {
            float chunkSize = option2Chance * 360f;
            float margin = chunkSize * 0.05f;
            landingAngle = Random.Range(margin, chunkSize - margin);
        }
        else
        {
            float chunkSize = option2Chance * 360f;
            float margin = chunkSize * 0.05f + 2f;
            landingAngle = Random.Range(chunkSize + margin, 360f - margin);
        }

        float totalRotation = fullLoops * 360f + landingAngle;

        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spinDuration;
            t = t * t * (3f - 2f * t);

            float currentAngle = Mathf.Lerp(0f, totalRotation, t);
            wheelTransform.localEulerAngles = new Vector3(0, 0, currentAngle);

            yield return null;
        }

        wheelTransform.localEulerAngles = new Vector3(0, 0, landingAngle);

        isSpinning = false;
        DetermineWinner(playerWins);
    }

    private void DetermineWinner(bool playerWins)
    {
        if (playerWins)
        {
            Debug.Log("Salvation!!!");
        }
        else
        {
            Debug.Log("No salvation :((");
        }
    }
}
