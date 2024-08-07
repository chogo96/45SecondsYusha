using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    void Awake()
    {
        DeckCompleteFrame.GetComponent<Image>().raycastTarget = false;
    }

    public void AddCard(CardAsset asset)
    {
        // 컬렉션을 탐색 중이라면 예외처리
        if (!InDeckBuildingMode)
        {
            return;

        }

        // 덱이 이미 가득 찼다면 예외처리
        if (deckList.Count == AmountOfCardsInDeck)
        {
            return;
        }

        int count = NumberOfThisCardInDeck(asset);

        int limitOfThisCardInDeck = SameCardLimit;

        // CardAsset에 다른 제한이 명시되어 있다면, 그것을 사용합니다.
        if (asset.LimitOfThisCardInDeck > 0)
            limitOfThisCardInDeck = asset.LimitOfThisCardInDeck;

        if (count < limitOfThisCardInDeck)
        {
            deckList.Add(asset);

            CheckDeckCompleteFrame();

            // 카드를 추가하면 수량을 하나 증가시킵니다.
            count++;

            // 그래픽 관련 작업 수행
            if (ribbons.ContainsKey(asset))
            {
                // 수량 업데이트
                ribbons[asset].SetQuantity(count);
            }
            else
            {
                // 1) 카드의 이름을 목록에 추가
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
        DeckCompleteFrame.SetActive(deckList.Count == AmountOfCardsInDeck);
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
        // TODO: 덱 목록에 카드가 있다면 모두 제거합니다. 
        // 실제 목록과 시각적으로 모든 카드 목록 항목을 삭제합니다.
        while (deckList.Count > 0)
        {
            RemoveCard(deckList[0]);
        }

        // 캐릭터 클래스를 적용하고 탭을 활성화합니다.
        DeckBuildingScreen.instance.TabsScript.SetClassOnClassTab(asset);
        DeckBuildingScreen.instance.CollectionBrowser.ShowCollectionForDeckBuilding(asset);

        CheckDeckCompleteFrame();

        // 입력 필드의 텍스트를 빈 문자열로 초기화합니다.
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
        // 덱 고치다가 게임을 끈다 하더라도 어쨌든 덱을 저장해야함
        DoneButtonHandler();
    }
}
