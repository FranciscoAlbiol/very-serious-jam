using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class SlotMachine : MonoBehaviour
{
    public static SlotMachine Instance;

    public enum Symbol
    {
        Skull,       // −100% of bet
        BrokenCoin,  // −50%  of bet
        Crack,       // −25%  of bet
        Clover,      // +25%  of bet
        Star,        // +50%  of bet
        Diamond,     // +100% of bet
        Seven        // +200% of bet
    }

    [System.Serializable]
    public struct SpinResult
    {
        public Symbol left;
        public Symbol middle;
        public Symbol right;
        public int moneyDelta;
        public string resultMessage;
    }

    public SlotReel leftReel;
    public SlotReel middleReel;
    public SlotReel rightReel;

    public float leftStopDelay   = 1.0f;
    public float middleStopDelay = 1.8f;
    public float rightStopDelay  = 2.6f;

    public int[] symbolWeights = { 20, 18, 16, 16, 14, 12, 4 };

    public int minBet = 10;

    [Header("UI")]
    public TextMeshProUGUI betText;

    public UnityEvent<SpinResult> onSpinComplete;
    public UnityEvent<string>     onError;
    public UnityEvent<int>        onBetChanged;
    public UnityEvent<int>        onMoneyChanged;

    public bool IsSpinning  { get; private set; }
    public int  CurrentBet  { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        CurrentBet = minBet;
        UpdateBetText();
    }

    public void ChangeBet(int amount)
    {
        int max = Mathf.Max(minBet, GameManager.Instance.current_money);
        CurrentBet = Mathf.Clamp(CurrentBet + amount, minBet, max);
        UpdateBetText();
        onBetChanged?.Invoke(CurrentBet);
    }

    public void Spin()
    {
        if (IsSpinning) return;

        int max = Mathf.Max(minBet, GameManager.Instance.current_money);
        CurrentBet = Mathf.Clamp(CurrentBet, minBet, max);

        if (GameManager.Instance.current_money <= 0) return;

        StartCoroutine(SpinRoutine());
    }

    private void UpdateBetText()
    {
        if (betText != null)
            betText.text = $"Bet: ${CurrentBet}";
    }

    private IEnumerator SpinRoutine()
    {
        IsSpinning = true;

        Symbol middle = PickSymbol();
        Symbol left   = Random.value < 0.75f ? middle : PickSymbol();
        Symbol right  = Random.value < 0.75f ? middle : PickSymbol();

        leftReel  .StartSpin(left,   leftStopDelay);
        middleReel.StartSpin(middle, middleStopDelay);
        rightReel .StartSpin(right,  rightStopDelay);

        yield return new WaitUntil(() =>
            !leftReel.IsSpinning && !middleReel.IsSpinning && !rightReel.IsSpinning);

        left   = leftReel  .LandedSymbol;
        middle = middleReel.LandedSymbol;
        right  = rightReel .LandedSymbol;

        Symbol outcome = ResolveSymbols(left, middle, right);
        int delta      = CalculateDelta(outcome, CurrentBet);

        GameManager.Instance.current_money += delta;
        onMoneyChanged?.Invoke(GameManager.Instance.current_money);

        SpinResult result = new SpinResult
        {
            left          = left,
            middle        = middle,
            right         = right,
            moneyDelta    = delta,
            resultMessage = BuildMessage(outcome, delta, CurrentBet)
        };

        onSpinComplete?.Invoke(result);
        Debug.Log(BuildMessage(outcome, delta, CurrentBet));
        IsSpinning = false;
    }

    private Symbol ResolveSymbols(Symbol l, Symbol m, Symbol r)
    {
        if (l == m && m == r) return l;
        if (l == m)           return l;
        if (m == r)           return m;
        if (l == r)           return l;
        return r;
    }

    private int CalculateDelta(Symbol s, int bet)
    {
        return s switch
        {
            Symbol.Skull      => -bet,
            Symbol.BrokenCoin => -Mathf.RoundToInt(bet * 0.50f),
            Symbol.Crack      => -Mathf.RoundToInt(bet * 0.25f),
            Symbol.Clover     =>  Mathf.RoundToInt(bet * 0.25f),
            Symbol.Star       =>  Mathf.RoundToInt(bet * 0.50f),
            Symbol.Diamond    =>  bet,
            Symbol.Seven      =>  bet * 2,
            _                 =>  0
        };
    }

    private string BuildMessage(Symbol s, int delta, int bet)
    {
        string sign = delta >= 0 ? "+" : "";
        return s switch
        {
            Symbol.Skull      => $"Lost all {bet} Dollars. ({sign}{delta})",
            Symbol.BrokenCoin => $"Lost 50% of {bet}. ({sign}{delta})",
            Symbol.Crack      => $"Lost 25% of {bet}. ({sign}{delta})",
            Symbol.Clover     => $"Won 25% of {bet}. ({sign}{delta})",
            Symbol.Star       => $"Won 50% of {bet}. ({sign}{delta})",
            Symbol.Diamond    => $"Won 100% of {bet}. ({sign}{delta})",
            Symbol.Seven      => $"Won 200% of {bet}! ({sign}{delta})",
            _                 => "idfk"
        };
    }

    private Symbol PickSymbol()
    {
        int total = 0;
        foreach (int w in symbolWeights) total += w;

        int roll = Random.Range(0, total);
        int cumulative = 0;

        for (int i = 0; i < symbolWeights.Length; i++)
        {
            cumulative += symbolWeights[i];
            if (roll < cumulative)
                return (Symbol)i;
        }

        return Symbol.Skull;
    }
}
