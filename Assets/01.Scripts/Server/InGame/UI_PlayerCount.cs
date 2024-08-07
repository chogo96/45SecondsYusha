using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_PlayerCount : MonoBehaviourPunCallbacks
{
    private PlayerSetManager playerSetManager;

    private int _baseHandCards;
    private int _baseDeckCards;

    private TMP_Text _deckCountText;
    private TMP_Text _handCountText;

    private string _nickName;

    private void Awake()
    {
        _nickName = PhotonNetwork.LocalPlayer.NickName;
        playerSetManager = FindObjectOfType<PlayerSetManager>();
    }

    private void Start()
    {
        _handCountText = transform.Find("HandImage/HandCountText (TMP)").GetComponent<TMP_Text>();
        _deckCountText = transform.Find("DeckImage/DeckCountText (TMP)").GetComponent<TMP_Text>();
    }

    /// <summary>
    /// ī����, ��ο� ������ Deck text �� Hand text �� �������� �鼭 ������ �������ִ� �Լ�.
    /// </summary>
    /// <param name="playerNumber">���� �÷��̾� ���� �ѹ�</param>
    /// <param name="plusMinus">Minus = ī���� / Plus = ī���ο�</param>
    /// <param name="deckCardCount">���� ���� ��</param>
    /// <param name="handCardCount">���� ������ ��</param>
    public void HandCardCount(string plusMinus, int deckCardCount, int handCardCount)
    {
        // ���� ������Ʈ
        // UpdateHandCardCount(plusMinus, deckCardCount, handCardCount);

        // �ٸ� Ŭ���̾�Ʈ�� ������Ʈ ��û
        playerSetManager.photonView.RPC("UpdateHandCardCount", RpcTarget.All, plusMinus, deckCardCount, handCardCount, _nickName);
    }


    private void UpdateHandCardCount(string plusMinus, int deckCardCount, int handCardCount)
    {
        _baseHandCards = handCardCount;
        _baseDeckCards = deckCardCount;

        // ���� �� �� �ִ��� ���� �����ֱ�
        switch (plusMinus)
        {
            case "Minus":
                _handCountText.text = $"{_baseHandCards}";
                break;
            case "Plus":
                _handCountText.text = $"{_baseHandCards}";
                _deckCountText.text = $"{_baseDeckCards}";
                if (_baseDeckCards <= 0)
                {
                    _deckCountText.text = $"0";
                }
                if (_baseHandCards <= 0)
                {
                    _handCountText.text = $"0";
                }
                break;
            default:
                break;
        }
    }
}
