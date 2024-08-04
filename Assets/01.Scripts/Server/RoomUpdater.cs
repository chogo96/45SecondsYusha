using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using System;

public class RoomUpdater : MonoBehaviourPunCallbacks
{
    
    private string _matchmakingRoomType = "matchmaking";
    /// <summary>
    /// Ŀ���ҹ� ������ ��ġ����ŷ ������ �����ϴ� �Լ�
    /// </summary>
    public void UpdateRoomInfo()
    {
        // �濡 ������ �������� Ȯ�� + ���常 ���� ����
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "roomType", _matchmakingRoomType }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(newProperties);
            Utils.Log("�� ������ ������Ʈ�Ǿ����ϴ�.");
        }
        else
        {
            Utils.LogRed("�濡 ������ ���°� �ƴմϴ�.");
        }
    }
}
