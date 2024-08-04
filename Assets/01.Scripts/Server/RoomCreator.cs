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

    private TMP_InputField _inputFieldRoomName;

    public RoomListDisplay roomListDisplay;
    public PlayerListDisplay playerListDisplay;

    private Button _newRoom;
    private GameObject _roomOptionPanel;
    private GameObject _inRoomPanel;
    private Button _roomExit;
    private Button _roomStart;
    private Button _selectDeck;

    // 랜덤 매칭 UI
    private GameObject _panelMatchmaking;
    private Button _matchmaking;
    private Button _cancelMatchmaking;
    private TMP_Text _matchmakingPlayer;

    // 입장 조건
    private const string _matchmakingRoomType = "matchmaking";
    private const string _customRoomType = "Custom";
    private bool isMatchmaking = false;

    // 커스텀방에서 매칭방으로 변경하는버튼
    private Button _matchmakingStart;
    private bool isChangedRoom = false;

    // PunChat 기능추가
    private GameObject _punChatPanel;

    // 메인로비 버튼 추가
    private Button _mainLobby;

    // 방생성시 버튼비활성화 처리할 게임오브젝트
    private GameObject _buttons;


    private void Awake()
    {

        _createRoomButton = transform.Find("Panel - BG/RoomOption/Button - RoomCreate").GetComponent<Button>();
        _inputFieldRoomName = transform.Find("Panel - BG/RoomOption/RoomName/InputField (TMP) - RoomName").GetComponent<TMP_InputField>();

        _roomOptionPanel = transform.Find("Panel - BG/RoomOption").gameObject;
        _inRoomPanel = transform.Find("Panel - BG/InRoom").gameObject;

        _roomExit = transform.Find("Panel - BG/InRoom/Button - Exit").GetComponent<Button>();
        _roomStart = transform.Find("Panel - BG/InRoom/Button - Start").GetComponent<Button>();
        _selectDeck = transform.Find("Panel - BG/InRoom/Button - DeckSelect").GetComponent<Button>();

        _newRoom = transform.Find("Panel - BG/Buttons/Button - NewRoom").GetComponent<Button>();
        _matchmaking = transform.Find("Panel - BG/Buttons/Button - Matchmaking").GetComponent<Button>();
        _mainLobby = transform.Find("Panel - BG/Buttons/Button - MainLobby").GetComponent<Button>();

        _panelMatchmaking = transform.Find("Panel - BG/Panel - Matchmaking").gameObject;
        _cancelMatchmaking = transform.Find("Panel - BG/Panel - Matchmaking/Button - MatchmakingCancel").GetComponent<Button>();
        _matchmakingPlayer = transform.Find("Panel - BG/Panel - Matchmaking/Text (TMP) -  Matching").GetComponent<TMP_Text>();


        _matchmakingStart = transform.Find("Panel - BG/InRoom/Button - MatchingStart").GetComponent<Button>();

        _punChatPanel = transform.Find("Panel - BG/PunChat").gameObject;

        _buttons = transform.Find("Panel - BG/Buttons").gameObject;

    }

    void Start()
    {
        _createRoomButton.onClick.AddListener(CreateCustomRoom);
        _newRoom.onClick.AddListener(OnClickNewRoomButton);
        _roomExit.onClick.AddListener(OnClickRoomExit);
        _roomStart.onClick.AddListener(OnClickRoomStart);
        _selectDeck.onClick.AddListener(OnClickSelectDeck);

        _matchmaking.onClick.AddListener(OnClickMatchmake);
        _mainLobby.onClick.AddListener(OnClickMainLobby);
        _cancelMatchmaking.onClick.AddListener(OnClickCancelMatchmake);


        _matchmakingStart.onClick.AddListener(UpdateFromCustomToMatching);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby(); // 로비에 입장하여 방 목록을 받아옵니다.
        }

        _inRoomPanel.SetActive(false);
        _roomOptionPanel.SetActive(false);
        _panelMatchmaking.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;

        _punChatPanel.SetActive(false);

        if (PhotonNetwork.InRoom)
        {
            // 방에 입장한 플레이어 수에 따라 닉네임 설정
            _inRoomPanel.SetActive(true);
            int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
            playerListDisplay.UpdatePlayerList(); // 방에 입장한 후 플레이어 목록 갱신
            _punChatPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("로비 씬에 있지만 현재 방에 들어가 있지 않습니다.");
        }
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
            Utils.LogRed("Photon is not connected!");
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
        Utils.Log("Failed to join a random room, creating a new room");
        CreateMatchmakingRoom();
    }

    /// <summary>
    /// 매칭중 취소버튼 눌렀을때 실행할 함수.
    /// </summary>
    public void OnClickCancelMatchmake()
    {
        if (isChangedRoom) //커스텀 방에서 배칭방으로 변경된 방에서 취소누를떄
        {
            UpdateFromMatchingToCustom();
        }
        else // 시작부터 매칭누른 사람이 취소 눌렀을때.
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
            // 버튼 활성화
            _buttons.SetActive(true);
            
            _punChatPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 서버 연결될때까지 기다리고 연결되면 JoinRandomRoom 실행하는 코루틴
    /// </summary>
    /// <returns>연결될때까지 기다림.</returns>

    private IEnumerator C_WaitForReadyAndJoinRandomRoom()
    {
        while (!PhotonNetwork.IsConnectedAndReady || (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer && PhotonNetwork.NetworkClientState != ClientState.JoinedLobby))
        {
            Utils.Log($"Waiting for connection to Master server... IsConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}, NetworkClientState: {PhotonNetwork.NetworkClientState}");
            yield return null; // 다음 프레임까지 대기
        }

        Utils.Log("Connected and ready. Joining random room...");
        JoinRandomRoom();
    }




    /// <summary>
    /// 플레이어 가 매칭취소했을때 다른유저들 UI 수정하는 기능
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
    // 랜덤 매칭 작업 끝



    /// <summary>
    /// 커스텀방 정보를 매치메이킹 방으로 변경하는 함수
    /// </summary>
    public void UpdateFromCustomToMatching()
    {
        // 방에 입장한 상태인지 확인 + 방장만 변경 가능
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {

            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "roomType", _matchmakingRoomType }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(newProperties);
            Utils.Log("방 정보가 업데이트되었습니다.");
            isChangedRoom = true;
            _matchmakingPlayer.text = $"Matching ( {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers} )";
            _panelMatchmaking.SetActive(true);

            // 플레이어가 처음부터 커스텀방에 있던 유저인지 체크
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
            Utils.LogRed("방에 입장한 상태가 아닙니다.");
        }
    }/// <summary>
     /// 매칭 방 정보를 커스텀 방으로 변경하는 함수
     /// </summary>
    public void UpdateFromMatchingToCustom()
    {
        // 문제점. 취소 버튼을 누르기전에 매칭유저가 들어왔다면 방으로 그 유저랑 같이 들어옴
        // 방에 입장한 상태인지 확인 + 방장만 변경 가능
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "roomType", _customRoomType }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(newProperties);
            Utils.Log("방 정보가 업데이트되었습니다.");
            KickNonChangedRoomPlayers(); //같은방 유저였던 유저 제외 강퇴 함수.
            isChangedRoom = false;
            _panelMatchmaking.SetActive(false);

            // 플레이어가 처음부터 커스텀방에 있던 유저인지 체크
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
            Utils.LogRed("방에 입장한 상태가 아닙니다.");
        }
    }

    /// <summary>
    /// 같은방 유저였던 유저 제외 강퇴 함수.
    /// </summary>
    /// <param name="isChangedRoom">bool타입 커스텀방에서 부터 있던 유저인가?</param>
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
    /// 커스텀 방 만들기 함수
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
        // 버튼 비활성화
        _buttons.SetActive(false);

        _roomOptionPanel.SetActive(false);
        _inRoomPanel.SetActive(true);
    }

    public override void OnCreatedRoom()
    {
        Utils.Log("Room created successfully!");
        PhotonNetwork.JoinLobby(); // 방을 생성한 후 로비에 다시 입장하여 방 목록을 갱신합니다.
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
            _matchmakingPlayer.text = $"Matching ( {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers} )";
            int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
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
            playerListDisplay.UpdatePlayerList(); // 방에 입장한 후 플레이어 목록 갱신
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
        _inRoomPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    private void OnClickRoomStart()
    {
        //if (PhotonNetwork.IsMasterClient && (PhotonNetwork.CurrentRoom.PlayerCount == 4))
        //{
        //    PhotonNetwork.LoadLevel("03.GamePlay Scene");
        //}
        //else
        //{
        //    Utils.LogRed($"플레이어 수가 4명이 아닙니다.\n현재 플레이어 수는 {PhotonNetwork.CurrentRoom.PlayerCount} 입니다.");
        //}
        
        if (PhotonNetwork.IsMasterClient)
        {
            // 서버 테스트할때 사용할 내용
            PhotonNetwork.LoadLevel("03.GamePlay Scene");
        }
        
    }


    /// <summary>
    /// 매치메이킹 버튼 눌렀을때 실행할 함수.
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
        // DeckSelectionUI의 인스턴스를 찾아서 ShowScreen 호출
        DeckSelectionUI deckSelectionUI = FindObjectOfType<DeckSelectionUI>();
        if (deckSelectionUI != null)
        {
            deckSelectionUI.ShowScreen();
        }
        else
        {
            Utils.LogRed("DeckSelectionUI 인스턴스를 찾을 수 없습니다.");
        }
    }

    private void StartGame()
    {
        Utils.Log("All players joined. Starting the game...");
        isMatchmaking = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("03.GamePlay Scene");
    }

    private void OnClickMainLobby()
    {
        SceneManager.LoadScene("01.MainScene");

    }
}
