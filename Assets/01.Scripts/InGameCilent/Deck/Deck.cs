using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    public List<CardAsset> Cards; // 덱에 있는 카드 목록
    public List<CardAsset> DiscardDeck = new List<CardAsset>(); // 폐기 덱
    public List<CardAsset> VanishDeck = new List<CardAsset>(); // 사라진 덱

    private void Start()
    {
        // 덱을 섞습니다.
        ShuffleDeck();
    }

    // 덱을 섞는 메소드
    public void ShuffleDeck()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            CardAsset temp = Cards[i];
            int randomIndex = Random.Range(i, Cards.Count);
            Cards[i] = Cards[randomIndex];
            Cards[randomIndex] = temp;
        }
    }

    // 덱에서 카드를 한 장 뽑는 메소드
    public CardAsset DrawCard()
    {
        if (Cards.Count == 0)
            return null;

        CardAsset card = Cards[0];
        Cards.RemoveAt(0);
        return card;
    }

    // 덱에 카드를 추가하는 메소드
    public void AddCard(CardAsset card)
    {
        Cards.Add(card);
    }

    // 덱에 있는 카드 수를 반환하는 메소드
    public int GetCardCount()
    {
        return Cards.Count;
    }
}
