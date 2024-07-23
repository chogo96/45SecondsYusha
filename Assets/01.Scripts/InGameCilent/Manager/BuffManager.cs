using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    //���������
    public bool BleedDebuff = false;
    /// <summary>
    /// ���� ������� �۵��ϴ� ����
    /// </summary>
    public float BleedInterval = 2.0f;


    private Deck _deck;
    private Coroutine _bleedCoroutine;

    private void Awake()
    {
        _deck = GameObject.Find("Deck1").GetComponent<Deck>();
    }
    public void ApplyBleedEffect()
    {
        if (!BleedDebuff)
        {
            BleedDebuff = true;
            _bleedCoroutine = StartCoroutine(BleedEffect());
        }
    }
    public void RemoveBleedEffect()
    {
        if (BleedDebuff)
        {
            BleedDebuff = false;
            StopCoroutine(_bleedCoroutine);
        }
    }

    private IEnumerator BleedEffect()
    {
        while (BleedDebuff)
        {
            if(_deck.GetCardCount()> 0)
            {
                CardAsset cardAsset = _deck.DrawCard();
                if(cardAsset != null)
                {
                    _deck.DiscardDeck.Add(cardAsset);
                    Debug.Log("������ ī�� ������" + cardAsset.CardScriptName);
                }
            }
            yield return new WaitForSeconds(BleedInterval);
        }
    }
}
