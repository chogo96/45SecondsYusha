//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;
//using System;
//public class CardCollection : MonoBehaviour
//{
//    public int DefaultNumberOfBasicCards = 0; //기본적으로 들고있어야하는 기초카드의 갯수

//    private Dictionary<string, CardAsset> AllCardsDictionary = new Dictionary<string, CardAsset>();

//    public Dictionary<CardAsset, int> QuantityOfEachCard = new Dictionary<CardAsset, int>();
//    //private FirebaseCardManager firebaseCardManager = FindObjectOfType<FirebaseCardManager>();


//    private CardAsset[] _allCardArray;
//    public static CardCollection instance;
//    private void Awake()
//    {
//        instance = this;
//        _allCardArray = Resources.LoadAll<CardAsset>("");
//        foreach (CardAsset cardAsset in _allCardArray)
//        {
//            if (!AllCardsDictionary.ContainsKey(cardAsset.name))
//            {
//                AllCardsDictionary.Add(cardAsset.name, cardAsset);
//            }
//        }
//        LoadQuantityOfCardsFromPlayerPrefs();
//    }
//    /// <summary>
//    /// 저장한 카드들을 로드함 basic카드들은 이미 가지고 있으니 패스
//    /// </summary>
//    private void LoadQuantityOfCardsFromPlayerPrefs()
//    {
//        //기초 카드가 아닌 카드만 로드를 해야함
//        foreach (CardAsset cardAsset in _allCardArray)
//        {
//            //기초카드 예외 처리
//            if (cardAsset.Rarity == RarityOptions.Basic)
//            {
//                QuantityOfEachCard.Add(cardAsset, DefaultNumberOfBasicCards);
//            }
//            else if (PlayerPrefs.HasKey("NumberOf" + cardAsset.name))
//            {
//                QuantityOfEachCard.Add(cardAsset, PlayerPrefs.GetInt("NumberOf" + cardAsset.name));
//            }
//            else
//            {
//                QuantityOfEachCard.Add(cardAsset, 0);
//            }
//        }
//    }
//    /// <summary>
//    /// 플레이어가 가진 카드들을 저장함
//    /// </summary>
//    private void SaveQuantityOfCardsIntoPlayerPrefs()
//    {
//        foreach (CardAsset cardAsset in _allCardArray)
//        {
//            if (cardAsset.Rarity == RarityOptions.Basic)
//            {
//                PlayerPrefs.SetInt("NumberOf" + cardAsset.name, DefaultNumberOfBasicCards);
//            }
//            else
//            {
//                PlayerPrefs.SetInt("NumberOf" + cardAsset.name, QuantityOfEachCard[cardAsset]);
//            }
//        }
//    }
//    /// <summary>
//    /// 어플리케이션 끌 때 카드를 저장함
//    /// </summary>
//    void OnApplicationQuit()
//    {
//        //SaveQuantityOfCardsIntoPlayerPrefs();
//    }

//    public CardAsset GetCardAssetByName(string name)
//    {
//        if (AllCardsDictionary.ContainsKey(name))//이 이름을 가진 카드가 있으면 그 카드 에셋을 반환함
//        {
//            return AllCardsDictionary[name];
//        }
//        else//없으면 null 값을 반환함
//        {
//            return null;
//        }
//    }

//    public List<CardAsset> GetCardsOfCharacter(CharacterAsset asset)
//    {
//        return GetCards(true, true, false, RarityOptions.Basic, asset);
//    }
//    public List<CardAsset> GetCardsWithRarity(RarityOptions rarity)
//    {
//        return GetCards(true, false, true, rarity);
//    }
//    /// <summary>
//    /// 여러 필터를 통해 이 함수로 카드를 가져올 것임
//    /// </summary>
//    /// <param name="showingCardsPlayerDoesNotOwn">플레이어 없는 카드를 보여줄까?</param>
//    /// <param name="includeAllRarity">모든 희귀도의 카드를 포함할까?</param>
//    /// <param name="includeAllCharacter">모든 직업을 포함할까?</param>
//    /// <param name="rarity">희귀도</param>
//    /// <param name="asset">에셋</param>
//    /// <param name="keyword">키워드</param>
//    /// <param name="includeTokenCards">토큰카드 여부</param>
//    /// <returns></returns>
//    public List<CardAsset> GetCards(bool showingCardsPlayerDoesNotOwn = false, bool includeAllRarity = true,bool includeAllCharacter = true, 
//        RarityOptions rarity = RarityOptions.Basic, CharacterAsset asset = null, string keyword = "", bool includeTokenCards = false)
//    {
//        //모든 카드를 선택함
//        var cards = from card in _allCardArray select card;


