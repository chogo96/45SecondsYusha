using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListOfDecksInCollection : MonoBehaviour
{

    public Transform Content;
    //프리팹 요구 항목들
    public GameObject DeckInListPrefab;
    public GameObject NewDeckButtonPrefab;
    private GameObject _scrollView;

    private void Awake()
    {
        _scrollView = transform.Find("Scroll View").gameObject;
    }

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
        
        foreach (DeckInfo info in DecksStorage.instance.AllDecks)
        {
            GameObject g = Instantiate(DeckInListPrefab, Content);
            g.transform.localScale = Vector3.one;
            DeckInScrollList deckInScrollListComponent = g.GetComponent<DeckInScrollList>();
            deckInScrollListComponent.ApplyInfo(info);
        }

        if (DecksStorage.instance.AllDecks.Count < 9)
        {
            GameObject g = Instantiate(NewDeckButtonPrefab, Content);
            g.transform.localScale = Vector3.one;
        }
    }
    
    public void ScrollViewSetActiveTrue()
    {
        _scrollView.SetActive(true);
    }

}
