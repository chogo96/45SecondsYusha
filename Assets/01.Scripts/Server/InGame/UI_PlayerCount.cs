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
    /// 카드사용, 드로우 했을때 Deck text 랑 Hand text 를 갱신해주 면서 서버와 연동해주는 함수.
    /// </summary>
    /// <param name="playerNumber">포톤 플레이어 액터 넘버</param>
    /// <param name="plusMinus">Minus = 카드사용 / Plus = 카드드로우</param>
    /// <param name="deckCardCount">현재 덱의 수</param>
    /// <param name="handCardCount">현재 손패의 수</param>
    public void HandCardCount(string plusMinus, int deckCardCount, int handCardCount)
    {
        // 로컬 업데이트
        // UpdateHandCardCount(plusMinus, deckCardCount, handCardCount);

        // 다른 클라이언트에 업데이트 요청
        playerSetManager.photonView.RPC("UpdateHandCardCount", RpcTarget.All, plusMinus, deckCardCount, handCardCount, _nickName);
    }


    private void UpdateHandCardCount(string plusMinus, int deckCardCount, int handCardCount)
    {
        _baseHandCards = handCardCount;
        _baseDeckCards = deckCardCount;

        // 손패 몇 장 있는지 숫자 보여주기
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
