using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : SingletonMonoBase<CardManager>
{
    [SerializeField]ItemSO _item;

    public GameObject _cardPrefab;

    List<Item> _itemBuffer;

    public Item PopItem() 
    {
        if (_itemBuffer.Count == 0)
        {
            SetupItemBuffer();
        }
        Item card = _itemBuffer[0];
        _itemBuffer.RemoveAt(0);
        return card;
    }
    void SetupItemBuffer()
    {
        _itemBuffer = new List<Item>();
        for (int i = 0; i < _item.cards.Length; i++)
        {
            Item card = _item.cards[i];
            for(int j = 0; j < card.percent; j++)
            {
                _itemBuffer.Add(card);
            }
        }
        for (int i = 0; i< _itemBuffer.Count; i++)
        {
            int rand = Random.Range(0, _itemBuffer.Count);
            Item temp = _itemBuffer[i];
            _itemBuffer[i] = _itemBuffer[rand];
            _itemBuffer[rand] = temp;
        }
    }

    private void Start()
    {
        SetupItemBuffer();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            AddCard(true);
        }
    }
    void AddCard(bool isMine)
    {
        var cardObject = Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity);
        var card = cardObject.GetComponent<Card>();
        card.Setup(PopItem(), isMine);
    }
}
