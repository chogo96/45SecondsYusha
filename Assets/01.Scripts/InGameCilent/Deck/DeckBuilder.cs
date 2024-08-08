using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Serialization;
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

    public bool InDeckBuildingMode { get; set; }

    private CharacterAsset buildingForCharacter;

    /// <summary>
    /// ��ư �ؽ�Ʈ�� ī�� ������ ������ �ϸ鼭 ����͵�
    /// </summary>
    private Button _deckBuilderDoneButton;
    private TMP_Text _deckBuilderDoneButtonText;
    private ListOfDecksInCollection listOfDecksInCollection;

    void Awake()
    {
        DeckCompleteFrame.GetComponent<Image>().raycastTarget = false;
        _deckBuilderDoneButton = transform.Find("DoneButton").GetComponent<Button>();
        _deckBuilderDoneButtonText = transform.Find("DoneButton/Text (TMP)").GetComponent<TMP_Text>();
        listOfDecksInCollection = FindObjectOfType<ListOfDecksInCollection>();
    }

    private void Start()
    {
        _deckBuilderDoneButton.onClick.AddListener(OnClickDoneButton);
    }

    public void AddCard(CardAsset asset)
    {
        // �÷����� Ž�� ���̶�� ����ó��
        if (!InDeckBuildingMode)
        {
            return;

        }

        // ���� �̹� ���� á�ٸ� ����ó��
        if (deckList.Count == AmountOfCardsInDeck)
        {
            return;
        }

        int count = NumberOfThisCardInDeck(asset);

        int limitOfThisCardInDeck = SameCardLimit;

        // CardAsset�� �ٸ� ������ ��õǾ� �ִٸ�, �װ��� ����մϴ�.
        if (asset.LimitOfThisCardInDeck > 0)
            limitOfThisCardInDeck = asset.LimitOfThisCardInDeck;

        if (count < limitOfThisCardInDeck)
        {
            deckList.Add(asset);

            CheckDeckCompleteFrame();

            // ī�带 �߰��ϸ� ������ �ϳ� ������ŵ�ϴ�.
            count++;

            // �׷��� ���� �۾� ����
            if (ribbons.ContainsKey(asset))
            {
                // ���� ������Ʈ
                ribbons[asset].SetQuantity(count);
            }
            else
            {
                // 1) ī���� �̸��� ��Ͽ� �߰�
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
        DeckCompleteFrame.SetActive(deckList.Count == AmountOfCardsInDeck);
        if(deckList.Count == AmountOfCardsInDeck)
        {
            _deckBuilderDoneButton.interactable = true;
        }
        else
        {
            _deckBuilderDoneButton.interactable = false;
        }
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
        Debug.Log("InRemoveCard");
        if (ribbons.ContainsKey(asset))
        {
            CardNameRibbon ribbonToRemove = ribbons[asset];
            ribbonToRemove.SetQuantity(ribbonToRemove.Quantity - 1);

            if (NumberOfThisCardInDeck(asset) == 1)
            {
                ribbons.Remove(asset);
                if (ribbonToRemove != null && ribbonToRemove.gameObject != null)
                {
                    Destroy(ribbonToRemove.gameObject);
                }
            }
        }

        deckList.Remove(asset);

        CheckDeckCompleteFrame();

        if (DeckBuildingScreen.instance.CollectionBrowser != null)
        {
            DeckBuildingScreen.instance.CollectionBrowser.UpdateQuantitiesOnPage();
        }
    }

    public void BuildADeckFor(CharacterAsset asset)
    {
        InDeckBuildingMode = true;
        buildingForCharacter = asset;
        // TODO: �� ��Ͽ� ī�尡 �ִٸ� ��� �����մϴ�. 
        // ���� ��ϰ� �ð������� ��� ī�� ��� �׸��� �����մϴ�.
        while (deckList.Count > 0)
        {
            RemoveCard(deckList[0]);
        }

        // ĳ���� Ŭ������ �����ϰ� ���� Ȱ��ȭ�մϴ�.
        DeckBuildingScreen.instance.TabsScript.SetClassOnClassTab(asset);
        DeckBuildingScreen.instance.CollectionBrowser.ShowCollectionForDeckBuilding(asset);

        CheckDeckCompleteFrame();

        // �Է� �ʵ��� �ؽ�Ʈ�� �� ���ڿ��� �ʱ�ȭ�մϴ�.
        DeckName.text = "";
    }


    public void DoneButtonHandler()
    {
        
        DeckInfo deckToSave = new DeckInfo(deckList, DeckName.text, buildingForCharacter);
        DecksStorage.instance.AllDecks.Add(deckToSave);
        DecksStorage.instance.SaveDecksIntoPlayerPrefs();
        DeckBuildingScreen.instance.ShowScreenForCollectionBrowsing();
    }

    void OnApplicationQuit()
    {
        // �� ��ġ�ٰ� ������ ���� �ϴ��� ��·�� ���� �����ؾ���
        DoneButtonHandler();
    }

    private void OnClickDoneButton()
    {
        if(deckList.Count == AmountOfCardsInDeck)
        {
            DoneButtonHandler();
            gameObject.SetActive(false);
            listOfDecksInCollection.ScrollViewSetActiveTrue();
            listOfDecksInCollection.UpdateList();
        }
    }
}
