using Photon.Pun;
using UnityEngine;

public class SpawnPoints : MonoBehaviourPunCallbacks
{
    PlayerScripts playerScripts;

    private void Awake()
    {
        GameManager.AllPlayersSpawned += FindDeck;
    }
    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        GameManager.AllPlayersSpawned -= FindDeck;
    }
    private void FindDeck()
    {
        playerScripts = transform.Find($"{PhotonNetwork.LocalPlayer.NickName}").GetComponent<PlayerScripts>();
        GameObject localPlayerObject = GameObject.Find($"{PhotonNetwork.LocalPlayer.NickName}/Player1PlayerArea/Deck1");
        Deck deck = localPlayerObject.transform.GetComponent<Deck>();
        playerScripts._deck = deck;

        playerScripts.InitializePlayerDeck();
    }
}
