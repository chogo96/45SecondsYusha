using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviourPunCallbacks
{
    public Image _deckSelectImage;
    public string imageName; // 이미지를 구분하는 이름 또는 ID
    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    public override void OnJoinedRoom()
    {
        _deckSelectImage = transform.Find($"{PhotonNetwork.LocalPlayer.NickName}/Image").GetComponent<Image>();
    }


    public void SetPlayerImage(string newImageName)
    {
        // 로컬에서도 imageName 업데이트
        imageName = newImageName;

        customProperties["PlayerImage"] = imageName; // 이미지 정보도 커스텀 속성에 추가
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        // 이미지 변경 RPC 호출
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
        // 현재 방에 있는 모든 플레이어들에게 이미지 변경 요청을 보냄
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("PlayerImage", out object image))
            {
                photonView.RPC("ImageChangePlayer", RpcTarget.All, player.ActorNumber, image.ToString());
            }
        }
    }
}