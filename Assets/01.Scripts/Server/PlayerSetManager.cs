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



        // GameManager�� AllPlayersSpawned �̺�Ʈ ����
        GameManager.AllPlayersSpawned += Reset;
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
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
    /// ī����, ��ο� ������ Deck text �� Hand text �� �������� �鼭 ������ �������ִ� �Լ�.
    /// </summary>
    /// <param name="playerNumber">���� �÷��̾� ���� �ѹ�</param>
    /// <param name="cardNum">�����ī�� �� or �̴� ī�� ��</param>
    /// <param name="plusMinus">Minus = ī���� / Plus = ī���ο�</param>
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

        // ���� �����ִ��� ���� �����ָ鼭
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

        // ī��޸�  ���� ���� ����
    }


    /// <summary>
    /// ����� �̹��� Ȱ��ȭ �Լ�
    /// </summary>
    /// <param name="playerID"> ���� �÷��̾� ���� �ѹ�(Ÿ��)</param>
    /// <param name="deBuff">����� ���� (bleed, blind, confusion)</param>
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
    /// ����� �̹��� ��Ȱ��ȭ �Լ�
    /// </summary>
    /// <param name="playerID">���� �÷��̾� ���� �ѹ�(Ÿ��)</param>
    /// <param name="deBuff">����� ���� (bleed, blind, confusion)</param>
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
