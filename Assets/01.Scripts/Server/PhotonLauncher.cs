using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    private static PhotonLauncher instance;



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
            Utils.Log("Connecting to Photon...");
        }
    }

    public override void OnConnectedToMaster()
    {
        Utils.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby(); // 로비에 접속합니다.
    }

    public override void OnJoinedLobby()
    {
        Utils.Log("Joined Lobby");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Utils.LogRed("Disconnected from Photon: " + cause.ToString());
    }
}