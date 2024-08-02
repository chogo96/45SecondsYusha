using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetManager : MonoBehaviourPunCallbacks
{
    PlayerScripts playerScripts;
    Hand hand;

    private Image[] _playerImage;

    private GameObject[] _bleedDebuffImage;
    private GameObject[] _blindDebuffImage;
    private GameObject[] _confusionDebuffImage;

    private TMP_Text[] _deckCountText;
    private TMP_Text[] _handCountText;

    private int _playerCount;

    private int[] _baseHandCards;
    private int[] _baseDeckCards;
    private int _actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

    private GameObject[] _playerHandArea;


    private void Awake()
    {

        _playerCount = PhotonNetwork.PlayerList.Length + 1;

        _playerImage = new Image[_playerCount];
        _bleedDebuffImage = new GameObject[_playerCount];
        _blindDebuffImage = new GameObject[_playerCount];
        _confusionDebuffImage = new GameObject[_playerCount];
        _deckCountText = new TMP_Text[_playerCount];
        _handCountText = new TMP_Text[_playerCount];
        _playerHandArea = new GameObject[_playerCount];

        _baseHandCards = new int[_playerCount];
        _baseDeckCards = new int[_playerCount];



        // GameManager의 AllPlayersSpawned 이벤트 구독
        GameManager.AllPlayersSpawned += Reset;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        GameManager.AllPlayersSpawned -= Reset;
    }

    private void Reset()
    {
        playerScripts = GetComponentInChildren<PlayerScripts>();
        hand = GetComponentInChildren<Hand>();

        for (int i = 1; i < _playerCount; i++)
        {
            Transform playerTransform = transform.Find($"Player_{i}");
            if (playerTransform == null)
            {
                Debug.LogError($"Player_{i} not found");
                continue;
            }

            _playerImage[i] = playerTransform.Find("Player_Image")?.GetComponent<Image>();


            _bleedDebuffImage[i] = playerTransform.Find("BleedDebuffImage")?.gameObject;
            _blindDebuffImage[i] = playerTransform.Find("BlindDebuffImage")?.gameObject;
            _confusionDebuffImage[i] = playerTransform.Find("ConfusionDebuffImage")?.gameObject;
            _handCountText[i] = playerTransform.Find("HandImage/HandCountText (TMP)")?.GetComponent<TMP_Text>();
            _deckCountText[i] = playerTransform.Find("DeckImage/DeckCountText (TMP)")?.GetComponent<TMP_Text>();

            _playerHandArea[i] = playerTransform.Find($"HandArea")?.gameObject;
            _playerImage[i].sprite = playerScripts.charAsset.AvatarImage;

        }



        HandCardCount(_actorNumber, "Minus");
    }



    /// <summary>
    /// 카드사용, 드로우 했을때 Deck text 랑 Hand text 를 갱신해주 면서 서버와 연동해주는 함수.
    /// </summary>
    /// <param name="playerNumber">포톤 플레이어 액터 넘버</param>
    /// <param name="cardNum">사용한카드 수 or 뽑는 카드 수</param>
    /// <param name="plusMinus">Minus = 카드사용 / Plus = 카드드로우</param>
    [PunRPC]
    public void HandCardCount(int playerNumber,string plusMinus)
    {
        int GetListCardCount(List<CardLogic> list)
        {
            return list.Count;
        }
        // _baseHandCards[playerNumber] = GetCardCount(_playerHandArea[playerNumber]);

        _baseHandCards[playerNumber] = GetListCardCount(hand.CardsInHand);

        int GetListCount(List<CardAsset> list)
        {
            return list.Count;
        }
        _baseDeckCards[playerNumber] = GetListCount( playerScripts._deck.Cards);

        // 손패 몇장있는지 숫자 보여주면서
        switch (plusMinus)
        {
            case "Minus":
                _handCountText[playerNumber].text = $"{_baseHandCards[playerNumber]}";
                break;
            case "Plus":
                _handCountText[playerNumber].text = $"{_baseHandCards[playerNumber]}";
                _deckCountText[playerNumber].text = $"{_baseDeckCards[playerNumber]}";
                if(_baseDeckCards[playerNumber] <= 0)
                {
                    _deckCountText[playerNumber].text = $"0";
                }
                break;
            default:
                break;
        }

        // 카드뒷면  생성 삭제 로직
    }


    /// <summary>
    /// 디버프 이미지 활성화 함수
    /// </summary>
    /// <param name="playerID"> 포톤 플레이어 액터 넘버(타겟)</param>
    /// <param name="deBuff">디버프 종류 (bleed, blind, confusion)</param>
    [PunRPC]
    public void DeBuffImageOn(int playerID, string deBuff)
    {
        switch (deBuff)
        {
            case "bleed":
                _bleedDebuffImage[playerID]?.SetActive(true);
                break;
            case "blind":
                _blindDebuffImage[playerID]?.SetActive(true);
                break;
            case "confusion":
                _confusionDebuffImage[playerID]?.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 디버프 이미지 비활성화 함수
    /// </summary>
    /// <param name="playerID">포톤 플레이어 액터 넘버(타겟)</param>
    /// <param name="deBuff">디버프 종류 (bleed, blind, confusion)</param>
    [PunRPC]
    public void DeBuffImageOff(int playerID, string deBuff)
    {
        switch (deBuff)
        {
            case "bleed":
                _bleedDebuffImage[playerID]?.SetActive(false);
                break;
            case "blind":
                _blindDebuffImage[playerID]?.SetActive(false);
                break;
            case "confusion":
                _confusionDebuffImage[playerID]?.SetActive(false);
                break;
        }
    }
}
