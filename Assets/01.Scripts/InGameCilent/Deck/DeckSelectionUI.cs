using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DeckSelectionUI : MonoBehaviour
{
    public GameObject ScreenContent; // �� ���� ȭ���� ���� �г�
    public Transform DeckListContent; // ScrollView�� Content
    public GameObject DeckButtonPrefab; // �� ��ư ������

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

        // ���� ��ư ����
        foreach (Transform child in DeckListContent)
        {
            Destroy(child.gameObject);
        }

        // �� ��ư ����
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