using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckSelectInLobby : MonoBehaviour
{
    public List<Deck> decks;
    private Deck selectedDeck;

    // 덱 선택 메소드
    public void SelectDeck(int deckIndex)
    {
        List<CardAsset> selectedDeckCards = decks[deckIndex].Cards;
        DeckGameManager.instance.SetSelectedDeck(selectedDeckCards);
    }

    // 게임 시작 메소드
    public void StartGame()
    {
        // 게임 신으로 이동
        SceneManager.LoadScene("05.GamePlay Scene");
    }
}
