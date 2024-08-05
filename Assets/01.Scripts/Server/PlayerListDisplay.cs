using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerListDisplay : MonoBehaviourPunCallbacks
{
    public GameObject playerImagePrefab; // 플레이어 이미지를 위한 프리팹
    private Transform playerListContent; // 플레이어 이미지를 추가할 부모 객체

    private Dictionary<int, GameObject> playerImages = new Dictionary<int, GameObject>();

    private void Awake()
    {
        playerListContent = transform.Find("Panel - BG/InRoom/Panel - PlayerImageView");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Utils.Log($"{newPlayer.NickName} joined the room.");
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Utils.Log($"{otherPlayer.NickName} left the room.");
        UpdatePlayerList();
    }

    public void UpdatePlayerList()
    {
        foreach (var playerImage in playerImages.Values)
        {
            Destroy(playerImage);
        }
        playerImages.Clear();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerImage = Instantiate(playerImagePrefab, playerListContent);
            playerImage.GetComponentInChildren<TMP_Text>().text = player.NickName;
            playerImages.Add(player.ActorNumber, playerImage);
        }
    }
}
