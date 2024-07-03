using UnityEngine;

public enum CardType
{
    None = 0,
    Attack = 1,
    Spell = 2,
    Tech = 3,
}
[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{

}

