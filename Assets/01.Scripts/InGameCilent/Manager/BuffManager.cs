using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class BuffManager : MonoBehaviourPun
{
    //���������
    public bool BleedDebuff = false;
    public bool BlindDebuff = false;
    public bool ConfusionDebuff = false;
    /// <summary>
    /// ���� ������� �۵��ϴ� ����
    /// </summary>
    public float BleedInterval = 2.0f;
    public static BuffManager instance;

    private Deck _deck;
    private Coroutine _bleedCoroutine;

    // ����� �̹���
    //[SerializeField] private GameObject bleedDebuffImage;
    //[SerializeField] private GameObject blindDebuffImage;
    //[SerializeField] private GameObject confusionDebuffImage;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        //yield return new WaitUntil(() => GameObject.Find("BleedDebuffImage") != null);
        //bleedDebuffImage = GameObject.Find("BleedDebuffImage");
        //if (bleedDebuffImage == null)
        //{
        //    Debug.LogError("BleedDebuffImage�� ã�� �� �����ϴ�.");
        //}

        //yield return new WaitUntil(() => GameObject.Find("BlindDebuffImage") != null);
        //blindDebuffImage = GameObject.Find("BlindDebuffImage");
        //if (blindDebuffImage == null)
        //{
        //    Debug.LogError("BlindDebuffImage�� ã�� �� �����ϴ�.");
        //}

        //yield return new WaitUntil(() => GameObject.Find("ConfusionDebuffImage") != null);
        //confusionDebuffImage = GameObject.Find("ConfusionDebuffImage");
        //if (confusionDebuffImage == null)
        //{
        //    Debug.LogError("ConfusionDebuffImage�� ã�� �� �����ϴ�.");
        //}

        yield return new WaitUntil(() => GameObject.Find("Deck1") != null);
        GameObject deckObject = GameObject.Find("Deck1");
        if (deckObject == null)
        {
            Debug.LogError("Deck1 ������Ʈ�� ã�� �� �����ϴ�.");
        }
        else
        {
            _deck = deckObject.GetComponent<Deck>();
            if (_deck == null)
            {
                Debug.LogError("Deck1 ������Ʈ���� Deck ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
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
            if (_bleedCoroutine != null)
            {
                StopCoroutine(_bleedCoroutine);
            }
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
            if (_deck != null && _deck.GetCardCount() > 0)
            {
                CardAsset cardAsset = _deck.DrawCard();
                if (cardAsset != null)
                {
                    _deck.DiscardDeck.Add(cardAsset);
                    Debug.Log("������ ī�� ������: " + cardAsset.CardScriptName);
                }
            }
            yield return new WaitForSeconds(BleedInterval);
        }
    }
}
