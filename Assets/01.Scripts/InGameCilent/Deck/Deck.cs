using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    public List<CardAsset> Cards; // ���� �ִ� ī�� ���
    public List<CardAsset> DiscardDeck = new List<CardAsset>(); // ��� ��
    public List<CardAsset> VanishDeck = new List<CardAsset>(); // ����� ��

    private void Start()
    {
        // ���� �����ϴ�.
        ShuffleDeck();
    }

    // ���� ���� �޼ҵ�
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

    // ������ ī�带 �� �� �̴� �޼ҵ�
    public CardAsset DrawCard()
    {
        if (Cards.Count == 0)
            return null;

        CardAsset card = Cards[0];
        Cards.RemoveAt(0);
        return card;
    }

    // ���� ī�带 �߰��ϴ� �޼ҵ�
    public void AddCard(CardAsset card)
    {
        Cards.Add(card);
    }

    // ���� �ִ� ī�� ���� ��ȯ�ϴ� �޼ҵ�
    public int GetCardCount()
    {
        return Cards.Count;
    }
}
