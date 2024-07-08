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
    public int OverrideLimitOfTHisCardInDeck = 0;// �� ī�尡 ���� ���� ������� �� �� �ֳ�? (��ȭ�� 1��, �� �ܿ� 3�����)

    public TypesOfCards TypeOfCard;

    [Header("Card Info")]
    [Range(0, 3)]
    public int SwordAttack;
    [Range(0, 3)]
    public int MagicAttack;
    [Range(0, 3)]
    public int ShieldAttack;
    public string CardScriptName;
    public TargetingOptions Targets;

    public int CompareTo(CardAsset other)
    {
        throw new NotImplementedException();
    }
}
