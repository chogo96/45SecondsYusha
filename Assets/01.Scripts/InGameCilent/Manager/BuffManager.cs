using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.XR;

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

    // �÷��̾� �� �Ŵ������� �� ����
    PlayerScripts playerScripts;
    PlayerSetManager playerSetManager;
    private int _playerID = PhotonNetwork.LocalPlayer.ActorNumber;

    // ����� �̹���
    //[SerializeField] private GameObject bleedDebuffImage;
    //[SerializeField] private GameObject blindDebuffImage;
    //[SerializeField] private GameObject confusionDebuffImage;

    private void Awake()
    {
        instance = this;
        playerSetManager = FindObjectOfType<PlayerSetManager>();
        InsertScripts.OnScriptsInserted += PlayerScriptsSetStart;

    }

    public void PlayerScriptsSetStart()
    {
        playerScripts = FindObjectOfType<PlayerScripts>();
    }
    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        InsertScripts.OnScriptsInserted -= PlayerScriptsSetStart;
    }

    private void OnEnable()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitUntil(() => GameObject.Find("Deck1") != null);
        GameObject deckObject = GameObject.Find("Deck1");
        if (deckObject == null)
        {
            Utils.LogRed("Deck1 ������Ʈ�� ã�� �� �����ϴ�.");
        }
        else
        {
            _deck = deckObject.GetComponent<Deck>();
            if (_deck == null)
            {
                Utils.LogRed("Deck1 ������Ʈ���� Deck ������Ʈ�� ã�� �� �����ϴ�.");
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
                playerSetManager.photonView.RPC("DeBuffImageOff", RpcTarget.All, _playerID, "bleed");

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
            playerSetManager.photonView.RPC("DeBuffImageOff", RpcTarget.All, _playerID, "blind");
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
            playerSetManager.photonView.RPC("DeBuffImageOff", RpcTarget.All, _playerID, "confusion");
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
                    Utils.Log("������ ī�� ������: " + cardAsset.CardScriptName);

                    playerScripts.UpdateCardCounts("Plus");
                }
            }
            yield return new WaitForSeconds(BleedInterval);
        }
    }
}
