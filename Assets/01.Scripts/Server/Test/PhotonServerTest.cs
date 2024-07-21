using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonServerTest : MonoBehaviourPunCallbacks
{
    private static PhotonServerTest instance;
    private const string _matchmakingRoomType = "matchmaking";


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ConnectToPhoton();
    }

    void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting to Photon...");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby(); // 로비에 접속합니다.
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon: " + cause.ToString());
    }
}