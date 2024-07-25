using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    public List<CardAsset> Cards; // ���� �ִ� ī�� ���
    public List<CardAsset> DiscardDeck = new List<CardAsset>(); // ���, ������ ��
    public List<CardAsset> VanishDeck = new List<CardAsset>(); // �Ҹ��� ī�� ��
    private BuffManager _buffManager;
    private void Start()
    {
        // ���� �����ϴ�.
        ShuffleDeck();
        _buffManager = GetComponent<BuffManager>();
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

        // ���� ������� ����Ǿ� �ִٸ� ī�带 ������ ���� ����
        if (_buffManager.BleedDebuff && _buffManager != null && Cards.Count > 0)
        {
            _buffManager.ApplyBleedEffect();
        }
    }

    // ���� �ִ� ī�� ���� ��ȯ�ϴ� �޼ҵ�
    public int GetCardCount()
    {
        return Cards.Count;
    }
    // ������ ������ ī�带 n�� ������ �޼ҵ�
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
    // discarddeck �� ������ ī�带 n�� ������ �ǵ����� �޼ҵ�
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
        // ���� �ٽ� �����ϴ�.
        ShuffleDeck();
    }
}
