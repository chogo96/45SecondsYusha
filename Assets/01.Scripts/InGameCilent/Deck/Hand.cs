using UnityEngine;
using System.Collections.Generic;

public class Hand : MonoBehaviour
{
    public List<CardLogic> CardsInHand = new List<CardLogic>(); // ���п� �ִ� ī�� ���
    public int maxHandSize = 10; // ������ �ִ� ũ��

    // ���п� ī�带 �߰��ϴ� �޼ҵ�
    public bool AddCard(CardLogic card)
    {
        if (CardsInHand.Count < maxHandSize)
        {
            CardsInHand.Add(card);
            return true;
        }
        else
        {
            Utils.Log("���а� ���� á���ϴ�.");
            return false;
        }
    }

    // ���п��� ī�带 �����ϴ� �޼ҵ�
    public void RemoveCard(CardLogic card)
    {
        CardsInHand.Remove(card);
    }

    // ���п� �ִ� ī�� ���� ��ȯ�ϴ� �޼ҵ�
    public int GetCardCount()
    {
        return CardsInHand.Count;
    }
}
