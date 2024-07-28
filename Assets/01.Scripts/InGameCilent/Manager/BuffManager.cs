using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BuffManager : MonoBehaviourPun
{
    //출혈디버프
    public bool BleedDebuff = false;
    public bool BlindDebuff = false;
    public bool ConfusionDebuff = false;
    /// <summary>
    /// 출혈 디버프가 작동하는 간격
    /// </summary>
    public float BleedInterval = 2.0f;
    public static BuffManager instance;

    private Deck _deck;
    private Coroutine _bleedCoroutine;


    // 디버프 이미지
    [SerializeField] private GameObject bleedDebuffImage;
    [SerializeField] private GameObject blindDebuffImage;
    [SerializeField] private GameObject confusionDebuffImage;





    private void Awake()
    {
        _deck = GameObject.Find("Deck1").GetComponent<Deck>();
        instance = this;
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
    public void ApplyBlindEffect()
    {
        if (!BlindDebuff)
        {
            BlindDebuff = true;
        }
    }

    public void RemoveBlindEffect()
    {
        if (BlindDebuff)
        {
            BlindDebuff = false;
        }
    }

    public void ApplyConfusionEffect()
    {
        if (!ConfusionDebuff)
        {
            ConfusionDebuff = true;
        }
    }
    public void RemoveConfusionEffect()
    {
        if (ConfusionDebuff)
        {
            ConfusionDebuff = false;
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
                    Debug.Log("출혈로 카드 버려짐" + cardAsset.CardScriptName);
                }
            }
            yield return new WaitForSeconds(BleedInterval);
        }
    }
}
