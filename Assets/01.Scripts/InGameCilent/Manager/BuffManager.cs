using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.XR;

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

    // 플레이어 셋 매니저에서 쓸 값들
    PlayerScripts playerScripts;
    PlayerSetManager playerSetManager;
    private int _playerID = PhotonNetwork.LocalPlayer.ActorNumber;

    // 디버프 이미지
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
        // 이벤트 구독 해제
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
            Utils.LogRed("Deck1 오브젝트를 찾을 수 없습니다.");
        }
        else
        {
            _deck = deckObject.GetComponent<Deck>();
            if (_deck == null)
            {
                Utils.LogRed("Deck1 오브젝트에서 Deck 컴포넌트를 찾을 수 없습니다.");
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
                    Utils.Log("출혈로 카드 버려짐: " + cardAsset.CardScriptName);

                    playerScripts.UpdateCardCounts("Plus");
                }
            }
            yield return new WaitForSeconds(BleedInterval);
        }
    }
}
