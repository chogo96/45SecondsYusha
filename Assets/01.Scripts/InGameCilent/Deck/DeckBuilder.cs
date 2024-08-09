using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Photon.Pun.UtilityScripts.TabViewManager;

public class DeckBuilder : MonoBehaviour
{
    public GameObject CardNamePrefab;
    public Transform Content;
    public TMP_InputField DeckName;

    public int SameCardLimit = 3;
    public int AmountOfCardsInDeck = 30;

    public GameObject DeckCompleteFrame;

    private List<CardAsset> deckList = new List<CardAsset>();
    private Dictionary<CardAsset, CardNameRibbon> ribbons = new Dictionary<CardAsset, CardNameRibbon>();
    public GameObject Tabs;
    public bool InDeckBuildingMode { get; set; }

    private CharacterAsset buildingForCharacter;

    private Button _deckBuilderDoneButton;
    private TMP_Text _deckBuilderDoneButtonText;
    private ListOfDecksInCollection listOfDecksInCollection;

    private FirebaseCardManager firebaseCardManager;

    void Awake()
    {
        DeckCompleteFrame.GetComponent<Image>().raycastTarget = false;
        _deckBuilderDoneButton = transform.Find("DoneButton").GetComponent<Button>();
        _deckBuilderDoneButtonText = transform.Find("DoneButton/Text (TMP)").GetComponent<TMP_Text>();
        listOfDecksInCollection = FindObjectOfType<ListOfDecksInCollection>();
        firebaseCardManager = FindObjectOfType<FirebaseCardManager>();
    }

    private void Start()
    {
        _deckBuilderDoneButton.onClick.AddListener(OnClickDoneButton);
    }
    public void AddCard(CardAsset asset)
    {
        if (!InDeckBuildingMode) return;
        if (deckList.Count == AmountOfCardsInDeck) return;

        int count = NumberOfThisCardInDeck(asset);
        int limitOfThisCardInDeck = asset.LimitOfThisCardInDeck > 0 ? asset.LimitOfThisCardInDeck : SameCardLimit;

        // FirebaseCardManager에서 카드의 수량을 확인
        int availableCount = firebaseCardManager.GetCardCount(LoginManager.UserId, asset);

        if (count < limitOfThisCardInDeck && count < availableCount)
        {
            deckList.Add(asset);
            CheckDeckCompleteFrame();

            count++;

            if (ribbons.ContainsKey(asset))
            {
                ribbons[asset].SetQuantity(count);
            }
            else
            {
                GameObject cardName = Instantiate(CardNamePrefab, Content) as GameObject;
                cardName.transform.SetAsLastSibling();
                cardName.transform.localScale = Vector3.one;
                CardNameRibbon ribbon = cardName.GetComponent<CardNameRibbon>();
                ribbon.ApplyAsset(asset, count);
                ribbons.Add(asset, ribbon);
            }
        }
    }
    void CheckDeckCompleteFrame()
    {
        _deckBuilderDoneButtonText.text = $"{deckList.Count} / {AmountOfCardsInDeck}\nDone";
        _deckBuilderDoneButton.interactable = true;
    }

    public int NumberOfThisCardInDeck(CardAsset asset)
    {
        int count = 0;
        foreach (CardAsset ca in deckList)
        {
            if (ca == asset)
                count++;
        }
        return count;
    }

    public void RemoveCard(CardAsset asset)
    {
        if (ribbons.ContainsKey(asset))
        {
            CardNameRibbon ribbonToRemove = ribbons[asset];
            ribbonToRemove.SetQuantity(ribbonToRemove.Quantity - 1);

            if (NumberOfThisCardInDeck(asset) == 1)
            {
                ribbons.Remove(asset);
                Destroy(ribbonToRemove.gameObject);
            }
        }

        deckList.Remove(asset);
        CheckDeckCompleteFrame();

        firebaseCardManager.LoadCardNames(LoginManager.UserId);  // 카드 수량이 변경된 후 업데이트
    }

    public void BuildADeckFor(CharacterAsset asset)
    {
        InDeckBuildingMode = true;
        buildingForCharacter = asset;

        while (deckList.Count > 0)
        {
            RemoveCard(deckList[0]);
        }

        DeckBuildingScreen.instance.TabsScript.SetClassOnClassTab(asset);
        firebaseCardManager.SetSelectedCharacter(asset);

        CheckDeckCompleteFrame();
        DeckName.text = "";
    }

    public void DoneButtonHandler()
    {
        DeckInfo deckToSave = new DeckInfo(deckList, DeckName.text, buildingForCharacter);
        DecksStorage.instance.AllDecks.Add(deckToSave);
        DecksStorage.instance.SaveDecksIntoPlayerPrefs();
        DeckBuildingScreen.instance.ShowScreenForCollectionBrowsing();
        Tabs.SetActive(true);
    }

    void OnApplicationQuit()
    {
        DoneButtonHandler();
    }

    private void OnClickDoneButton()
    {
        //if (deckList.Count == AmountOfCardsInDeck)
        //{
            DoneButtonHandler();
            gameObject.SetActive(false);
            listOfDecksInCollection.ScrollViewSetActiveTrue();
            listOfDecksInCollection.UpdateList();
        //}
    }
}
