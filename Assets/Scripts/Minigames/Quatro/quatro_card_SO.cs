using UnityEngine;

[CreateAssetMenu(fileName = "quatro_card", menuName = "Scriptable Objects/quatro_card")]
public class quatro_card_SO : ScriptableObject
{
    public enum Color {red, blue, green, yellow};

    public int number;
    public Color color;
    public Sprite card_sprite;
}
