using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public enum TargetingOptions
{
    Nothing,
    Myself,
    Other,
    All,
    Enemy,
}
public enum RarityOptions
{
    Basic, Normal, Rare, Epic, Mythic
}
public enum TypesOfCards
{
    Attacks, Magics, Techniques
}
public class CardAsset : ScriptableObject, IComparable<CardAsset>
{
    // �� ī���� ��κ��� ������ ���� ��
    [Header("ī�� ����")]
    public CharacterAsset CharacterAsset; //���� �� ���� ������ �߸� ī���� , ����ī�� ����
    [TextArea(2, 3)]
    public string Description;//ȿ�� �����
    [TextArea(2, 3)]
    public string Tags;//ī���� ������ ���� �ؼ� ���� ã�� �� �ְ� �س��� (����, ���, ����)
    public RarityOptions Rarity;
    [PreviewSprite]
    public Sprite CardImage;
    public bool TokenCard = false; //���� ��ū ī��� �ݷ��ǿ��� �Ⱥ��̰� �� ����
    public int LimitOfThisCardInDeck = 0;// �� ī�尡 ���� ���� ������� �� �� �ֳ�? (��ȭ�� 1��, �� �ܿ� 3�����)
    public bool IsVanishCard;
    public TypesOfCards TypeOfCard;

    [Header("Card Info")]
    [Range(0, 3)]
    public int SwordAttack;
    [Range(0, 3)]
    public int MagicAttack;
    [Range(0, 3)]
    public int ShieldAttack;
    [Range(0, 4)]
    public int RandomAttack;
    [Range(0, 15)]
    public int RandomRestoreDeck;
    [Range(0,3)]
    public int AdditionalSwordAttack;
    [Range(0,3)]
    public int AdditionalMagicAttack;
    [Range(0,3)]
    public int AdditionalShieldAttack;
    [Range(0, 3)]
    public int AdditionalRandomAttack;

    public string CardScriptName;
    public string RemoveDebuff;
    public TargetingOptions Targets;

    // CompareTo �޼��� ����: Rarity�� �������� ��
    public int CompareTo(CardAsset other)
    {
        if (other == null)
            return 1;

        int rarityComparison = this.Rarity.CompareTo(other.Rarity);

        if (rarityComparison != 0)
            return rarityComparison;
        else
            // �����ϴٸ� �̸������� �������� 
            return name.CompareTo(other.name);
    }

    // Define the is greater than operator.
    public static bool operator >(CardAsset operand1, CardAsset operand2)
    {
        return operand1.CompareTo(operand2) > 0;
    }

    // Define the is less than operator.
    public static bool operator <(CardAsset operand1, CardAsset operand2)
    {
        return operand1.CompareTo(operand2) < 0;
    }

    // Define the is greater than or equal to operator.
    public static bool operator >=(CardAsset operand1, CardAsset operand2)
    {
        return operand1.CompareTo(operand2) >= 0;
    }

    // Define the is less than or equal to operator.
    public static bool operator <=(CardAsset operand1, CardAsset operand2)
    {
        return operand1.CompareTo(operand2) <= 0;
    }
}

