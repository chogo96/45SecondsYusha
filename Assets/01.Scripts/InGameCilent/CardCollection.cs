using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class CardCollection : SingletonMonoBase<CardCollection>
{
    public int DefaultNumberOfBasicCards = 0; //�⺻������ ����־���ϴ� ����ī���� ����

    private Dictionary<string, CardAsset> AllCardsDictionary = new Dictionary<string, CardAsset>();

    public Dictionary<CardAsset, int> QuantityOfEachCard = new Dictionary<CardAsset, int>();

    private CardAsset[] _allCardArray;

    private void Awake()
    {
        _allCardArray = Resources.LoadAll<CardAsset>("");
        foreach (CardAsset cardAsset in _allCardArray)
        {
            if (!AllCardsDictionary.ContainsKey(cardAsset.name))
            {
                AllCardsDictionary.Add(cardAsset.name, cardAsset);
            }
        }
        LoadQuantityOfCardsFromPlayerPrefs();
    }
    /// <summary>
    /// ������ ī����� �ε��� basicī����� �̹� ������ ������ �н�
    /// </summary>
    private void LoadQuantityOfCardsFromPlayerPrefs()
    {
        //���� ī�尡 �ƴ� ī�常 �ε带 �ؾ���
        foreach (CardAsset cardAsset in _allCardArray)
        {
            //����ī�� ���� ó��
            if (cardAsset.Rarity == RarityOptions.Basic)
            {
                QuantityOfEachCard.Add(cardAsset, DefaultNumberOfBasicCards);
            }
            else if (PlayerPrefs.HasKey("NumberOf" + cardAsset.name))
            {
                QuantityOfEachCard.Add(cardAsset, PlayerPrefs.GetInt("NumberOf" + cardAsset.name));
            }
            else
            {
                QuantityOfEachCard.Add(cardAsset, 0);
            }
        }
    }
    /// <summary>
    /// �÷��̾ ���� ī����� ������
    /// </summary>
    private void SaveQuantityOfCardsIntoPlayerPrefs()
    {
        foreach (CardAsset cardAsset in _allCardArray)
        {
            if (cardAsset.Rarity == RarityOptions.Basic)
            {
                PlayerPrefs.SetInt("NumberOf" + cardAsset.name, DefaultNumberOfBasicCards);
            }
            else
            {
                PlayerPrefs.SetInt("NumberOf" + cardAsset.name, QuantityOfEachCard[cardAsset]);
            }
        }
    }
    /// <summary>
    /// ���ø����̼� �� �� ī�带 ������
    /// </summary>
    void OnApplicationQuit()
    {
        SaveQuantityOfCardsIntoPlayerPrefs();
    }

    public CardAsset GetCardAssetByName(string name)
    {
        if (AllCardsDictionary.ContainsKey(name))//�� �̸��� ���� ī�尡 ������ �� ī�� ������ ��ȯ��
        {
            return AllCardsDictionary[name];
        }
        else//������ null ���� ��ȯ��
        {
            return null;
        }
    }

    public List<CardAsset> GetCardsOfCharacter(CharacterAsset asset)
    {
        return GetCards(true, true, false, RarityOptions.Basic, asset);
    }
    public List<CardAsset> GetCardsWithRarity(RarityOptions rarity)
    {
        return GetCards(true, false, true, rarity);
    }
    /// <summary>
    /// ���� ���͸� ���� �� �Լ��� ī�带 ������ ����
    /// </summary>
    /// <param name="showingCardsPlayerDoesNotOwn">�÷��̾� ���� ī�带 �����ٱ�?</param>
    /// <param name="includeAllRarity">��� ��͵��� ī�带 �����ұ�?</param>
    /// <param name="includeAllCharacter">��� ������ �����ұ�?</param>
    /// <param name="rarity">��͵�</param>
    /// <param name="asset">����</param>
    /// <param name="keyword">Ű����</param>
    /// <param name="includeTokenCards">��ūī�� ����</param>
    /// <returns></returns>
    public List<CardAsset> GetCards(bool showingCardsPlayerDoesNotOwn = false, bool includeAllRarity = true,bool includeAllCharacter = true, 
        RarityOptions rarity = RarityOptions.Basic, CharacterAsset asset = null, string keyword = "", bool includeTokenCards = false)
    {
        //��� ī�带 ������
        var cards = from card in _allCardArray select card;


        if (!showingCardsPlayerDoesNotOwn)//ȹ�� ī�带 ������ �ʴ´ٸ�
        {
            cards = cards.Where(card => QuantityOfEachCard[card] > 0);
        }
        if (!includeTokenCards)
        {
            cards = cards.Where(card => card.TokenCard == false);
        }
        if (!includeAllRarity)
        {
            cards = cards.Where(card => card.Rarity == rarity);
        }
        if (!includeAllCharacter)
        {
            cards = cards.Where(card => card.CharacterAsset == asset);
        }
        if(keyword != null && keyword != "")
        {
            cards = cards.Where(card => (card.name.ToLower().Contains(keyword.ToLower())) || 
            (card.Tags.ToLower().Contains(keyword.ToLower()) && !keyword.ToLower().Contains("")));
        }

        var returnList = cards.ToList<CardAsset>();
        returnList.Sort();

        return returnList;
    }
}
