using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckSelectionScreen : MonoBehaviour
{

    public GameObject ScreenContent;
    public DeckIcon[] DeckIcons;
    public HeroInfoPanel HeroPanelDeckSelection;

    public static DeckSelectionScreen instance;

    void Awake()
    {
        instance = this;
        HideScreen();
    }

    public void ShowDecks()
    {
        // If there are no decks at all, show the character selection screen
        if (DecksStorage.instance.AllDecks.Count == 0)
        {
            HideScreen();
            CharacterSelectionScreen.instance.ShowScreen();
            return;
        }

        // disable all deck icons first
        foreach (DeckIcon icon in DeckIcons)
        {
            icon.gameObject.SetActive(false);
            icon.InstantDeselect();
        }

        for (int j = 0; j < DecksStorage.instance.AllDecks.Count; j++)
        {
            DeckIcons[j].ApplyLookToIcon(DecksStorage.instance.AllDecks[j]);
            DeckIcons[j].gameObject.SetActive(true);
        }
    }

    public void ShowScreen()
    {
        ScreenContent.SetActive(true);
        ShowDecks();
        HeroPanelDeckSelection.OnOpen();
    }

    public void HideScreen()
    {
        ScreenContent.SetActive(false);
    }
    public void GoToPlay()
    {
        SceneManager.LoadScene("05.GamePlay Scene");
    }
}
