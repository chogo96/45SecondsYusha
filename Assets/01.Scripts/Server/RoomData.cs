using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{
    private TMP_Text roomInfoText;
    private string roomName;

    void Awake()
    {
        // TMP_Text ������Ʈ�� �ڽ� ��ü���� ã��
        roomInfoText = GetComponentInChildren<TMP_Text>();
    }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        roomName = roomInfo.Name;
        roomInfoText.text = $"{roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => JoinRoom());
    }

    void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.LogError("Photon is not connected!");
        }
    }
}
