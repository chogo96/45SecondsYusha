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
        PhotonNetwork.JoinLobby(); // �κ� �����մϴ�.
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon: " + cause.ToString());
    }
}