using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviourPunCallbacks
{
    public Image _deckSelectImage;
    public string imageName; // �̹����� �����ϴ� �̸� �Ǵ� ID
    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    public override void OnJoinedRoom()
    {
        _deckSelectImage = transform.Find($"{PhotonNetwork.LocalPlayer.NickName}/Image").GetComponent<Image>();
    }


    public void SetPlayerImage(string newImageName)
    {
        // ���ÿ����� imageName ������Ʈ
        imageName = newImageName;

        customProperties["PlayerImage"] = imageName; // �̹��� ������ Ŀ���� �Ӽ��� �߰�
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        // �̹��� ���� RPC ȣ��
        photonView.RPC("ImageChangePlayer", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, newImageName);
    }

    [PunRPC]
    public void ImageChangePlayer(int playerID, string newImageName)
    {
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(playerID);
        if (player == null)
        {
            Debug.LogError("Player not found.");
            return;
        }

        GameObject playerObject = GameObject.Find($"{player.NickName}");
        if (playerObject == null)
        {
            Debug.LogError("Player object not found.");
            return;
        }

        Image playerImage = playerObject.transform.Find("Image").GetComponent<Image>();
        if (playerImage != null)
        {
            Sprite sprite = Resources.Load<Sprite>($"Images/{newImageName}");
            if (sprite != null)
            {
                playerImage.sprite = sprite;
            }
            else
            {
                Debug.LogError($"Failed to load sprite: {newImageName}");
            }
        }
        else
        {
            Debug.LogError("Player's Image component is null.");
        }
    }

    public void UpdateAllPlayersImage()
    {
        // ���� �濡 �ִ� ��� �÷��̾�鿡�� �̹��� ���� ��û�� ����
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("PlayerImage", out object image))
            {
                photonView.RPC("ImageChangePlayer", RpcTarget.All, player.ActorNumber, image.ToString());
            }
        }
    }
}