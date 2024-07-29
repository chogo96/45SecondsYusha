using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomCreator : MonoBehaviourPunCallbacks
{
    private Button _createRoomButton;
    private Button[] _maxPlayer;

    private TMP_InputField _inputFieldRoomName;
    private TMP_Text _textRoomName;

    public RoomListDisplay roomListDisplay;
    public PlayerListDisplay playerListDisplay;

    private Button _newRoom;
    private Button _reSearch;
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

    private void Awake()
    {
        _maxPlayer = new Button[4];
        // ��ư ã�� �� �迭�� �߰�
        for (int i = 0; i < _maxPlayer.Length; i++)
        {
            string buttonName = $"RoomOption/MaxPlayer/MaxPlayer_Button/Button - {i + 1}";
            _maxPlayer[i] = transform.Find(buttonName).GetComponent<Button>();
            int index = i;
            _maxPlayer[i].onClick.AddListener(() => OnClickButton(index));
        }

        _createRoomButton = transform.Find("RoomOption/Button - RoomCreate").GetComponent<Button>();
        _inputFieldRoomName = transform.Find("RoomOption/RoomName/InputField (TMP) - RoomName").GetComponent<TMP_InputField>();
        _textRoomName = transform.Find("RoomOption/MaxPlayer/Text (TMP) - SelectPlayer").GetComponent<TMP_Text>();

        _roomOptionPanel = transform.Find("RoomOption").gameObject;
        _inRoomPanel = transform.Find("InRoom").gameObject;

        _roomExit = transform.Find("InRoom/Button - Exit").GetComponent<Button>();
        _roomStart = transform.Find("InRoom/Button - Start").GetComponent<Button>();
        _selectDeck = transform.Find("InRoom/Button - DeckSelect").GetComponent<Button>();

        _newRoom = transform.Find("Buttons/Button - NewRoom").GetComponent<Button>();
        _reSearch = transform.Find("Buttons/Button - ReSearch").GetComponent<Button>();
        _matchmaking = transform.Find("Buttons/Button - Matchmaking").GetComponent<Button>();

        _panelMatchmaking = transform.Find("Panel - Matchmaking").gameObject;
        _cancelMatchmaking = transform.Find("Panel - Matchmaking/Button - MatchmakingCancel").GetComponent<Button>();
        _matchmakingPlayer = transform.Find("Panel - Matchmaking/Text (TMP) -  Matching").GetComponent<TMP_Text>();


        _matchmakingStart = transform.Find("InRoom/Button - MatchingStart").GetComponent<Button>();

        _punChatPanel = transform.Find("PunChat").gameObject;
    }

    void Start()
    {
        _createRoomButton.onClick.AddListener(CreateCustomRoom);
        _newRoom.onClick.AddListener(OnClickNewRoomButton);
        _roomExit.onClick.AddListener(OnClickRoomExit);
        _roomStart.onClick.AddListener(OnClickRoomStart);
        _selectDeck.onClick.AddListener(OnClickSelectDeck);

        _matchmaking.onClick.AddListener(OnClickMatchmake);
        _cancelMatchmaking.onClick.AddListener(OnClickCancelMatchmake);


        _matchmakingStart.onClick.AddListener(UpdateFromCustomToMatching);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby(); // �κ� �����Ͽ� �� ����� �޾ƿɴϴ�.
        }

        _inRoomPanel.SetActive(false);
        _roomOptionPanel.SetActive(false);
        _panelMatchmaking.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;

        _punChatPanel.SetActive(false);
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
            Debug.LogError("Photon is not connected!");
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
                //IsVisible = false // ~~��ġ����ŷ ���� �� ��Ͽ� �� ������ ����~~  X�� �̰Ŷ����� ��� ��Ī�ȵǴ°ſ��� ����� ������ ��Ȯ�� �˾ƾ� �մϴ�.
            });
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a random room, creating a new room");
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
            Debug.Log($"Waiting for connection to Master server... IsConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}, NetworkClientState: {PhotonNetwork.NetworkClientState}");
            yield return null; // ���� �����ӱ��� ���
        }

        Debug.Log("Connected and ready. Joining random room...");
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
        // �濡 ������ �������� Ȯ�� + ���常 ���� ����
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {

            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "roomType", _matchmakingRoomType }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(newProperties);
            Debug.Log("�� ������ ������Ʈ�Ǿ����ϴ�.");
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
            Debug.LogError("�濡 ������ ���°� �ƴմϴ�.");
        }
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
            Debug.Log("�� ������ ������Ʈ�Ǿ����ϴ�.");
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
            Debug.LogError("�濡 ������ ���°� �ƴմϴ�.");
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
                    Debug.Log($"{player.NickName} has been kicked.");
                }
            }
            else
            {
                photonView.RPC("KickPlayer", player);
                Debug.Log($"{player.NickName} has been kicked (property not set).");
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
        if (int.Parse(_textRoomName.text) >= 5)
        {
            Debug.LogError("Max players should be less than 5");
        }
        else if (PhotonNetwork.IsConnected)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = byte.Parse(_textRoomName.text);
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "roomType", _customRoomType } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "roomType" };
            PhotonNetwork.CreateRoom(_inputFieldRoomName.text, roomOptions, null);
        }
        else
        {
            Debug.LogError("Photon is not connected!");
        }
        _roomOptionPanel.SetActive(false);
        _inRoomPanel.SetActive(true);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully!");
        PhotonNetwork.JoinLobby(); // ���� ������ �� �κ� �ٽ� �����Ͽ� �� ����� �����մϴ�.
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed: " + message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully!");

        string roomType = (string)PhotonNetwork.CurrentRoom.CustomProperties["roomType"];

        if (roomType == _matchmakingRoomType)
        {
            playerListDisplay.UpdatePlayerList();
            _matchmakingPlayer.text = $"Matching ( {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers} )";
            int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
            playerListDisplay.UpdatePlayerList();

            // ���� ���� �ο��� 4������ Ȯ��
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                StartGame();
            }
        }
        else if (roomType == _customRoomType)
        {
            // �濡 ������ �÷��̾� ���� ���� �г��� ����
            _inRoomPanel.SetActive(true);
            int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
            playerListDisplay.UpdatePlayerList(); // �濡 ������ �� �÷��̾� ��� ����
            _punChatPanel.SetActive(true);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string roomType = (string)PhotonNetwork.CurrentRoom.CustomProperties["roomType"];

        if (roomType == _matchmakingRoomType)
        {
            _matchmakingPlayer.text = $"Matching ( {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers} )";
            playerListDisplay.UpdatePlayerList();

            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                StartGame();
            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated");
        roomListDisplay.OnRoomListUpdate(roomList);
    }

    private void OnClickButton(int index)
    {
        switch (index)
        {
            case 0:
                _textRoomName.text = "1";
                break;
            case 1:
                _textRoomName.text = "2";
                break;
            case 2:
                _textRoomName.text = "3";
                break;
            case 3:
                _textRoomName.text = "4";
                break;
        }
    }

    private void OnClickNewRoomButton()
    {
        _inRoomPanel.SetActive(false);
        _roomOptionPanel.SetActive(true);
    }

    private void OnClickRoomExit()
    {
        _inRoomPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    private void OnClickRoomStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("05.GamePlay Scene");
        }
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
            Debug.LogError("DeckSelectionUI �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }

    private void StartGame()
    {
        Debug.Log("All players joined. Starting the game...");
        isMatchmaking = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("05.GamePlay Scene");
    }
}
