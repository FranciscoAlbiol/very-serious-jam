using UnityEngine;

[CreateAssetMenu(fileName = "bottle", menuName = "Scriptable Objects/bottle")]
public class bottle_SO : ScriptableObject
{
    public enum Buff {extra_money, luck};

    public string name;
    public string description;

    public int price;
    public Buff buff;
    public int buff_mult; //x2 luck, x4 luck, x8 luck...
    public bool is_bought;


}
