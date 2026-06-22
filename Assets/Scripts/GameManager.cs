using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static int current_money;

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
}
