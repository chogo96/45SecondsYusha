using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionBrowser : MonoBehaviour
{
    public Transform Content; // ScrollView의 Content GameObject
    public GameObject CardPrefab; // 카드 Prefab

    public GameObject OneCharacterTabs;
    public GameObject AllCharactersTabs;

    public KeywordInputField KeywordInputFieldScript;
    public CardsThatYouDoNotHaveToggle CardsThatYouDoNotHaveToggleScript;
    public RarityFilter RarityFilter;

    private CharacterAsset _character;
    public List<GameObject> CreatedCards = new List<GameObject>();
    FirebaseCardManager firebaseCardManager;

    private void Awake()
    {
        firebaseCardManager = FindObjectOfType<FirebaseCardManager>();
    }

    #region PROPERTIES
    private bool _showingCardsPlayerDoesNotOwn = false;
    public bool ShowingCardsPlayerDoesNotOwn
    {
        get { return _showingCardsPlayerDoesNotOwn; }

        set
        {
            _showingCardsPlayerDoesNotOwn = value;
            UpdatePage();
        }
    }

    private int _pageIndex = 0;
    public int PageIndex
    {
        get { return _pageIndex; }
        set
        {
            _pageIndex = value;
            UpdatePage();
        }
    }

    private bool _includeAllRarities = true;
    public bool IncludeAllRarities
    {
        get { return _includeAllRarities; }
        set
        {
            _includeAllRarities = value;
            UpdatePage();
        }
    }

    private bool _includeAllCharacters = true;
    public bool IncludeAllCharacters
    {
        get { return _includeAllCharacters; }
        set
        {
            _includeAllCharacters = value;
            // 카드가 즉시 보이도록 첫 번째 페이지를 표시합니다.
            _pageIndex = 0;
            UpdatePage();
        }
    }

    private RarityOptions _rarity = RarityOptions.Basic;
    public RarityOptions Rarity
    {
        get { return _rarity; }
        set
        {
            _rarity = value;
            UpdatePage();
        }
    }

    private CharacterAsset _asset = null;
    public CharacterAsset Asset
    {
        get { return _asset; }
        set
        {
            _asset = value;
            // 카드가 즉시 보이도록 첫 번째 페이지를 표시합니다.
            _pageIndex = 0;
            UpdatePage();
        }
    }

    private string _keyword = "";
    public string Keyword
    {
        get { return _keyword; }
        set
        {
            _keyword = value;
            UpdatePage();
        }
    }

    private bool _includeTokenCards = false;
    public bool IncludeTokenCards
    {
        get { return _includeTokenCards; }
        set
        {
            _includeTokenCards = value;
            UpdatePage();
        }
    }
    #endregion

    public void SetLoadedCards(List<CardAsset> cards, Dictionary<CardAsset, int> cardCounts)
    {
        ClearCreatedCards();
        foreach (CardAsset cardAsset in cards)
        {
            //GameObject newCard = Instantiate(CardPrefab, Content);
            //CreatedCards.Add(newCard);

            //OneCardManager manager = newCard.GetComponent<OneCardManager>();
            //manager.cardAsset = cardAsset;
            //manager.ReadCardFromAsset();

            //AddCardToDeck addCardComponent = newCard.GetComponent<AddCardToDeck>();
            //addCardComponent.SetCardAsset(cardAsset);
            //addCardComponent.UpdateQuantity();
        }
    }

    public void ShowCollectionForBrowsing()
    {
        KeywordInputFieldScript.Clear();
        CardsThatYouDoNotHaveToggleScript.SetValue(false);
        RarityFilter.RemoveAllFilters();

        ShowCards(false, 0, true, false, RarityOptions.Basic, null, "", false);

        DeckBuildingScreen.instance.TabsScript.NeutralTabWhenCollectionBrowsing.Select(instant: true);
        DeckBuildingScreen.instance.TabsScript.SelectTab(DeckBuildingScreen.instance.TabsScript.NeutralTabWhenCollectionBrowsing, instant: true);
    }

    public void ShowCollectionForDeckBuilding(CharacterAsset buildingForThisCharacter)
    {
        KeywordInputFieldScript.Clear();
        CardsThatYouDoNotHaveToggleScript.SetValue(false);
        RarityFilter.RemoveAllFilters();

        _character = buildingForThisCharacter;

        ShowCards(false, 0, true, false, RarityOptions.Basic, _character, "", false);

        DeckBuildingScreen.instance.TabsScript.ClassTab.Select(instant: true);
        DeckBuildingScreen.instance.TabsScript.SelectTab(DeckBuildingScreen.instance.TabsScript.ClassTab, instant: true);
    }

    public void ClearCreatedCards()
    {
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }
        CreatedCards.Clear();
    }

    public void UpdateQuantitiesOnPage()
    {
        for (int i = CreatedCards.Count - 1; i >= 0; i--)
        {
            GameObject card = CreatedCards[i];
            if (card != null)  // 오브젝트가 존재하는지 확인
            {
                AddCardToDeck addCardComponent = card.GetComponent<AddCardToDeck>();
                if (addCardComponent != null)  // 컴포넌트가 존재하는지 확인
                {
                    addCardComponent.UpdateQuantity();
                }
            }
            else
            {
                CreatedCards.RemoveAt(i);  // 리스트에서 삭제
            }
        }
    }

    public void UpdatePage()
    {
        //ShowCards(_showingCardsPlayerDoesNotOwn, _pageIndex, _includeAllRarities, _includeAllCharacters, _rarity, _asset, _keyword, _includeTokenCards);

        firebaseCardManager.LoadCardNames(LoginManager.UserId);
    }

    private void ShowCards(bool showingCardsPlayerDoesNotOwn = false, int pageIndex = 0, bool includeAllRarities = true, bool includeAllCharacters = true,
        RarityOptions rarity = RarityOptions.Basic, CharacterAsset asset = null, string keyword = "", bool includeTokenCards = false)
    {
        _showingCardsPlayerDoesNotOwn = showingCardsPlayerDoesNotOwn;
        _pageIndex = pageIndex;
        _includeAllRarities = includeAllRarities;
        _includeAllCharacters = includeAllCharacters;
        _rarity = rarity;
        _asset = asset;
        _keyword = keyword;
        _includeTokenCards = includeTokenCards;

        List<CardAsset> CardsOnThisPage = PageSelection(showingCardsPlayerDoesNotOwn, pageIndex, includeAllRarities, includeAllCharacters, rarity,
            asset, keyword, includeTokenCards);

        ClearCreatedCards();

        if (CardsOnThisPage.Count == 0)
        {
            return;
        }

        foreach (CardAsset cardAsset in CardsOnThisPage)
        {
            GameObject newCard = Instantiate(CardPrefab, Content);
            CreatedCards.Add(newCard);

            OneCardManager manager = newCard.GetComponent<OneCardManager>();
            manager.cardAsset = cardAsset;
            manager.ReadCardFromAsset();

            AddCardToDeck addCardComponent = newCard.GetComponent<AddCardToDeck>();
            addCardComponent.SetCardAsset(cardAsset);
            addCardComponent.UpdateQuantity();
        }
    }

    public void Next()
    {
        if (PageSelection(_showingCardsPlayerDoesNotOwn, _pageIndex + 1, _includeAllRarities, _includeAllCharacters, _rarity,
            _asset, _keyword, _includeTokenCards).Count == 0)
            return;

        ShowCards(_showingCardsPlayerDoesNotOwn, _pageIndex + 1, _includeAllRarities, _includeAllCharacters, _rarity,
            _asset, _keyword, _includeTokenCards);
    }

    public void Previous()
    {
        if (_pageIndex == 0)
            return;

        ShowCards(_showingCardsPlayerDoesNotOwn, _pageIndex - 1, _includeAllRarities, _includeAllCharacters, _rarity,
            _asset, _keyword, _includeTokenCards);
    }

    private List<CardAsset> PageSelection(bool showingCardsPlayerDoesNotOwn = false, int pageIndex = 0, bool includeAllRarities = true, bool includeAllCharacters = true,
        RarityOptions rarity = RarityOptions.Basic, CharacterAsset asset = null, string keyword = "", bool includeTokenCards = false)
    {
        List<CardAsset> returnList = new List<CardAsset>();

        List<CardAsset> cardsToChooseFrom = CardCollection.instance.GetCards(showingCardsPlayerDoesNotOwn, includeAllRarities, includeAllCharacters, rarity,
            asset, keyword, includeTokenCards);

        if (cardsToChooseFrom.Count > pageIndex * 10)
        {
            for (int i = 0; (i < cardsToChooseFrom.Count - pageIndex * 10 && i < 10); i++)
            {
                returnList.Add(cardsToChooseFrom[pageIndex * 10 + i]);
            }
        }

        return returnList;
    }
}
