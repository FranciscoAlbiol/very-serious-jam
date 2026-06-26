using UnityEngine;

public enum Buff {extra_money, luck};

[CreateAssetMenu(fileName = "bottle", menuName = "Scriptable Objects/bottle")]
public class bottle_SO : ScriptableObject
{
    

    public string name;
    public string description;

    public int price;
    public Buff buff;
    public BuffTier buff_tier;
    public bool is_bought;


}
