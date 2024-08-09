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
    private PunChatManager punChatManager;

    // 메인로비 버튼 추가
    private Button _mainLobby;

    // 방생성시 버튼비활성화 처리할 게임오브젝트
    private GameObject _buttons;

    // 레디 했는지 안했는지 확인하는거 만듬
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
            PhotonNetwork.JoinLobby(); // 로비에 입장하여 방 목록을 받아옵니다.
        }

        _inRoomPanel.SetActive(false);
        _roomOptionPanel.SetActive(false);
        _panelMatchmaking.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;

        _punChatPanel.SetActive(false);
        _notReady.SetActive(false);

        if (PhotonNetwork.InRoom)
        {
            // 방에 입장한 플레이어 수에 따라 닉네임 설정
            _inRoomPanel.SetActive(true);
            int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
            playerListDisplay.UpdatePlayerList(); // 방에 입장한 후 플레이어 목록 갱신
            _punChatPanel.SetActive(true);
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                UpdatePlayerReadyState(PhotonNetwork.PlayerList[i].ActorNumber, true);
            }
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
                //IsVisible = false 
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
            _matchmaking.gameObject.SetActive(true);
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
        CheckAllPlayersReady(1);
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
    void CancelCreateCustomRoom()
    {
        _roomOptionPanel.SetActive(false);
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
            _matchmakingPlayer.text = $"Matching ( {PhotonNetwork.CurrentRoom.PlayerCount} / 4 )";
            playerListDisplay.UpdatePlayerList();

            // 현재 방의 인원이 4명인지 확인
            if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
            {
                isMatchmaking = false;
                _panelMatchmaking.SetActive(false);
                // 방에 입장한 플레이어 수에 따라 닉네임 설정
                _inRoomPanel.SetActive(true);
                int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
                playerListDisplay.UpdatePlayerList(); // 방에 입장한 후 플레이어 목록 갱신
                _punChatPanel.SetActive(true);
                _buttons.SetActive(false);

                _playerNickName = transform.Find($"Panel - BG/InRoom/Panel - PlayerImageView/{PhotonNetwork.LocalPlayer.NickName}/Text (TMP)").GetComponent<TMP_Text>();
                if (PhotonNetwork.IsMasterClient)
                {
                    // 마스터클라이언트(방장) 이라면 닉네임을 파랑으로
                    _playerNickName.color = Color.blue;

                    // 그리고 마스터클라이언트 니까 바로 레디상태 박아버림
                    ExitGames.Client.Photon.Hashtable customPropertiess = new ExitGames.Client.Photon.Hashtable();
                    customPropertiess["Ready"] = true;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(customPropertiess);
                }
                else
                {
                    // 다른플레이어는 기본상태가 준비X 니까 닉네임을 빨강으로 변경
                    _playerNickName.color = Color.red;
                }
                photonView.RPC("UpdatePlayerReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, _isRoomReady);
                UpdateStartButtonVisibility();
            }
        }
        else if (roomType == _customRoomType)
        {
            // 방에 입장한 플레이어 수에 따라 닉네임 설정
            _inRoomPanel.SetActive(true);
            int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
            playerListDisplay.UpdatePlayerList(); // 방에 입장한 후 플레이어 목록 갱신
            _punChatPanel.SetActive(true);
            _buttons.SetActive(false);

            _playerNickName = transform.Find($"Panel - BG/InRoom/Panel - PlayerImageView/{PhotonNetwork.LocalPlayer.NickName}/Text (TMP)").GetComponent<TMP_Text>();
            if (PhotonNetwork.IsMasterClient)
            {
                // 마스터클라이언트(방장) 이라면 닉네임을 파랑으로
                _playerNickName.color = Color.blue;

                // 그리고 마스터클라이언트 니까 바로 레디상태 박아버림
                ExitGames.Client.Photon.Hashtable customPropertiess = new ExitGames.Client.Photon.Hashtable();
                customPropertiess["Ready"] = true;
                PhotonNetwork.LocalPlayer.SetCustomProperties(customPropertiess);
            }
            else
            {
                // 다른플레이어는 기본상태가 준비X 니까 닉네임을 빨강으로 변경
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
                // 방에 입장한 플레이어 수에 따라 닉네임 설정
                _inRoomPanel.SetActive(true);
                int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
                playerListDisplay.UpdatePlayerList(); // 방에 입장한 후 플레이어 목록 갱신
                _punChatPanel.SetActive(true);
                _buttons.SetActive(false);

                _playerNickName = transform.Find($"Panel - BG/InRoom/Panel - PlayerImageView/{PhotonNetwork.LocalPlayer.NickName}/Text (TMP)").GetComponent<TMP_Text>();
                if (PhotonNetwork.IsMasterClient)
                {
                    // 마스터클라이언트(방장) 이라면 닉네임을 파랑으로
                    _playerNickName.color = Color.blue;

                    // 그리고 마스터클라이언트 니까 바로 레디상태 박아버림
                    ExitGames.Client.Photon.Hashtable customPropertiess = new ExitGames.Client.Photon.Hashtable();
                    customPropertiess["Ready"] = true;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(customPropertiess);
                }
                else
                {
                    // 다른플레이어는 기본상태가 준비X 니까 닉네임을 빨강으로 변경
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
        customProperties["PlayerImage"] = "디폴트";
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        _inRoomPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    private void OnClickRoomStart()
    {
        
        CheckAllPlayersReady(0);

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

    private void OnClickMainLobby()
    {
        SceneManager.LoadScene("01.MainScene");

    }


    /// <summary>
    /// 모든 유저의 "Ready" 상태를 확인하는 메서드
    /// </summary>
    /// <param name="num">0 = 커스텀방 / 1 = 커스텀 -> 매칭방</param>
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
            // PlayerImage가 설정되었는지 확인
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
                    //    Utils.LogRed($"플레이어 수가 4명이 아닙니다.\n현재 플레이어 수는 {PhotonNetwork.CurrentRoom.PlayerCount} 입니다.");
                    //}

                    if (PhotonNetwork.IsMasterClient)
                    {
                        // 서버 테스트할때 사용할 내용
                        PhotonNetwork.LoadLevel("03.GamePlay Scene");
                    }
                    break;
                case 1:
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
                    break;
                default:
                    break;
            }
            Utils.Log("All players are ready!");
            // 모든 플레이어가 준비된 상태일 때의 로직
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

        // RPC 호출로 모든 클라이언트에게 상태 변경 전파
        photonView.RPC("UpdatePlayerReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, _isRoomReady);
    }

    /// <summary>
    /// 방에들어올때, 레디 박을때, 레디 취소할때 이름색 변경 함수
    /// </summary>
    /// <param name="playerID">대상</param>
    /// <param name="isReady">레디 했는지?</param>
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
                playerNickName.color = Color.blue; // 방장의 닉네임 색상을 파란색으로 설정
            }
            else
            {
                playerNickName.color = isReady ? Color.green : Color.red; // 다른 플레이어의 준비 상태에 따라 색상 설정
            }
        }
        else
        {
            Utils.LogRed("Player's NickName component is null.");
        }

        // 방에 있는 모든 플레이어의 상태 업데이트
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
                        otherPlayerNickName.color = Color.blue; // 방장의 닉네임 색상을 파란색으로 설정
                    }
                    else if (otherPlayer.CustomProperties.TryGetValue("Ready", out object otherIsReady))
                    {
                        otherPlayerNickName.color = (bool)otherIsReady ? Color.green : Color.red; // 다른 플레이어의 준비 상태에 따라 색상 설정
                    }
                    else
                    {
                        otherPlayerNickName.color = Color.red; // 기본적으로 빨간색으로 설정
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
    /// 방장만 버튼 보이게 만드는 함수
    /// </summary>
    private void UpdateStartButtonVisibility()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            _roomStart.gameObject.SetActive(true); // 방장이라면 Start 버튼 활성화
            _matchmakingStart.gameObject.SetActive(true); // 방장이라면 Start 버튼 활성화
            _roomReady.gameObject.SetActive(false);  // 방장은 Ready 누를 필요가 없음
        }
        else
        {
            _roomStart.gameObject.SetActive(false); // 방장이 아니라면 Start 버튼 비활성화
            _matchmakingStart.gameObject.SetActive(false); // 방장이 아니라면 Start 버튼 비활성화
        }
    }
}

