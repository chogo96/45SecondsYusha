using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string name;
    public int SwordAttack;
    public int MagicAttack;
    public int ShieldAttack;
    public Sprite sprite;
    public float percent;
}
[CreateAssetMenu(fileName = "ItemSO",menuName = "ScriptableObject/ItemCreate")]
public class ItemSO : ScriptableObject
{
    public Item[] cards;
}
