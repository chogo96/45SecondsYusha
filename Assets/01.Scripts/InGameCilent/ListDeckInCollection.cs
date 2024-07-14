using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListOfDecksInCollection : MonoBehaviour
{

    public Transform Content;
    //프리팹 요구 항목들
    public GameObject DeckInListPrefab;
    public GameObject NewDeckButtonPrefab;

    public void UpdateList()
    {
        // 모든 덱 아이콘을 먼저 지운다.
        foreach (Transform transform in Content)
        {
            if (transform != Content)
            {
                Destroy(transform.gameObject);
            }
        }
        // load the information about decks from DecksStorage
        foreach (DeckInfo info in DecksStorage.instance.AllDecks)
        {
            // create a new DeckInListPrefab;
            GameObject g = Instantiate(DeckInListPrefab, Content);
            g.transform.localScale = Vector3.one;
            DeckInScrollList deckInScrollListComponent = g.GetComponent<DeckInScrollList>();
            deckInScrollListComponent.ApplyInfo(info);
        }

        // if there is room to create more decks, create a NewDeckButton
        if (DecksStorage.instance.AllDecks.Count < 9)
        {
            GameObject g = Instantiate(NewDeckButtonPrefab, Content);
            g.transform.localScale = Vector3.one;
        }
    }

}
