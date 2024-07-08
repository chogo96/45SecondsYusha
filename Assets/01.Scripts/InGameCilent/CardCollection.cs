using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class CardCollection : SingletonMonoBase<CardCollection>
{
    public int DefaultNumberOfBasicCards = 0; //기본적으로 들고있어야하는 기초카드의 갯수

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
    /// 저장한 카드들을 로드함 basic카드들은 이미 가지고 있으니 패스
    /// </summary>
    private void LoadQuantityOfCardsFromPlayerPrefs()
    {
        //기초 카드가 아닌 카드만 로드를 해야함
        foreach (CardAsset cardAsset in _allCardArray)
        {
            //기초카드 예외 처리
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
    /// 플레이어가 가진 카드들을 저장함
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
    /// 어플리케이션 끌 때 카드를 저장함
    /// </summary>
    void OnApplicationQuit()
    {
        SaveQuantityOfCardsIntoPlayerPrefs();
    }

    public CardAsset GetCardAssetByName(string name)
    {
        if (AllCardsDictionary.ContainsKey(name))//이 이름을 가진 카드가 있으면 그 카드 에셋을 반환함
        {
            return AllCardsDictionary[name];
        }
        else//없으면 null 값을 반환함
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
    /// 여러 필터를 통해 이 함수로 카드를 가져올 것임
    /// </summary>
    /// <param name="showingCardsPlayerDoesNotOwn">플레이어 없는 카드를 보여줄까?</param>
    /// <param name="includeAllRarity">모든 희귀도의 카드를 포함할까?</param>
    /// <param name="includeAllCharacter">모든 직업을 포함할까?</param>
    /// <param name="rarity">희귀도</param>
    /// <param name="asset">에셋</param>
    /// <param name="keyword">키워드</param>
    /// <param name="includeTokenCards">토큰카드 여부</param>
    /// <returns></returns>
    public List<CardAsset> GetCards(bool showingCardsPlayerDoesNotOwn = false, bool includeAllRarity = true,bool includeAllCharacter = true, 
        RarityOptions rarity = RarityOptions.Basic, CharacterAsset asset = null, string keyword = "", bool includeTokenCards = false)
    {
        //모든 카드를 선택함
        var cards = from card in _allCardArray select card;


        if (!showingCardsPlayerDoesNotOwn)//획득 카드를 보이지 않는다면
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
