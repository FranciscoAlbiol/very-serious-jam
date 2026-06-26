using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DiceGame : MonoBehaviour
{
    public static DiceGame Instance;

    public enum Phase { Idle, Betting, HouseRolling, PlayerRolling, Result }
    public Phase CurrentPhase { get; private set; } = Phase.Idle;

    public DiceFace houseDie1;
    public DiceFace houseDie2;
    public DiceFace playerDie1;
    public DiceFace playerDie2;

    public Transform houseDie1LandPoint;
    public Transform houseDie2LandPoint;
    public Transform playerDie1LandPoint;
    public Transform playerDie2LandPoint;

    public float diceDropStartY  = 5f;
    public float diceDropDuration = 0.5f;
    public float flickerRate     = 0.08f;

    public int minBet  = 10;
    public int betStep = 10;

    public GameObject betScreen;
    public TextMeshProUGUI betAmountText;
    public Button betIncreaseButton;
    public Button lockBetButton;
    public TextMeshProUGUI houseTotalText;
    public TextMeshProUGUI playerTotalText;
    public GameObject throwButton;
    public GameObject leaveButton;

    public UnityEvent<int> onMoneyChanged;

    private int currentBet = 10;
    private int houseTotal;
    private int playerTotal;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        HideAll();
        HideDice();
    }

    void Update()
    {
        if (CurrentPhase == Phase.Idle) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Quit();
    }

    public void StartGame()
    {
        currentBet = Mathf.Clamp(minBet, minBet, Mathf.Max(minBet, GameManager.Instance.current_money));
        EnterBetPhase();
    }

    private void EnterBetPhase()
    {
        CurrentPhase = Phase.Betting;
        HideAll();
        HideDice();
        betScreen.SetActive(true);
        RefreshBetUI();
    }

    public void IncreaseBet()
    {
        if (CurrentPhase != Phase.Betting) return;
        currentBet = Mathf.Clamp(currentBet + betStep, minBet, GameManager.Instance.current_money);
        RefreshBetUI();
    }

    public void DecreaseBet()
    {
        if (CurrentPhase != Phase.Betting) return;
        currentBet = Mathf.Clamp(currentBet - betStep, minBet, GameManager.Instance.current_money);
        RefreshBetUI();
    }

    public void LockBet()
    {
        if (CurrentPhase != Phase.Betting) return;
        if (GameManager.Instance.current_money < currentBet) return;
        StartCoroutine(HouseRollPhase());
    }

    private void RefreshBetUI()
    {
        betAmountText.text             = $"${currentBet}";
        betIncreaseButton.interactable = currentBet < GameManager.Instance.current_money;
    }

    private IEnumerator HouseRollPhase()
    {
        CurrentPhase = Phase.HouseRolling;
        HideAll();
        houseTotalText.text  = "";
        playerTotalText.text = "";
        throwButton.SetActive(false);

        int hDie1 = Random.Range(1, 7);
        int hDie2 = Random.Range(1, 7);
        houseTotal = hDie1 + hDie2;

        yield return StartCoroutine(DropAndRoll(
            houseDie1, houseDie2,
            houseDie1LandPoint.position, houseDie2LandPoint.position,
            hDie1, hDie2));

        
        houseTotalText.gameObject.SetActive(true);
        houseTotalText.text = houseTotal.ToString();;

        CurrentPhase = Phase.PlayerRolling;
        throwButton.SetActive(true);
    }

    public void PlayerThrow()
    {
        if (CurrentPhase != Phase.PlayerRolling) return;
        StartCoroutine(PlayerRollPhase());
    }

    private IEnumerator PlayerRollPhase()
    {
        CurrentPhase = Phase.Result;
        throwButton.SetActive(false);
        playerTotalText.text = "";

        int pDie1 = Random.Range(1, 7);
        int pDie2 = Random.Range(1, 7);
        int luckBonus = BuffManager.Instance != null ? BuffManager.Instance.GetDiceLuckBonus() : 0;
        playerTotal = pDie1 + pDie2 + luckBonus;

        yield return StartCoroutine(DropAndRoll(
            playerDie1, playerDie2,
            playerDie1LandPoint.position, playerDie2LandPoint.position,
            pDie1, pDie2));

        playerTotalText.gameObject.SetActive(true);
        playerTotalText.text = playerTotal.ToString();;

        yield return new WaitForSeconds(0.6f);
        ShowResult();
    }

    private void ShowResult()
    {
        bool playerWins = playerTotal > houseTotal;
        bool tie        = playerTotal == houseTotal;
        int  rawDelta   = playerWins ? currentBet : (tie ? 0 : -currentBet);
        int  delta      = BuffManager.Instance != null ? BuffManager.Instance.ApplyCashout(rawDelta) : rawDelta;

        GameManager.Instance.AddMoney(delta);
        onMoneyChanged?.Invoke(GameManager.Instance.current_money);
        leaveButton.SetActive(true);

        if (playerWins) Debug.Log($"You won! +${currentBet}");
        else if (tie)   Debug.Log("Tie!");
        else            Debug.Log($"You lost! -${currentBet}");
    }

    public void PlayAgain() => EnterBetPhase();

    public void Quit()
    {
        StopAllCoroutines();
        CurrentPhase = Phase.Idle;
        if (houseTotalText  != null) houseTotalText .gameObject.SetActive(false);
        if (playerTotalText != null) playerTotalText.gameObject.SetActive(false);
        HideAll();
        HideDice();
    }

    private IEnumerator DropAndRoll(
        DiceFace die1, DiceFace die2,
        Vector3 landPos1, Vector3 landPos2,
        int result1, int result2)
    {
        Vector3 startPos1 = landPos1 + Vector3.up * diceDropStartY;
        Vector3 startPos2 = landPos2 + Vector3.up * diceDropStartY;

        die1.transform.position = startPos1;
        die2.transform.position = startPos2;

        Vector3 spinAxis1 = Random.onUnitSphere;
        Vector3 spinAxis2 = Random.onUnitSphere;
        float spinSpeed1  = Random.Range(400f, 720f);
        float spinSpeed2  = Random.Range(400f, 720f);

        Quaternion spinRot1 = Random.rotation;
        Quaternion spinRot2 = Random.rotation;

        die1.transform.rotation = spinRot1;
        die2.transform.rotation = spinRot2;

        die1.gameObject.SetActive(true);
        die2.gameObject.SetActive(true);

        Quaternion landRot1 = die1.GetFaceRotation(result1);
        Quaternion landRot2 = die2.GetFaceRotation(result2);

        float elapsed = 0f;

        while (elapsed < diceDropDuration)
        {
            float t      = elapsed / diceDropDuration;
            float easedT = t * t;

            die1.transform.position = Vector3.Lerp(startPos1, landPos1, easedT);
            die2.transform.position = Vector3.Lerp(startPos2, landPos2, easedT);
            if (t < 0.8f)
            {
                spinRot1 = Quaternion.AngleAxis(spinSpeed1 * Time.deltaTime, spinAxis1) * spinRot1;
                spinRot2 = Quaternion.AngleAxis(spinSpeed2 * Time.deltaTime, spinAxis2) * spinRot2;
                die1.transform.rotation = spinRot1;
                die2.transform.rotation = spinRot2;
            }
            else
            {
                float snapT = (t - 0.8f) / 0.2f;
                float smoothT = snapT * snapT * (3f - 2f * snapT);
                die1.transform.rotation = Quaternion.Slerp(spinRot1, landRot1, smoothT);
                die2.transform.rotation = Quaternion.Slerp(spinRot2, landRot2, smoothT);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        die1.transform.position = landPos1;
        die2.transform.position = landPos2;
        die1.ShowFace(result1);
        die2.ShowFace(result2);
    }

    private void HideAll()
    {
        if (betScreen    != null) betScreen.SetActive(false);
        if (throwButton  != null) throwButton.SetActive(false);
        if (leaveButton != null) leaveButton.SetActive(false);
    }

    private void HideDice()
    {
        if (houseDie1  != null) houseDie1.gameObject.SetActive(false);
        if (houseDie2  != null) houseDie2.gameObject.SetActive(false);
        if (playerDie1 != null) playerDie1.gameObject.SetActive(false);
        if (playerDie2 != null) playerDie2.gameObject.SetActive(false);
    }
}