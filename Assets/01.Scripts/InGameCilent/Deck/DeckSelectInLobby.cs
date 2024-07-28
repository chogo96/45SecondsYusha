using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckSelectInLobby : MonoBehaviour
{
    public List<Deck> decks;
    private Deck selectedDeck;

    // �� ���� �޼ҵ�
    public void SelectDeck(int deckIndex)
    {
        List<CardAsset> selectedDeckCards = decks[deckIndex].Cards;
        DeckGameManager.instance.SetSelectedDeck(selectedDeckCards);
    }

    // ���� ���� �޼ҵ�
    public void StartGame()
    {
        // ���� ������ �̵�
        SceneManager.LoadScene("05.GamePlay Scene");
    }
}
