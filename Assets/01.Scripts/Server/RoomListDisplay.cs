using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;

public class RoomListDisplay : MonoBehaviourPunCallbacks
{
    public GameObject roomListItemPrefab;
    private GameObject roomListContent;
    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();

    private void Awake()
    {
        roomListContent = transform.Find("RoomList/Scroll View - RoomList/Viewport/Content").gameObject;
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated");

        // 삭제된 RoomItem 프리팹을 저장할 임시변수
        GameObject tempRoom = null;

        foreach (var roomInfo in roomList)
        {
            // 룸이 삭제된 경우
            if (roomInfo.RemovedFromList == true)
            {
                // 딕셔너리에서 룸 이름으로 검색해 저장된 RoomItem 프리팹을 추출
                if (rooms.TryGetValue(roomInfo.Name, out tempRoom))
                {
                    // RoomItem 프리팹 삭제
                    Destroy(tempRoom);
                    // 딕셔너리에서 해당 룸 이름의 데이터를 삭제
                    rooms.Remove(roomInfo.Name);
                }
            }
            else // 룸 정보가 변경된 경우
            {
                // 룸 이름이 딕셔너리에 없는 경우 새로 추가
                if (!rooms.ContainsKey(roomInfo.Name))
                {
                    // RoomInfo 프리팹을 scrollContent 하위에 생성
                    GameObject roomPrefab = Instantiate(roomListItemPrefab, roomListContent.transform);
                    // 룸 정보를 표시하기 위해 RoomInfo 정보 전달
                    roomPrefab.GetComponent<RoomData>().SetRoomInfo(roomInfo);
                    // 딕셔너리 자료형에 데이터 추가
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else // 룸 이름이 딕셔너리에 있는 경우에 룸 정보를 갱신
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
