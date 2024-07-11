using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    // 랜덤 매칭 UI
    private GameObject _panelMatchmaking;
    private Button _matchmaking;
    private Button _cancelMatchmaking;
    private TMP_Text _matchmakingPlayer;

    // 입장 조건
    private const string _matchmakingRoomType = "matchmaking";
    private const string _customRoomType = "Custom";
    private bool isMatchmaking = false;

    private void Awake()
    {
        _maxPlayer = new Button[4];
        // 버튼 찾기 및 배열에 추가
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

        _newRoom = transform.Find("Buttons/Button - NewRoom").GetComponent<Button>();
        _reSearch = transform.Find("Buttons/Button - ReSearch").GetComponent<Button>();
        _matchmaking = transform.Find("Buttons/Button - Matchmaking").GetComponent<Button>();

        _panelMatchmaking = transform.Find("Panel - Matchmaking").gameObject;
        _cancelMatchmaking = transform.Find("Panel - Matchmaking/Button - MatchmakingCancel").GetComponent<Button>();
        _matchmakingPlayer = transform.Find("Panel - Matchmaking/Text (TMP) -  Matching").GetComponent<TMP_Text>();
    }

    void Start()
    {
        _createRoomButton.onClick.AddListener(CreateCustomRoom);
        _newRoom.onClick.AddListener(OnClickNewRoomButton);
        _roomExit.onClick.AddListener(OnClickRoomExit);
        _roomStart.onClick.AddListener(OnClickRoomStart);

        _matchmaking.onClick.AddListener(OnClickMatchmake);
        _cancelMatchmaking.onClick.AddListener(OnClickCancelMatchmake);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby(); // 로비에 입장하여 방 목록을 받아옵니다.
        }

        _inRoomPanel.SetActive(false);
        _roomOptionPanel.SetActive(false);
        _panelMatchmaking.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // 랜덤 매칭 작업 시작
    /// <summary>
    /// 매치메이킹 방 만들기 함수 사실살 필요없긴함
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
            roomOptions.IsVisible = false; // 매치메이킹 방은 방 목록에 안 나오게 설정
            PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        }
        else
        {
            Debug.LogError("Photon is not connected!");
        }
    }

    /// <summary>
    /// 매치메이킹 방 입장해서 들어가는 함수 -> 매치매이킹방 없으면 생성
    /// </summary>
    public void JoinRandomRoom()
    {
        string roomName = "Room_" + Random.Range(1000, 9999) + Random.Range(1000, 9999);
        PhotonNetwork.JoinRandomOrCreateRoom(
            new ExitGames.Client.Photon.Hashtable { { "roomType", _matchmakingRoomType } }, // 방 속성 필터
            0, // 방 크기, 기본값 0
            MatchmakingMode.FillRoom, // 매칭 모드, 기본값 FillRoom
            null, // 타입, 기본값 null
            null, // 로비, 기본값 null
            roomName, // 룸 이름
            new RoomOptions
            {
                MaxPlayers = 4,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "roomType", _matchmakingRoomType } },
                CustomRoomPropertiesForLobby = new string[] { "roomType" },
                //IsVisible = false // ~~매치메이킹 방은 방 목록에 안 나오게 설정~~  X발 이거때문에 계속 매칭안되는거였네 사람은 정보를 정확히 알아야 합니다.
            });
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a random room, creating a new room");
        CreateMatchmakingRoom();
    }

    private void OnClickCancelMatchmake()
    {
        if (isMatchmaking)
        {
            isMatchmaking = false;
            PhotonNetwork.LeaveRoom();
            _matchmaking.gameObject.SetActive(true);
            _panelMatchmaking.SetActive(false);
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string roomType = (string)PhotonNetwork.CurrentRoom.CustomProperties["roomType"];

        if (roomType == _matchmakingRoomType)
        {
            _matchmakingPlayer.text = $"Matching ( {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers} )";
            playerListDisplay.UpdatePlayerList();
        }
    }
    // 랜덤 매칭 작업 끝


    /// <summary>
    /// 커스텀 방 만들기 함수
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
        PhotonNetwork.JoinLobby(); // 방을 생성한 후 로비에 다시 입장하여 방 목록을 갱신합니다.
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
            PhotonNetwork.NickName = $"Player {playerNumber}";
            playerListDisplay.UpdatePlayerList();

            // 현재 방의 인원이 4명인지 확인
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                StartGame();
            }
        }
        else if (roomType == _customRoomType)
        {
            // 방에 입장한 플레이어 수에 따라 닉네임 설정
            _inRoomPanel.SetActive(true);
            int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
            playerListDisplay.UpdatePlayerList();
            PhotonNetwork.NickName = $"Player {playerNumber}";
            playerListDisplay.UpdatePlayerList(); // 방에 입장한 후 플레이어 목록 갱신
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

    private void StartGame()
    {
        Debug.Log("All players joined. Starting the game...");
        isMatchmaking = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("05.GamePlay Scene");
    }
}
