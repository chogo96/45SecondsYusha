using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject otherPlayerPrefab;
    public GameObject playerPrefab;
    private Transform canvasTransform;
    private Vector3 localPlayerPosition = new Vector3(-961, -536, 0);
    private Vector3[] otherPlayerPositions;

    public static Action AllPlayersSpawned;

    private void Start()
    {
        // Find the Canvas - PlayerSpawn object
        canvasTransform = GameObject.Find("Canvas - PlayerSpawn").transform;
        if (canvasTransform == null)
        {
            Utils.LogRed("Canvas - PlayerSpawn object not found in Start.");
            return;
        }
        else
        {
            Utils.Log("Canvas - PlayerSpawn found in Start: " + canvasTransform.name);
        }

        otherPlayerPositions = new Vector3[]
        {
            new Vector3(-961, 16, 0),    // Player 2 (9 o'clock)
            new Vector3(91, 218, 0),     // Player 3 (12 o'clock)
            new Vector3(597, -167, 0)    // Player 4 (3 o'clock)
        };

        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnAllPlayers();
        }
        else
        {
            Utils.LogRed("PhotonNetwork is not connected and ready.");
        }
    }

    private void SpawnAllPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int playerIndex = player.ActorNumber - 1;
            Vector3 spawnPosition;

            if (player.IsLocal)
            {
                spawnPosition = localPlayerPosition;
                // Instantiate the local player
                GameObject playerObject = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
                SetupPlayerObject(playerObject, player, spawnPosition);
            }
            else
            {
                spawnPosition = otherPlayerPositions[playerIndex % otherPlayerPositions.Length];
                // Instantiate other players
                GameObject playerObject = PhotonNetwork.Instantiate(otherPlayerPrefab.name, Vector3.zero, Quaternion.identity);
                SetupPlayerObject(playerObject, player, spawnPosition);
            }
        }

        // Check if all players have spawned and invoke event
        CheckAllPlayersSpawned();
    }

    private void SetupPlayerObject(GameObject playerObject, Player player, Vector3 spawnPosition)
    {
        if (playerObject != null)
        {
            // Set a custom name to the player object
            playerObject.name = $"{player.UserId}_{player.ActorNumber}";

            // Set the player as a child of the Canvas - PlayerSpawn object
            playerObject.transform.SetParent(canvasTransform, false);
            playerObject.transform.localPosition = spawnPosition;

            // Assign PlayerScripts to GlobalSettings
            PlayerScripts playerScripts = playerObject.GetComponent<PlayerScripts>();
            if (player.IsLocal)
            {
                GlobalSettings.instance.AssignLowPlayer(playerScripts);
            }

            // Debugging information
            Utils.Log("Player parent after setting: " + playerObject.transform.parent.name);
            Utils.Log("Player local position after setting: " + playerObject.transform.localPosition);
        }
        else
        {
            Utils.LogRed("Player instantiation failed.");
        }
    }

    private void CheckAllPlayersSpawned()
    {
        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            AllPlayersSpawned?.Invoke();
        }
    }


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트가 아니면
            PhotonNetwork.LoadLevel("02.Lobby Scene");
        }
    }
}
