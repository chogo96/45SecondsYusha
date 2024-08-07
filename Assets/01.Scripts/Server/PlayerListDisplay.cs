using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerListDisplay : MonoBehaviourPunCallbacks
{
    public GameObject playerImagePrefab; // �÷��̾� �̹����� ���� ������
    private Transform playerListContent; // �÷��̾� �̹����� �߰��� �θ� ��ü

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

        // ���� �÷��̾�鿡�� �̹��� ������ ������Ʈ ��û
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

            // ������ ���� ������Ʈ�� �̸��� ���� �г������� ����
            playerImage.name = player.NickName;

            // �̹��� ���� ����ȭ
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
        // ���� �÷��̾� ����� ������� ���ο� �÷��̾� �̹��� �߰�
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddPlayerToList(player);
        }

        // �÷��̾� ��Ͽ��� ������ �÷��̾� �̹��� ����
        List<int> playerIds = new List<int>(playerImages.Keys);
        foreach (int playerId in playerIds)
        {
            Player player = PhotonNetwork.CurrentRoom.GetPlayer(playerId);
            if (player == null)
            {
                // �÷��̾ �������� ������ ����Ʈ���� ����
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

        // ���� �÷��̾��� �̹��� ������ ���� ������Ʈ
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
