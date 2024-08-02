using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DeckSelectionUI : MonoBehaviour
{
    public GameObject ScreenContent; // 덱 선택 화면의 메인 패널
    public Transform DeckListContent; // ScrollView의 Content
    public GameObject DeckButtonPrefab; // 덱 버튼 프리팹

    private void Awake()
    {
        HideScreen();
    }

    public void ShowScreen()
    {
        if (ScreenContent == null || DeckListContent == null || DeckButtonPrefab == null)
        {
            return;
        }

        ScreenContent.SetActive(true);
        ShowDeckList();
    }

    public void HideScreen()
    {
        ScreenContent.SetActive(false);
    }

    private void ShowDeckList()
    {

        if (DecksStorage.instance == null)
        {
            return;
        }

        if (DecksStorage.instance.AllDecks == null)
        {
            return;
        }

        // 기존 버튼 제거
        foreach (Transform child in DeckListContent)
        {
            Destroy(child.gameObject);
        }

        // 덱 버튼 생성
        foreach (DeckInfo info in DecksStorage.instance.AllDecks)
        {
            if (info.IsComplete())
            {
                GameObject deckButtonObj = Instantiate(DeckButtonPrefab, DeckListContent);
                SelectDeckButton deckButton = deckButtonObj.GetComponent<SelectDeckButton>();
                deckButton.SetDeckInfo(info);
            }
        }
    }

    private void OnDeckButtonClick(int index)
    {
        DeckInfo selectedDeck = DecksStorage.instance.AllDecks[index];
        DeckGameManager.instance.SetSelectedDeck(selectedDeck.Cards);
        HideScreen();
    }
}