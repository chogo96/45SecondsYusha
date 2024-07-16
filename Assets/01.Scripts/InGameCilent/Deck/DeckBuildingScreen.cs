using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckBuildingScreen : MonoBehaviour
{

    public GameObject ScreenContent;
    public GameObject ReadyDecksList;
    public GameObject CardsInDeckList;
    public DeckBuilder BuilderScript;
    public ListOfDecksInCollection ListOfMadeDeck;
    public CollectionBrowser CollectionBrowser;
    public CharacterSelectionTabs TabsScript;
    public bool ShowReducedQuantities = true;

    public static DeckBuildingScreen instance;

    void Awake()
    {
        instance = this;
        HideScreen();
    }

    public void ShowScreenForCollectionBrowsing()
    {
        ScreenContent.SetActive(true);
        ReadyDecksList.SetActive(true);
        CardsInDeckList.SetActive(false);
        BuilderScript.InDeckBuildingMode = false;
        ListOfMadeDeck.UpdateList();

        CollectionBrowser.AllCharactersTabs.gameObject.SetActive(true);
        CollectionBrowser.OneCharacterTabs.gameObject.SetActive(false);
        Canvas.ForceUpdateCanvases();

        CollectionBrowser.ShowCollectionForBrowsing();
    }

    public void ShowScreenForDeckBuilding()
    {
        ScreenContent.SetActive(true);
        ReadyDecksList.SetActive(false);
        CardsInDeckList.SetActive(true);

        CollectionBrowser.AllCharactersTabs.gameObject.SetActive(false);
        CollectionBrowser.OneCharacterTabs.gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
        // TODO:  탭에 우리가 덱을 빌드하고 있는 캐릭터 클래스의 이름을 표시하도록 업데이트하고, 탭의 스크립트를 업데이트해야함
    }

    public void BuildADeckFor(CharacterAsset asset)
    {
        ShowScreenForDeckBuilding();
        CollectionBrowser.ShowCollectionForDeckBuilding(asset);
        TabsScript.SetClassOnClassTab(asset);
        BuilderScript.BuildADeckFor(asset);
    }

    public void HideScreen()
    {
        ScreenContent.SetActive(false);
        CollectionBrowser.ClearCreatedCards();
    }
}
