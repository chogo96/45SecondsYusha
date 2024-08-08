using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
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

    private int _playerCount;
    private TMP_Text[] _playerDeckCount;
    private TMP_Text[] _playerHandCount;
    private string[] _Nickname;
    private int _baseHandCards;
    private int _baseDeckCards;

    private int _actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

    private GameObject[] _playerHandArea;


    private void Awake()
    {
        _playerCount = PhotonNetwork.PlayerList.Length + 1;

        _playerImage = new Image[_playerCount];
        _bleedDebuffImage = new GameObject[_playerCount];
        _blindDebuffImage = new GameObject[_playerCount];
        _confusionDebuffImage = new GameObject[_playerCount];
        _playerHandArea = new GameObject[_playerCount];

        _playerDeckCount = new TMP_Text[_playerCount];
        _playerHandCount = new TMP_Text[_playerCount];
        _Nickname = new string[_playerCount];

        // GameManager의 AllPlayersSpawned 이벤트 구독
        InsertScripts.OnScriptsInserted += Reset;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        InsertScripts.OnScriptsInserted -= Reset;
    }

    private void Reset()
    {
        string localPlayerName = $"{PhotonNetwork.LocalPlayer.NickName}";
        playerScripts = transform.Find(localPlayerName)?.GetComponent<PlayerScripts>();
        hand = transform.Find($"{localPlayerName}/Hand")?.GetComponent<Hand>();

        if (playerScripts == null || hand == null)
        {
            Utils.LogRed($"{localPlayerName} or Hand component not found");
            return;
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player player = PhotonNetwork.PlayerList[i];
            string playerName = $"{player.NickName}";
            Transform playerTransform = transform.Find(playerName);
            if (playerTransform == null)
            {
                Utils.LogRed($"{playerName} not found");
                continue;
            }

            _playerImage[i + 1] = playerTransform.Find("Player_Image")?.GetComponent<Image>();
            _bleedDebuffImage[i + 1] = playerTransform.Find("BleedDebuffImage")?.gameObject;
            _blindDebuffImage[i + 1] = playerTransform.Find("BlindDebuffImage")?.gameObject;
            _confusionDebuffImage[i + 1] = playerTransform.Find("ConfusionDebuffImage")?.gameObject;
            _playerHandArea[i + 1] = playerTransform.Find("HandArea")?.gameObject;
            _Nickname[i + 1] = player.NickName;
            _playerDeckCount[i + 1] = playerTransform.Find("DeckImage/DeckCountText (TMP)")?.GetComponent<TMP_Text>();
            _playerHandCount[i + 1] = playerTransform.Find("HandImage/HandCountText (TMP)")?.GetComponent<TMP_Text>();

            // 여기서 필요한 초기화나 설정 작업을 수행합니다.
        }

        StartCoroutine(SetCustomPropertiesCharacterImage());
    }
    /// <summary>
    /// 플레이어 스크립트의 덱이 Null값이기전에 이미지바꾸는걸 시도하지 않는 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator SetCustomPropertiesCharacterImage()
    {
        yield return new WaitForSeconds(0.5f);
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["PlayerImage"] = playerScripts.charAsset.AvatarImage.name;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        Utils.LogGreen(playerScripts.charAsset.AvatarImage.name);
        Utils.LogGreen($"{customProperties["PlayerImage"]}");
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("PlayerImage", out object image))
            {
                photonView.RPC("ImageChangePlayer", RpcTarget.All, player.ActorNumber, image.ToString());
            }
        }
        yield return null;
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

    /// <summary>
    /// 플레이어의 이미지를 바꾸는 함수
    /// </summary>
    /// <param name="playerID"> 액터넘버 </param>
    /// <param name="newImageName"> 플레이어가 선택한 덱의 용사 이미지</param>
    [PunRPC]
    public void ImageChangePlayer(int playerID, string newImageName)
    {
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(playerID);
        if (player == null)
        {
            Utils.LogRed("Player not found.");
            return;
        }

        string playerObjectName = $"{player.NickName}";
        GameObject playerObject = GameObject.Find(playerObjectName);
        if (playerObject == null)
        {
            Utils.LogRed("Player object not found.");
            return;
        }

        Image playerImage = playerObject.transform.Find("Player_Image").GetComponent<Image>();
        if (playerImage != null)
        {
            Sprite sprite = Resources.Load<Sprite>($"Images/{newImageName}");
            if (sprite != null)
            {
                playerImage.sprite = sprite;
            }
            else
            {
                Utils.LogRed($"Failed to load sprite: {newImageName}");
            }
        }
        else
        {
            Utils.LogRed("Player's Image component is null.");
        }
    }



    [PunRPC]
    private void UpdateHandCardCount(string plusMinus, int deckCardCount, int handCardCount, string nickName)
    {
        int GetPlayerIndexByNickName(string userNickName)
        {
            for (int i = 0; i < _Nickname.Length; i++)
            {
                if (_Nickname[i] == nickName)
                {
                    return i; // NickName이 일치하는 인덱스를 반환
                }
            }
            return -1; // NickName이 배열에 없으면 -1을 반환
        }

        int playerIndex = GetPlayerIndexByNickName(nickName);


        _baseHandCards = handCardCount;
        _baseDeckCards = deckCardCount;

        // 손패 몇 장 있는지 숫자 보여주기
        switch (plusMinus)
        {
            case "Minus":
                _playerHandCount[playerIndex].text = $"{_baseHandCards}";
                _playerDeckCount[playerIndex].text = $"{_baseDeckCards}";
                break;
            case "Plus":
                _playerHandCount[playerIndex].text = $"{_baseHandCards}";
                _playerDeckCount[playerIndex].text = $"{_baseDeckCards}";
                if (_baseDeckCards <= 0)
                {
                    _playerDeckCount[playerIndex].text = $"0";
                }
                if (_baseHandCards <= 1)
                {
                    _playerHandCount[playerIndex].text = $"0";
                }
                break;
            default:
                break;
        }
    }
}
