using UnityEngine;

[CreateAssetMenu(fileName = "card_SO", menuName = "Scriptable Objects/card_SO")]
public class card_SO : ScriptableObject
{
    public enum Suit {club, diamond, spade, heart};

    public int number;
    public Suit suit;
    public Sprite card_sprite;
}
