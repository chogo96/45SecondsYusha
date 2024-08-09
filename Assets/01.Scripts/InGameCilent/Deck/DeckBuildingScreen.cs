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
    public CharacterSelectionTabs TabsScript;
    public bool ShowReducedQuantities = true;

    public static DeckBuildingScreen instance;

    private FirebaseCardManager firebaseCardManager;

    void Awake()
    {
        instance = this;
        firebaseCardManager = FindObjectOfType<FirebaseCardManager>();
        HideScreen();
    }

    public void ShowScreenForCollectionBrowsing()
    {
        ScreenContent.SetActive(true);
        ReadyDecksList.SetActive(true);
        CardsInDeckList.SetActive(false);
        BuilderScript.InDeckBuildingMode = false;
        ListOfMadeDeck.UpdateList();

        Canvas.ForceUpdateCanvases();

        firebaseCardManager.SetSelectedCharacter(null); // 모든 캐릭터의 카드를 표시
    }

    public void ShowScreenForDeckBuilding()
    {
        ScreenContent.SetActive(true);
        ReadyDecksList.SetActive(false);
        CardsInDeckList.SetActive(true);

        Canvas.ForceUpdateCanvases();
    }

    public void BuildADeckFor(CharacterAsset asset)
    {
        ShowScreenForDeckBuilding();
        firebaseCardManager.SetSelectedCharacter(asset); // 선택된 캐릭터의 카드만 표시
        TabsScript.SetClassOnClassTab(asset);
        BuilderScript.BuildADeckFor(asset);
    }

    public void HideScreen()
    {
        ScreenContent.SetActive(false);
        firebaseCardManager.ClearCreatedCards();
    }
}
