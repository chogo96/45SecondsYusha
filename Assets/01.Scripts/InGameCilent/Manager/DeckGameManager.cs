using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckGameManager : MonoBehaviour
{
    public static DeckGameManager instance { get; private set; }
    public List<CardAsset> selectedDeckCards;
    public DeckInfo selectedDeckInfo;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetSelectedDeck(List<CardAsset> cards)
    {
        selectedDeckCards = cards;
    }

    public List<CardAsset> GetSelectedDeckCards()
    {
        return selectedDeckCards;
    }
    public void SetSelectedDeckInfo(DeckInfo deckInfo)
    {
        selectedDeckInfo = deckInfo;
    }
    public DeckInfo GetSelectedDeckInfo()
    {
        return selectedDeckInfo;
    }
    public void GoToLobby()
    {
        SceneManager.LoadScene("04.Lobby Scene");
    }
}
