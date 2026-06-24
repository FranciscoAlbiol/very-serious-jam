using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int current_money = 100;
    public TextMeshProUGUI moneytext;

    public quatro_manager quatro;
    public poker_manager poker;
    public SlotMachine slotMachine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        moneytext.text = current_money.ToString();
    }

    public void start_minigame(string minigame) {
        switch(minigame) 
        {
        case "poker":
            poker_manager.Instance.start_poker();
            break;
        case "quatro":
            quatro_manager.Instance.start_quatro();
            break;
        case "slots":
            break;
        default:
            Debug.Log("No minigame found.");
            break;
        }
    }
}