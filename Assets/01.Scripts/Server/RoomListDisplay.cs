using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;

public class RoomListDisplay : MonoBehaviourPunCallbacks
{
    public GameObject roomListItemPrefab; // 방 목록 아이템 프리팹
    private GameObject roomListContent; // 방 목록 콘텐츠를 담는 부모 객체
    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>(); // 방 이름과 방 목록 아이템 매핑

    private const string _matchmakingRoomType = "matchmaking"; // 매치메이킹 방 타입
    private const string _customRoomType = "Custom"; // 커스텀 방 타입

    private void Awake()
    {
        // 방 목록 콘텐츠를 찾습니다
        roomListContent = transform.Find("RoomList/Scroll View - RoomList/Viewport/Content").gameObject;
    }

    // 방 목록이 업데이트될 때 호출됩니다
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated");

        // 삭제된 RoomItem 프리팹을 저장할 임시변수
        GameObject tempRoom = null;

        foreach (var roomInfo in roomList)
        {
            // 매치메이킹 방을 필터링하여 제외합니다
            if (roomInfo.CustomProperties.ContainsKey("roomType") &&
                (string)roomInfo.CustomProperties["roomType"] == _matchmakingRoomType)
            {
                continue; // 매치메이킹 방은 건너뜁니다
            }

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
