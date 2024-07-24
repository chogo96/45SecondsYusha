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
    // 이 카드의 대부분의 정보를 담을 것
    [Header("카드 정보")]
    public CharacterAsset CharacterAsset; //만약 이 값이 없으면 중립 카드임 , 직업카드 여부
    [TextArea(2, 3)]
    public string Description;//효과 설명란
    [TextArea(2, 3)]
    public string Tags;//카드의 종류를 정의 해서 쉽게 찾을 수 있게 해놓음 (공격, 기술, 마법)
    public RarityOptions Rarity;
    [PreviewSprite]
    public Sprite CardImage;
    public bool TokenCard = false; //만약 토큰 카드면 콜렉션에서 안보이게 할 것임
    public int LimitOfThisCardInDeck = 0;// 이 카드가 과연 덱에 몇장까지 들어갈 수 있나? (신화급 1장, 그 외엔 3장까지)
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

    // CompareTo 메서드 수정: Rarity를 기준으로 비교
    public int CompareTo(CardAsset other)
    {
        if (other == null)
            return 1;

        int rarityComparison = this.Rarity.CompareTo(other.Rarity);

        if (rarityComparison != 0)
            return rarityComparison;
        else
            // 동일하다면 이름순으로 정렬하자 
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

