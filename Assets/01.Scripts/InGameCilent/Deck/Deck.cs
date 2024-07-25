using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    public List<CardAsset> Cards; // 덱에 있는 카드 목록
    public List<CardAsset> DiscardDeck = new List<CardAsset>(); // 사용, 버려진 덱
    public List<CardAsset> VanishDeck = new List<CardAsset>(); // 소멸한 카드 덱
    private BuffManager _buffManager;
    private void Start()
    {
        // 덱을 섞습니다.
        ShuffleDeck();
        _buffManager = GetComponent<BuffManager>();
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

        // 출혈 디버프가 적용되어 있다면 카드를 버리는 로직 실행
        if (_buffManager.BleedDebuff && _buffManager != null && Cards.Count > 0)
        {
            _buffManager.ApplyBleedEffect();
        }
    }

    // 덱에 있는 카드 수를 반환하는 메소드
    public int GetCardCount()
    {
        return Cards.Count;
    }
    // 덱에서 랜덤한 카드를 n장 버리는 메소드
    public void DiscardRandomCards(int n)
    {
        for (int i = 0; i < n; i++)
        {
            if (Cards.Count == 0)
                break;

            int randomIndex = Random.Range(0, Cards.Count);
            CardAsset card = Cards[randomIndex];
            Cards.RemoveAt(randomIndex);
            DiscardDeck.Add(card);
        }
    }
    // discarddeck 중 랜덤한 카드를 n장 덱으로 되돌리는 메소드
    public void ReturnRandomCardsFromDiscard(int n)
    {
        for (int i = 0; i < n; i++)
        {
            if (DiscardDeck.Count == 0)
                break;

            int randomIndex = Random.Range(0, DiscardDeck.Count);
            CardAsset card = DiscardDeck[randomIndex];
            DiscardDeck.RemoveAt(randomIndex);
            Cards.Add(card);
        }
        // 덱을 다시 섞습니다.
        ShuffleDeck();
    }
}
