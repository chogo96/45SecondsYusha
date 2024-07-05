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
    private Button _maxPlayer1;
    private Button _maxPlayer2;
    private Button _maxPlayer3;
    private Button _maxPlayer4;
    private TMP_InputField _inputFieldRoomName;
    private TMP_Text _textRoomName;

    public RoomListDisplay roomListDisplay;
    public PlayerListDisplay playerListDisplay;

    private Button _newRoom;
    private Button _reSearch;
    private GameObject _roomOptionPanal;
    private GameObject _inRoomPanal;
    private Button _roomExit;
    private Button _roomStart;

    private void Awake()
    {
        _createRoomButton = transform.Find("RoomOption/Button - RoomCreate").GetComponent<Button>();
        _maxPlayer1 = transform.Find("RoomOption/MaxPlayer/MaxPlayer_Button/Button - 1").GetComponent<Button>();
        _maxPlayer2 = transform.Find("RoomOption/MaxPlayer/MaxPlayer_Button/Button - 2").GetComponent<Button>();
        _maxPlayer3 = transform.Find("RoomOption/MaxPlayer/MaxPlayer_Button/Button - 3").GetComponent<Button>();
        _maxPlayer4 = transform.Find("RoomOption/MaxPlayer/MaxPlayer_Button/Button - 4").GetComponent<Button>();
        _inputFieldRoomName = transform.Find("RoomOption/RoomName/InputField (TMP) - RoomName").GetComponent<TMP_InputField>();
        _textRoomName = transform.Find("RoomOption/MaxPlayer/Text (TMP) - SelectPlayer").GetComponent<TMP_Text>();
        _newRoom = transform.Find("Buttons/Button - NewRoom").GetComponent<Button>();
        _reSearch = transform.Find("Buttons/Button - ReSearch").GetComponent<Button>();
        _roomOptionPanal = transform.Find("RoomOption").gameObject;
        _inRoomPanal = transform.Find("InRoom").gameObject;
        _roomExit = transform.Find("InRoom/Button - Exit").GetComponent<Button>();
        _roomStart = transform.Find("InRoom/Button - Start").GetComponent<Button>();

    }

    void Start()
    {
        _createRoomButton.onClick.AddListener(CreateRoom);
        _maxPlayer1.onClick.AddListener(OnClickButton1);
        _maxPlayer2.onClick.AddListener(OnClickButton2);
        _maxPlayer3.onClick.AddListener(OnClickButton3);
        _maxPlayer4.onClick.AddListener(OnClickButton4);
        _newRoom.onClick.AddListener(OnClickNewRoomButton);
        //_reSearch.onClick.AddListener(OnClickReSearch);
        _roomExit.onClick.AddListener(OnClickRoomExit);
        _roomStart.onClick.AddListener(OnClickRoomStart);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby(); // 로비에 입장하여 방 목록을 받아옵니다.
        }

        _inRoomPanal.SetActive(false);
        _roomOptionPanal.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void CreateRoom()
    {
        if (int.Parse(_textRoomName.text) >= 5)
        {
            Debug.LogError("Max players should be less than 5");
        }
        else if (PhotonNetwork.IsConnected)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = byte.Parse(_textRoomName.text);
            PhotonNetwork.CreateRoom(_inputFieldRoomName.text, roomOptions, null);
        }
        else
        {
            Debug.LogError("Photon is not connected!");
        }
        _roomOptionPanal.SetActive(false);
        _inRoomPanal.SetActive(true);
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
        // 방에 입장한 플레이어 수에 따라 닉네임 설정
        _inRoomPanal.SetActive(true);
        int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
        PhotonNetwork.NickName = $"Player {playerNumber}";
        playerListDisplay.UpdatePlayerList(); // 방에 입장한 후 플레이어 목록 갱신
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated");
        // 방 목록이 변경될 때 RoomListDisplay 갱신
        roomListDisplay.OnRoomListUpdate(roomList);
    }

    private void OnClickButton1()
    {
        _textRoomName.text = "1";
    }
    private void OnClickButton2()
    {
        _textRoomName.text = "2";
    }
    private void OnClickButton3()
    {
        _textRoomName.text = "3";
    }
    private void OnClickButton4()
    {
        _textRoomName.text = "4";
    }
    private void OnClickNewRoomButton()
    {
        _inRoomPanal.SetActive(false);
        _roomOptionPanal.SetActive(true);
    }
    //private void OnClickReSearch()
    //{
    //    if (PhotonNetwork.IsConnectedAndReady)
    //    {
    //        PhotonNetwork.GetRoomList// 로비에 다시 입장하여 방 목록 갱신
    //    }
    //    else
    //    {
    //        Debug.Log("Photon is not connected!");
    //    }
    //}
    private void OnClickRoomExit()
    {
        _inRoomPanal.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }
    private void OnClickRoomStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("05.GamePlay Scene");
        }
    }
}
