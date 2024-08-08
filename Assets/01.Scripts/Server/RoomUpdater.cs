using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using System;

public class RoomUpdater : MonoBehaviourPunCallbacks
{
    
    private string _matchmakingRoomType = "matchmaking";
    /// <summary>
    /// 커스텀방 정보를 매치메이킹 방으로 변경하는 함수
    /// </summary>
    public void UpdateRoomInfo()
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
        }
        else
        {
            Utils.LogRed("방에 입장한 상태가 아닙니다.");
        }
    }
}
