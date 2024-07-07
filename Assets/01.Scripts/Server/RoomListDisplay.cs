using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;

public class RoomListDisplay : MonoBehaviourPunCallbacks
{
    public GameObject roomListItemPrefab; // �� ��� ������ ������
    private GameObject roomListContent; // �� ��� �������� ��� �θ� ��ü
    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>(); // �� �̸��� �� ��� ������ ����

    private const string _matchmakingRoomType = "matchmaking"; // ��ġ����ŷ �� Ÿ��
    private const string _customRoomType = "Custom"; // Ŀ���� �� Ÿ��

    private void Awake()
    {
        // �� ��� �������� ã���ϴ�
        roomListContent = transform.Find("RoomList/Scroll View - RoomList/Viewport/Content").gameObject;
    }

    // �� ����� ������Ʈ�� �� ȣ��˴ϴ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated");

        // ������ RoomItem �������� ������ �ӽú���
        GameObject tempRoom = null;

        foreach (var roomInfo in roomList)
        {
            // ��ġ����ŷ ���� ���͸��Ͽ� �����մϴ�
            if (roomInfo.CustomProperties.ContainsKey("roomType") &&
                (string)roomInfo.CustomProperties["roomType"] == _matchmakingRoomType)
            {
                continue; // ��ġ����ŷ ���� �ǳʶݴϴ�
            }

            // ���� ������ ���
            if (roomInfo.RemovedFromList == true)
            {
                // ��ųʸ����� �� �̸����� �˻��� ����� RoomItem �������� ����
                if (rooms.TryGetValue(roomInfo.Name, out tempRoom))
                {
                    // RoomItem ������ ����
                    Destroy(tempRoom);
                    // ��ųʸ����� �ش� �� �̸��� �����͸� ����
                    rooms.Remove(roomInfo.Name);
                }
            }
            else // �� ������ ����� ���
            {
                // �� �̸��� ��ųʸ��� ���� ��� ���� �߰�
                if (!rooms.ContainsKey(roomInfo.Name))
                {
                    // RoomInfo �������� scrollContent ������ ����
                    GameObject roomPrefab = Instantiate(roomListItemPrefab, roomListContent.transform);
                    // �� ������ ǥ���ϱ� ���� RoomInfo ���� ����
                    roomPrefab.GetComponent<RoomData>().SetRoomInfo(roomInfo);
                    // ��ųʸ� �ڷ����� ������ �߰�
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else // �� �̸��� ��ųʸ��� �ִ� ��쿡 �� ������ ����
                {
                    if (rooms.TryGetValue(roomInfo.Name, out tempRoom))
                    {
                        tempRoom.GetComponent<RoomData>().SetRoomInfo(roomInfo);
                    }
                }
            }

            Debug.Log($"Room={roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})");
        }
    }
}
