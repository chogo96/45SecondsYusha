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
            // ī�尡 ��� ���̵��� ù ��° �������� ǥ���մϴ�.
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
            // ī�尡 ��� ���̵��� ù ��° �������� ǥ���մϴ�.
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

        // ī�带 Ŭ����
        ClearCreatedCards();
        
        if (CardsOnThisPage.Count == 0)//����ó��
        {
            return;
        }


        for (int i = 0; i < CardsOnThisPage.Count; i++)
        {
            GameObject newMenuCard;

            if (CardsOnThisPage[i].TypeOfCard == TypesOfCards.Attacks)
            {
                // ����ī��
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

    // �������� ǥ���� ī���� �ڻ� ����� ��ȯ�մϴ�. ���õ� ��� ����(��͵� ��...)�� �´� ī�带 �����մϴ�.
    private List<CardAsset> PageSelection(bool showingCardsPlayerDoesNotOwn = false, int pageIndex = 0, bool includeAllRarities = true, bool includeAllCharacters = true,
        RarityOptions rarity = RarityOptions.Basic, CharacterAsset asset = null, string keyword = "", bool includeTokenCards = false)
    {
        List<CardAsset> returnList = new List<CardAsset>();

        // ���õ� ��� ������ �����ϴ� �÷��ǿ��� ī�带 �����ɴϴ�.
        List<CardAsset> cardsToChooseFrom = CardCollection.instance.GetCards(showingCardsPlayerDoesNotOwn, includeAllRarities, includeAllCharacters, rarity,
            asset, keyword, includeTokenCards);

        // ������ �ε����� ī�带 ǥ���� �� ���� ��ŭ ����� ī�尡 �ִ� ���
        // �׷��� ������ �� ����Ʈ�� ��ȯ�˴ϴ�.
        if (cardsToChooseFrom.Count > pageIndex * Slots.Length)
        {
            // 1) i < cardsToChooseFrom.Count - pageIndex * Slots.Length�� ������ ���������� ī�带 �� ������� �ʾҴ��� Ȯ���մϴ�
            // (���� ���, �������� 10���� ������ ������ 5���� ī�常 ǥ���ؾ� �ϴ� ���)
            // 2) i < Slots.Length�� �� �������� ǥ���� ī���� �ѵ��� ä������ Ȯ���մϴ� (�������� �� ä������ Ȯ��)
            for (int i = 0; (i < cardsToChooseFrom.Count - pageIndex * Slots.Length && i < Slots.Length); i++)
            {
                returnList.Add(cardsToChooseFrom[pageIndex * Slots.Length + i]);
            }
        }

        return returnList;
    }

}