//        if (!showingCardsPlayerDoesNotOwn)//획득 카드를 보이지 않는다면
//        {
//            cards = cards.Where(card => QuantityOfEachCard[card] > 0);
//        }
//        if (!includeTokenCards)
//        {
//            cards = cards.Where(card => card.TokenCard == false);
//        }
//        if (!includeAllRarity)
//        {
//            cards = cards.Where(card => card.Rarity == rarity);
//        }
//        if (!includeAllCharacter)
//        {
//            cards = cards.Where(card => card.CharacterAsset == asset);
//        }
//        if(keyword != null && keyword != "")
//        {
//            cards = cards.Where(card => (card.name.ToLower().Contains(keyword.ToLower())) || 
//            (card.Tags.ToLower().Contains(keyword.ToLower()) && !keyword.ToLower().Contains("")));
//        }

//        var returnList = cards.ToList<CardAsset>();
//        returnList.Sort();

//        return returnList;
//    }
//}
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardCollection : MonoBehaviour
{
    public int DefaultNumberOfBasicCards = 0; // 기본적으로 들고있어야하는 기초카드의 갯수

    private Dictionary<string, CardAsset> AllCardsDictionary = new Dictionary<string, CardAsset>();
    public Dictionary<CardAsset, int> QuantityOfEachCard = new Dictionary<CardAsset, int>();

    private CardAsset[] _allCardArray;
    public static CardCollection instance; // Singleton 인스턴스
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 게임 객체가 다른 씬으로 전환되어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
        // 기초 카드가 아닌 카드만 로드를 해야함
        foreach (CardAsset cardAsset in _allCardArray)
        {
            // 기초카드 예외 처리
            if (cardAsset.Rarity == RarityOptions.Basic)
            {
                QuantityOfEachCard[cardAsset] = DefaultNumberOfBasicCards;
            }
            else if (PlayerPrefs.HasKey("NumberOf" + cardAsset.name))
            {
                QuantityOfEachCard[cardAsset] = PlayerPrefs.GetInt("NumberOf" + cardAsset.name);
            }
            else
            {
                QuantityOfEachCard[cardAsset] = 0;
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
        SaveQuantityOfCardsIntoPlayerPrefs(); // 저장 기능 활성화
    }

    public CardAsset GetCardAssetByName(string name)
    {
        if (AllCardsDictionary.TryGetValue(name, out var cardAsset))
        {
            return cardAsset;
        }
        return null;
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
    public List<CardAsset> GetCards(
        bool showingCardsPlayerDoesNotOwn = false,
        bool includeAllRarity = true,
        bool includeAllCharacter = true,
        RarityOptions rarity = RarityOptions.Basic,
        CharacterAsset asset = null,
        string keyword = "",
        bool includeTokenCards = false)
    {
        // 모든 카드를 선택함
        var cards = _allCardArray.AsEnumerable();

        if (!showingCardsPlayerDoesNotOwn) // 획득 카드를 보이지 않는다면
        {
            cards = cards.Where(card => QuantityOfEachCard[card] > 0);
        }
        if (!includeTokenCards)
        {
            cards = cards.Where(card => !card.TokenCard);
        }
        if (!includeAllRarity)
        {
            cards = cards.Where(card => card.Rarity == rarity);
        }
        if (!includeAllCharacter)
        {
            cards = cards.Where(card => card.CharacterAsset == asset);
        }
        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = keyword.ToLower();
            cards = cards.Where(card => card.name.ToLower().Contains(keyword) ||
                                        card.Tags.ToLower().Contains(keyword));
        }

        var returnList = cards.ToList();
        returnList.Sort(); // 카드 정렬

        return returnList;
    }

    /// <summary>
    /// 카드 컬렉션을 다시 로드하여 UI를 업데이트하는 메서드
    /// </summary>
    public void RefreshCardUI()
    {
        // 여기에 UI 갱신 로직을 추가하세요.
        // Firebase에서 카드 데이터를 다시 가져와 업데이트하거나, CardManager와 상호작용하여 새로운 데이터를 로드합니다.
    }
}
