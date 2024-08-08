using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerListDisplay : MonoBehaviourPunCallbacks
{
    public GameObject playerImagePrefab; // 플레이어 이미지를 위한 프리팹
    private Transform playerListContent; // 플레이어 이미지를 추가할 부모 객체

    private Dictionary<int, GameObject> playerImages = new Dictionary<int, GameObject>();
    private LobbyPlayer lobbyPlayer;

    private void Awake()
    {
        playerListContent = transform.Find("Panel - BG/InRoom/Panel - PlayerImageView");
        lobbyPlayer = FindObjectOfType<LobbyPlayer>();
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Utils.Log($"{newPlayer.NickName} joined the room.");
        AddPlayerToList(newPlayer);

        // 기존 플레이어들에게 이미지 정보를 업데이트 요청
        lobbyPlayer.UpdateAllPlayersImage();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Utils.Log($"{otherPlayer.NickName} left the room.");
        RemovePlayerFromList(otherPlayer);
    }

    private void AddPlayerToList(Player player)
    {
        if (!playerImages.ContainsKey(player.ActorNumber))
        {
            GameObject playerImage = Instantiate(playerImagePrefab, playerListContent);
            playerImage.GetComponentInChildren<TMP_Text>().text = player.NickName;

            // 생성된 게임 오브젝트의 이름을 포톤 닉네임으로 설정
            playerImage.name = player.NickName;

            // 이미지 정보 동기화
            if (player.CustomProperties.TryGetValue("PlayerImage", out object image))
            {
                string imageName = image.ToString();
                Sprite sprite = Resources.Load<Sprite>($"Images/{imageName}");
                if (sprite != null)
                {
                    playerImage.GetComponentInChildren<Image>().sprite = sprite;
                }
                else
                {
                    Debug.LogError($"Failed to load sprite: {imageName}");
                }
            }

            playerImages.Add(player.ActorNumber, playerImage);
        }
    }

    private void RemovePlayerFromList(Player player)
    {
        if (playerImages.TryGetValue(player.ActorNumber, out GameObject playerImage))
        {
            if (playerImage != null)
            {
                Destroy(playerImage);
            }
            playerImages.Remove(player.ActorNumber);
        }
    }

    public void UpdatePlayerList()
    {
        // 현재 플레이어 목록을 기반으로 새로운 플레이어 이미지 추가
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddPlayerToList(player);
        }

        // 플레이어 목록에서 삭제된 플레이어 이미지 제거
        List<int> playerIds = new List<int>(playerImages.Keys);
        foreach (int playerId in playerIds)
        {
            Player player = PhotonNetwork.CurrentRoom.GetPlayer(playerId);
            if (player == null)
            {
                // 플레이어가 존재하지 않으면 리스트에서 제거
                if (playerImages.TryGetValue(playerId, out GameObject playerImage))
                {
                    if (playerImage != null)
                    {
                        Destroy(playerImage);
                    }
                    playerImages.Remove(playerId);
                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        Utils.Log("Joined the room.");
        UpdatePlayerList();

        // 기존 플레이어의 이미지 정보를 새로 업데이트
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("PlayerImage", out object image))
            {
                lobbyPlayer.photonView.RPC("ImageChangePlayer", RpcTarget.All, player.ActorNumber, image.ToString());
            }
        }
    }

    public override void OnLeftRoom()
    {
        Utils.Log("Left the room.");
        foreach (var playerImage in playerImages.Values)
        {
            Destroy(playerImage);
        }
        playerImages.Clear();
    }
}
