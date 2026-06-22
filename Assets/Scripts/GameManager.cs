using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static int current_money;

    public quatro_manager quatro;
    public poker_manager poker;

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
        current_money = 0;
    }

    void Update()
    {
        
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
        default:
            Debug.Log("No minigame found.");
            break;
        }
    }
}
