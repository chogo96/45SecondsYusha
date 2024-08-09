using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomCreator : MonoBehaviourPunCallbacks
{
    private Button _createRoomButton;
    private Button _cancelCreateRoomButton;

    private TMP_InputField _inputFieldRoomName;

    public RoomListDisplay roomListDisplay;
    public PlayerListDisplay playerListDisplay;

    private Button _newRoom;
    private GameObject _roomOptionPanel;
    private GameObject _inRoomPanel;
    private Button _roomExit;
    private Button _roomStart;
    private Button _selectDeck;

    // ���� ��Ī UI
    private GameObject _panelMatchmaking;
    private Button _matchmaking;
    private Button _cancelMatchmaking;
    private TMP_Text _matchmakingPlayer;

    // ���� ����
    private const string _matchmakingRoomType = "matchmaking";
    private const string _customRoomType = "Custom";
    private bool isMatchmaking = false;

    // Ŀ���ҹ濡�� ��Ī������ �����ϴ¹�ư
    private Button _matchmakingStart;
    private bool isChangedRoom = false;

    // PunChat ����߰�
    private GameObject _punChatPanel;
    private PunChatManager punChatManager;

    // ���ηκ� ��ư �߰�
    private Button _mainLobby;

    // ������� ��ư��Ȱ��ȭ ó���� ���ӿ�����Ʈ
    private GameObject _buttons;

    // ���� �ߴ��� ���ߴ��� Ȯ���ϴ°� ����
    private Button _roomReady;
    private bool _isRoomReady = false;
    private TMP_Text _playerNickName;
    private GameObject _notReady;
    private Button _checkButton;
    

    private void Awake()
    {

        _createRoomButton = transform.Find("Panel - BG/RoomOption/Button - RoomCreate").GetComponent<Button>();
        _cancelCreateRoomButton = transform.Find("Panel - BG/RoomOption/Button - Cancel").GetComponent<Button>();

        _inputFieldRoomName = transform.Find("Panel - BG/RoomOption/RoomName/InputField (TMP) - RoomName").GetComponent<TMP_InputField>();

        _roomOptionPanel = transform.Find("Panel - BG/RoomOption").gameObject;
        _inRoomPanel = transform.Find("Panel - BG/InRoom").gameObject;

        _roomExit = transform.Find("Panel - BG/InRoom/Button - Exit").GetComponent<Button>();
        _roomStart = transform.Find("Panel - BG/InRoom/Button - Start").GetComponent<Button>();
        _selectDeck = transform.Find("Panel - BG/InRoom/Button - DeckSelect").GetComponent<Button>();
        _roomReady = transform.Find("Panel - BG/InRoom/Button - Ready").GetComponent<Button>();

        _newRoom = transform.Find("Panel - BG/Buttons/Button - NewRoom").GetComponent<Button>();
        _matchmaking = transform.Find("Panel - BG/Buttons/Button - Matchmaking").GetComponent<Button>();
        _mainLobby = transform.Find("Panel - BG/Buttons/Button - MainLobby").GetComponent<Button>();

        _panelMatchmaking = transform.Find("Panel - BG/Panel - Matchmaking").gameObject;
        _cancelMatchmaking = transform.Find("Panel - BG/Panel - Matchmaking/Button - MatchmakingCancel").GetComponent<Button>();
        _matchmakingPlayer = transform.Find("Panel - BG/Panel - Matchmaking/Text (TMP) -  Matching").GetComponent<TMP_Text>();


        _matchmakingStart = transform.Find("Panel - BG/InRoom/Button - MatchingStart").GetComponent<Button>();

        _punChatPanel = transform.Find("Panel - BG/PunChat").gameObject;

        _buttons = transform.Find("Panel - BG/Buttons").gameObject;

        punChatManager = FindObjectOfType<PunChatManager>();

        _notReady = transform.Find("Panel - BG/Panel - PlayerIsNotReady").gameObject;
        _checkButton = transform.Find("Panel - BG/Panel - PlayerIsNotReady/Button - Check").GetComponent<Button>();

    }

    void Start()
    {
        _createRoomButton.onClick.AddListener(CreateCustomRoom);
        _cancelCreateRoomButton.onClick.AddListener(CancelCreateCustomRoom);

        _newRoom.onClick.AddListener(OnClickNewRoomButton);
        _roomExit.onClick.AddListener(OnClickRoomExit);
        _roomStart.onClick.AddListener(OnClickRoomStart);
        _selectDeck.onClick.AddListener(OnClickSelectDeck);
        _roomReady.onClick.AddListener(OnClickRoomReady);

        _matchmaking.onClick.AddListener(OnClickMatchmake);
        _mainLobby.onClick.AddListener(OnClickMainLobby);
        _cancelMatchmaking.onClick.AddListener(OnClickCancelMatchmake);


        _matchmakingStart.onClick.AddListener(UpdateFromCustomToMatching);

        _checkButton.onClick.AddListener(OnClickNotReadyCheckButton);


        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby(); // �κ� �����Ͽ� �� ����� �޾ƿɴϴ�.
        }

        _inRoomPanel.SetActive(false);
        _roomOptionPanel.SetActive(false);
        _panelMatchmaking.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;

        _punChatPanel.SetActive(false);
        _notReady.SetActive(false);

        if (PhotonNetwork.InRoom)
        {
            // �濡 ������ �÷��̾� ���� ���� �г��� ����
            _inRoomPanel.SetActive(true);
            int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
            playerListDisplay.UpdatePlayerList(); // �濡 ������ �� �÷��̾� ��� ����
            _punChatPanel.SetActive(true);
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                UpdatePlayerReadyState(PhotonNetwork.PlayerList[i].ActorNumber, true);
            }
        }
        else
        {
            Debug.LogWarning("�κ� ���� ������ ���� �濡 �� ���� �ʽ��ϴ�.");
        }
    }

    // ���� ��Ī �۾� ����
    /// <summary>
    /// ��ġ����ŷ �� ����� �Լ� ��ǻ� �ʿ������
    /// </summary>
    void CreateMatchmakingRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            string roomName = "Room_" + Random.Range(1000, 9999) + Random.Range(1000, 9999);
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "roomType", _matchmakingRoomType } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "roomType" };
            roomOptions.IsVisible = false; // ��ġ����ŷ ���� �� ��Ͽ� �� ������ ����
            PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        }
        else
        {
            Utils.LogRed("Photon is not connected!");
        }
    }

    /// <summary>
    /// ��ġ����ŷ �� �����ؼ� ���� �Լ� -> ��ġ����ŷ�� ������ ����
    /// </summary>
    public void JoinRandomRoom()
    {

        string roomName = "Room_" + Random.Range(1000, 9999) + Random.Range(1000, 9999);
        PhotonNetwork.JoinRandomOrCreateRoom(
            new ExitGames.Client.Photon.Hashtable { { "roomType", _matchmakingRoomType } }, // �� �Ӽ� ����
            0, // �� ũ��, �⺻�� 0
            MatchmakingMode.FillRoom, // ��Ī ���, �⺻�� FillRoom
            null, // Ÿ��, �⺻�� null
            null, // �κ�, �⺻�� null
            roomName, // �� �̸�
            new RoomOptions
            {
                MaxPlayers = 4,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "roomType", _matchmakingRoomType } },
                CustomRoomPropertiesForLobby = new string[] { "roomType" },
                //IsVisible = false 
            });
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Utils.Log("Failed to join a random room, creating a new room");
        CreateMatchmakingRoom();
    }

    /// <summary>
    /// ��Ī�� ��ҹ�ư �������� ������ �Լ�.
    /// </summary>
    public void OnClickCancelMatchmake()
    {
        if (isChangedRoom) //Ŀ���� �濡�� ��Ī������ ����� �濡�� ��Ҵ�����
        {
            UpdateFromMatchingToCustom();
        }
        else // ���ۺ��� ��Ī���� ����� ��� ��������.
        {
            isMatchmaking = false;
            PhotonNetwork.LeaveRoom();
            _matchmaking.gameObject.SetActive(true);
            _panelMatchmaking.SetActive(false);
        }
    }


    public override void OnLeftRoom()
    {
        if (isMatchmaking)
        {
            StartCoroutine(C_WaitForReadyAndJoinRandomRoom());
        }
        else
        {
            // ��ư Ȱ��ȭ
            _buttons.SetActive(true);
            _matchmaking.gameObject.SetActive(true);
            _punChatPanel.SetActive(false);
        }
    }

    /// <summary>
    /// ���� ����ɶ����� ��ٸ��� ����Ǹ� JoinRandomRoom �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns>����ɶ����� ��ٸ�.</returns>

    private IEnumerator C_WaitForReadyAndJoinRandomRoom()
    {
        while (!PhotonNetwork.IsConnectedAndReady || (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer && PhotonNetwork.NetworkClientState != ClientState.JoinedLobby))
        {
            Utils.Log($"Waiting for connection to Master server... IsConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}, NetworkClientState: {PhotonNetwork.NetworkClientState}");
            yield return null; // ���� �����ӱ��� ���
        }

        Utils.Log("Connected and ready. Joining random room...");
        JoinRandomRoom();
    }




    /// <summary>
    /// �÷��̾� �� ��Ī��������� �ٸ������� UI �����ϴ� ���
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        string roomType = (string)PhotonNetwork.CurrentRoom.CustomProperties["roomType"];

        if (roomType == _matchmakingRoomType)
        {
            _matchmakingPlayer.text = $"Matching ( {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers} )";
            playerListDisplay.UpdatePlayerList();
        }
    }
    // ���� ��Ī �۾� ��



    /// <summary>
    /// Ŀ���ҹ� ������ ��ġ����ŷ ������ �����ϴ� �Լ�
    /// </summary>
    public void UpdateFromCustomToMatching()
    {
        CheckAllPlayersReady(1);
    }/// <summary>
     /// ��Ī �� ������ Ŀ���� ������ �����ϴ� �Լ�
     /// </summary>
    public void UpdateFromMatchingToCustom()
    {
        // ������. ��� ��ư�� ���������� ��Ī������ ���Դٸ� ������ �� ������ ���� ����
        // �濡 ������ �������� Ȯ�� + ���常 ���� ����
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "roomType", _customRoomType }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(newProperties);
            Utils.Log("�� ������ ������Ʈ�Ǿ����ϴ�.");
            KickNonChangedRoomPlayers(); //������ �������� ���� ���� ���� �Լ�.
            isChangedRoom = false;
            _panelMatchmaking.SetActive(false);

            // �÷��̾ ó������ Ŀ���ҹ濡 �ִ� �������� üũ
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                {
                    { "isChangedRoom", false }
                };
                player.SetCustomProperties(playerProperties);
            }
        }
        else
        {
            Utils.LogRed("�濡 ������ ���°� �ƴմϴ�.");
        }
    }

    /// <summary>
    /// ������ �������� ���� ���� ���� �Լ�.
    /// </summary>
    /// <param name="isChangedRoom">boolŸ�� Ŀ���ҹ濡�� ���� �ִ� �����ΰ�?</param>
    public void KickNonChangedRoomPlayers()
    {
        //ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
        //{
        //    { "isChangedRoom", isChangedRoom }
        //};
        //PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("isChangedRoom", out object isChangedRoomObj))
            {
                bool isChangedRoom = (bool)isChangedRoomObj;
                if (!isChangedRoom)
                {
                    photonView.RPC("KickPlayer", player);
                    Utils.Log($"{player.NickName} has been kicked.");
                }
            }
            else
            {
                photonView.RPC("KickPlayer", player);
                Utils.Log($"{player.NickName} has been kicked (property not set).");
            }
        }
    }
    [PunRPC]
    private void KickPlayer()
    {
        PhotonNetwork.LeaveRoom();
    }



    /// <summary>
    /// Ŀ���� �� ����� �Լ�
    /// </summary>
    void CreateCustomRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "roomType", _customRoomType } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "roomType" };
            PhotonNetwork.CreateRoom(_inputFieldRoomName.text, roomOptions, null);
        }
        else
        {
            Utils.LogRed("Photon is not connected!");
        }
        // ��ư ��Ȱ��ȭ
        _buttons.SetActive(false);

        _roomOptionPanel.SetActive(false);
        _inRoomPanel.SetActive(true);
    }
    void CancelCreateCustomRoom()
    {
        _roomOptionPanel.SetActive(false);
    }

    public override void OnCreatedRoom()
    {
        Utils.Log("Room created successfully!");
        PhotonNetwork.JoinLobby(); // ���� ������ �� �κ� �ٽ� �����Ͽ� �� ����� �����մϴ�.
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Utils.LogRed("Room creation failed: " + message);
    }

    public override void OnJoinedRoom()
    {
        Utils.Log("Joined room successfully!");

        string roomType = (string)PhotonNetwork.CurrentRoom.CustomProperties["roomType"];

        if (roomType == _matchmakingRoomType)
        {
            playerListDisplay.UpdatePlayerList();
            _matchmakingPlayer.text = $"Matching ( {PhotonNetwork.CurrentRoom.PlayerCount} / 4 )";
            playerListDisplay.UpdatePlayerList();

            // ���� ���� �ο��� 4������ Ȯ��
            if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
            {
                isMatchmaking = false;
                _panelMatchmaking.SetActive(false);
                // �濡 ������ �÷��̾� ���� ���� �г��� ����
                _inRoomPanel.SetActive(true);
                int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
                playerListDisplay.UpdatePlayerList(); // �濡 ������ �� �÷��̾� ��� ����
                _punChatPanel.SetActive(true);
                _buttons.SetActive(false);

                _playerNickName = transform.Find($"Panel - BG/InRoom/Panel - PlayerImageView/{PhotonNetwork.LocalPlayer.NickName}/Text (TMP)").GetComponent<TMP_Text>();
                if (PhotonNetwork.IsMasterClient)
                {
                    // ������Ŭ���̾�Ʈ(����) �̶�� �г����� �Ķ�����
                    _playerNickName.color = Color.blue;

                    // �׸��� ������Ŭ���̾�Ʈ �ϱ� �ٷ� ������� �ھƹ���
                    ExitGames.Client.Photon.Hashtable customPropertiess = new ExitGames.Client.Photon.Hashtable();
                    customPropertiess["Ready"] = true;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(customPropertiess);
                }
                else
                {
                    // �ٸ��÷��̾�� �⺻���°� �غ�X �ϱ� �г����� �������� ����
                    _playerNickName.color = Color.red;
                }
                photonView.RPC("UpdatePlayerReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, _isRoomReady);
                UpdateStartButtonVisibility();
            }
        }
        else if (roomType == _customRoomType)
        {
            // �濡 ������ �÷��̾� ���� ���� �г��� ����
            _inRoomPanel.SetActive(true);
            int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
            playerListDisplay.UpdatePlayerList(); // �濡 ������ �� �÷��̾� ��� ����
            _punChatPanel.SetActive(true);
            _buttons.SetActive(false);

            _playerNickName = transform.Find($"Panel - BG/InRoom/Panel - PlayerImageView/{PhotonNetwork.LocalPlayer.NickName}/Text (TMP)").GetComponent<TMP_Text>();
            if (PhotonNetwork.IsMasterClient)
            {
                // ������Ŭ���̾�Ʈ(����) �̶�� �г����� �Ķ�����
                _playerNickName.color = Color.blue;

                // �׸��� ������Ŭ���̾�Ʈ �ϱ� �ٷ� ������� �ھƹ���
                ExitGames.Client.Photon.Hashtable customPropertiess = new ExitGames.Client.Photon.Hashtable();
                customPropertiess["Ready"] = true;
                PhotonNetwork.LocalPlayer.SetCustomProperties(customPropertiess);
            }
            else
            {
                // �ٸ��÷��̾�� �⺻���°� �غ�X �ϱ� �г����� �������� ����
                _playerNickName.color = Color.red;
            }
            photonView.RPC("UpdatePlayerReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, _isRoomReady);
            UpdateStartButtonVisibility();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string roomType = (string)PhotonNetwork.CurrentRoom.CustomProperties["roomType"];

        if (roomType == _matchmakingRoomType)
        {
            _matchmakingPlayer.text = $"Matching ( {PhotonNetwork.CurrentRoom.PlayerCount} / 4)";
            playerListDisplay.UpdatePlayerList();

            if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
            {
                isMatchmaking = false;
                _panelMatchmaking.SetActive(false);
                // �濡 ������ �÷��̾� ���� ���� �г��� ����
                _inRoomPanel.SetActive(true);
                int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
                playerListDisplay.UpdatePlayerList(); // �濡 ������ �� �÷��̾� ��� ����
                _punChatPanel.SetActive(true);
                _buttons.SetActive(false);

                _playerNickName = transform.Find($"Panel - BG/InRoom/Panel - PlayerImageView/{PhotonNetwork.LocalPlayer.NickName}/Text (TMP)").GetComponent<TMP_Text>();
                if (PhotonNetwork.IsMasterClient)
                {
                    // ������Ŭ���̾�Ʈ(����) �̶�� �г����� �Ķ�����
                    _playerNickName.color = Color.blue;

                    // �׸��� ������Ŭ���̾�Ʈ �ϱ� �ٷ� ������� �ھƹ���
                    ExitGames.Client.Photon.Hashtable customPropertiess = new ExitGames.Client.Photon.Hashtable();
                    customPropertiess["Ready"] = true;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(customPropertiess);
                }
                else
                {
                    // �ٸ��÷��̾�� �⺻���°� �غ�X �ϱ� �г����� �������� ����
                    _playerNickName.color = Color.red;
                }
                photonView.RPC("UpdatePlayerReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, _isRoomReady);
                UpdateStartButtonVisibility();
            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Utils.Log("Room list updated");
        roomListDisplay.OnRoomListUpdate(roomList);
    }

    private void OnClickNewRoomButton()
    {
        _inRoomPanel.SetActive(false);
        _roomOptionPanel.SetActive(true);
    }

    private void OnClickRoomExit()
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["Ready"] = false;
        customProperties["PlayerImage"] = "����Ʈ";
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        _inRoomPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    private void OnClickRoomStart()
    {
        
        CheckAllPlayersReady(0);

    }


    /// <summary>
    /// ��ġ����ŷ ��ư �������� ������ �Լ�.
    /// </summary>
    private void OnClickMatchmake()
    {
        _panelMatchmaking.SetActive(true);
        if (isMatchmaking == false)
        {
            isMatchmaking = true;
            _matchmaking.gameObject.SetActive(false);
            JoinRandomRoom();
        }
    }
    private void OnClickSelectDeck()
    {
        // DeckSelectionUI�� �ν��Ͻ��� ã�Ƽ� ShowScreen ȣ��
        DeckSelectionUI deckSelectionUI = FindObjectOfType<DeckSelectionUI>();
        if (deckSelectionUI != null)
        {
            deckSelectionUI.ShowScreen();
        }
        else
        {
            Utils.LogRed("DeckSelectionUI �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }

    private void OnClickMainLobby()
    {
        SceneManager.LoadScene("01.MainScene");

    }


    /// <summary>
    /// ��� ������ "Ready" ���¸� Ȯ���ϴ� �޼���
    /// </summary>
    /// <param name="num">0 = Ŀ���ҹ� / 1 = Ŀ���� -> ��Ī��</param>
    public void CheckAllPlayersReady(int num)
    {
        bool allReady = true;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("Ready", out object isReady))
            {
                if (!(bool)isReady)
                {
                    allReady = false;
                    break;
                }
            }
            else
            {
                allReady = false;
                break;
            }
            // PlayerImage�� �����Ǿ����� Ȯ��
            if (!player.CustomProperties.TryGetValue("PlayerImage", out object playerImage) || playerImage == null)
            {
                allReady = false;
                Utils.LogRed($"Player {player.NickName} has no PlayerImage set.");
                break;
            }
        }

        if (allReady)
        {
            switch (num)
            {
                case 0:
                    //if (PhotonNetwork.IsMasterClient && (PhotonNetwork.CurrentRoom.PlayerCount == 4))
                    //{
                    //    PhotonNetwork.LoadLevel("03.GamePlay Scene");
                    //}
                    //else
                    //{
                    //    Utils.LogRed($"�÷��̾� ���� 4���� �ƴմϴ�.\n���� �÷��̾� ���� {PhotonNetwork.CurrentRoom.PlayerCount} �Դϴ�.");
                    //}

                    if (PhotonNetwork.IsMasterClient)
                    {
                        // ���� �׽�Ʈ�Ҷ� ����� ����
                        PhotonNetwork.LoadLevel("03.GamePlay Scene");
                    }
                    break;
                case 1:
                    // �濡 ������ �������� Ȯ�� + ���常 ���� ����
                    if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
                    {

                        ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable
                    {
                    { "roomType", _matchmakingRoomType }
                    };
                        PhotonNetwork.CurrentRoom.SetCustomProperties(newProperties);
                        Utils.Log("�� ������ ������Ʈ�Ǿ����ϴ�.");
                        isChangedRoom = true;
                        _matchmakingPlayer.text = $"Matching ( {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers} )";
                        _panelMatchmaking.SetActive(true);

                        // �÷��̾ ó������ Ŀ���ҹ濡 �ִ� �������� üũ
                        foreach (Player player in PhotonNetwork.PlayerList)
                        {
                            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                        {
                        { "isChangedRoom", true }
                        };
                            player.SetCustomProperties(playerProperties);
                        }
                    }
                    else
                    {
                        Utils.LogRed("�濡 ������ ���°� �ƴմϴ�.");
                    }
                    break;
                default:
                    break;
            }
            Utils.Log("All players are ready!");
            // ��� �÷��̾ �غ�� ������ ���� ����
        }
        else
        {
            Utils.Log("Not all players are ready.");
            _notReady.SetActive(true);
        }
    }

    public void OnClickRoomReady()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return;
        }

        _isRoomReady = !_isRoomReady;

        ExitGames.Client.Photon.Hashtable customPropertiess = new ExitGames.Client.Photon.Hashtable();
        customPropertiess["Ready"] = _isRoomReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customPropertiess);

        // RPC ȣ��� ��� Ŭ���̾�Ʈ���� ���� ���� ����
        photonView.RPC("UpdatePlayerReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, _isRoomReady);
    }

    /// <summary>
    /// �濡���ö�, ���� ������, ���� ����Ҷ� �̸��� ���� �Լ�
    /// </summary>
    /// <param name="playerID">���</param>
    /// <param name="isReady">���� �ߴ���?</param>
    [PunRPC]
    public void UpdatePlayerReadyState(int playerID, bool isReady)
    {
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(playerID);
        if (player == null)
        {
            Utils.LogRed("Player not found.");
            return;
        }

        GameObject playerObject = GameObject.Find($"{player.NickName}");
        if (playerObject == null)
        {
            Utils.LogRed("Player object not found.");
            return;
        }

        TMP_Text playerNickName = playerObject.GetComponentInChildren<TMP_Text>();
        if (playerNickName != null)
        {
            if (player.IsMasterClient)
            {
                playerNickName.color = Color.blue; // ������ �г��� ������ �Ķ������� ����
            }
            else
            {
                playerNickName.color = isReady ? Color.green : Color.red; // �ٸ� �÷��̾��� �غ� ���¿� ���� ���� ����
            }
        }
        else
        {
            Utils.LogRed("Player's NickName component is null.");
        }

        // �濡 �ִ� ��� �÷��̾��� ���� ������Ʈ
        foreach (Player otherPlayer in PhotonNetwork.PlayerList)
        {
            GameObject otherPlayerObject = GameObject.Find($"{otherPlayer.NickName}");
            if (otherPlayerObject != null)
            {
                TMP_Text otherPlayerNickName = otherPlayerObject.GetComponentInChildren<TMP_Text>();
                if (otherPlayerNickName != null)
                {
                    if (otherPlayer.IsMasterClient)
                    {
                        otherPlayerNickName.color = Color.blue; // ������ �г��� ������ �Ķ������� ����
                    }
                    else if (otherPlayer.CustomProperties.TryGetValue("Ready", out object otherIsReady))
                    {
                        otherPlayerNickName.color = (bool)otherIsReady ? Color.green : Color.red; // �ٸ� �÷��̾��� �غ� ���¿� ���� ���� ����
                    }
                    else
                    {
                        otherPlayerNickName.color = Color.red; // �⺻������ ���������� ����
                    }
                }
                else
                {
                    Utils.LogRed("Other player's NickName component is null.");
                }
            }
            else
            {
                Utils.LogRed("Other player object not found.");
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            UpdateStartButtonVisibility();
        }
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready"))
        {
            bool isReady = (bool)changedProps["Ready"];
            UpdatePlayerReadyState(targetPlayer.ActorNumber, isReady);
        }
    }

    public void OnClickNotReadyCheckButton()
    {
        _notReady.SetActive(false);
    }


    /// <summary>
    /// ���常 ��ư ���̰� ����� �Լ�
    /// </summary>
    private void UpdateStartButtonVisibility()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            _roomStart.gameObject.SetActive(true); // �����̶�� Start ��ư Ȱ��ȭ
            _matchmakingStart.gameObject.SetActive(true); // �����̶�� Start ��ư Ȱ��ȭ
            _roomReady.gameObject.SetActive(false);  // ������ Ready ���� �ʿ䰡 ����
        }
        else
        {
            _roomStart.gameObject.SetActive(false); // ������ �ƴ϶�� Start ��ư ��Ȱ��ȭ
            _matchmakingStart.gameObject.SetActive(false); // ������ �ƴ϶�� Start ��ư ��Ȱ��ȭ
        }
    }
}

