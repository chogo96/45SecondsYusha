using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionBrowser : MonoBehaviour
{

    public Transform[] Slots;
    public GameObject SpellMenuPrefab;
    public GameObject CreatureMenuPrefab;

    public GameObject OneCharacterTabs;
    public GameObject AllCharactersTabs;

    public KeywordInputField KeywordInputFieldScript;
    public CardsThatYouDoNotHaveToggle CardsThatYouDoNotHaveToggleScript;
    public RarityFilter RarityFilter;

    private CharacterAsset _character;

    private List<GameObject> CreatedCards = new List<GameObject>();

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
        while (CreatedCards.Count > 0)
        {
            GameObject g = CreatedCards[0];
            CreatedCards.RemoveAt(0);
            Destroy(g);
        }
    }

    public void UpdateQuantitiesOnPage()
    {
        foreach (GameObject card in CreatedCards)
        {
            AddCardToDeck addCardComponent = card.GetComponent<AddCardToDeck>();
            addCardComponent.UpdateQuantity();
        }
    }

    public void UpdatePage()
    {
        ShowCards(_showingCardsPlayerDoesNotOwn, _pageIndex, _includeAllRarities, _includeAllCharacters, _rarity, _asset, _keyword, _includeTokenCards);
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

        // 카드를 클리어
        ClearCreatedCards();
        
        if (CardsOnThisPage.Count == 0)//예외처리
        {
            return;
        }


        for (int i = 0; i < CardsOnThisPage.Count; i++)
        {
            GameObject newMenuCard;

            if (CardsOnThisPage[i].TypeOfCard == TypesOfCards.Attacks)
            {
                // 공격카드
                newMenuCard = Instantiate(CreatureMenuPrefab, Slots[i].position, Quaternion.identity) as GameObject;
            }
            else
            {
                newMenuCard = Instantiate(SpellMenuPrefab, Slots[i].position, Quaternion.identity) as GameObject;
            }

            newMenuCard.transform.SetParent(this.transform);

            CreatedCards.Add(newMenuCard);

            OneCardManager manager = newMenuCard.GetComponent<OneCardManager>();
            manager.cardAsset = CardsOnThisPage[i];
            manager.ReadCardFromAsset();

            AddCardToDeck addCardComponent = newMenuCard.GetComponent<AddCardToDeck>();
            addCardComponent.SetCardAsset(CardsOnThisPage[i]);
            addCardComponent.UpdateQuantity();
        }
    }

    public void Next()
    {
        if (PageSelection(_showingCardsPlayerDoesNotOwn, _pageIndex + 1, _includeAllRarities, _includeAllCharacters, _rarity,
            _asset, _keyword,_includeTokenCards).Count == 0)
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

    // 페이지에 표시할 카드의 자산 목록을 반환합니다. 선택된 모든 조건(희귀도 등...)에 맞는 카드를 선택합니다.
    private List<CardAsset> PageSelection(bool showingCardsPlayerDoesNotOwn = false, int pageIndex = 0, bool includeAllRarities = true, bool includeAllCharacters = true,
        RarityOptions rarity = RarityOptions.Basic, CharacterAsset asset = null, string keyword = "", bool includeTokenCards = false)
    {
        List<CardAsset> returnList = new List<CardAsset>();

        // 선택된 모든 조건을 만족하는 컬렉션에서 카드를 가져옵니다.
        List<CardAsset> cardsToChooseFrom = CardCollection.instance.GetCards(showingCardsPlayerDoesNotOwn, includeAllRarities, includeAllCharacters, rarity,
            asset, keyword, includeTokenCards);

        // 페이지 인덱스에 카드를 표시할 수 있을 만큼 충분한 카드가 있는 경우
        // 그렇지 않으면 빈 리스트가 반환됩니다.
        if (cardsToChooseFrom.Count > pageIndex * Slots.Length)
        {
            // 1) i < cardsToChooseFrom.Count - pageIndex * Slots.Length는 마지막 페이지에서 카드를 다 사용하지 않았는지 확인합니다
            // (예를 들어, 페이지에 10개의 슬롯이 있지만 5개의 카드만 표시해야 하는 경우)
            // 2) i < Slots.Length는 한 페이지에 표시할 카드의 한도를 채웠는지 확인합니다 (페이지를 다 채웠는지 확인)
            for (int i = 0; (i < cardsToChooseFrom.Count - pageIndex * Slots.Length && i < Slots.Length); i++)
            {
                returnList.Add(cardsToChooseFrom[pageIndex * Slots.Length + i]);
            }
        }

        return returnList;
    }

}

