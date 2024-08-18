using UnityEngine;
using System.Collections.Generic;

public class Hand : MonoBehaviour
{
    public List<CardLogic> CardsInHand = new List<CardLogic>(); // 손패에 있는 카드 목록
    public int maxHandSize = 10; // 손패의 최대 크기

    // 손패에 카드를 추가하는 메소드
    public bool AddCard(CardLogic card)
    {
        if (CardsInHand.Count < maxHandSize)
        {
            CardsInHand.Add(card);
            return true;
        }
        else
        {
            Utils.Log("손패가 가득 찼습니다.");
            return false;
        }
    }

    // 손패에서 카드를 제거하는 메소드
    public void RemoveCard(CardLogic card)
    {
        CardsInHand.Remove(card);
    }

    // 손패에 있는 카드 수를 반환하는 메소드
    public int GetCardCount()
    {
        return CardsInHand.Count;
    }
}
