using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    private Transform canvasTransform;
    private Vector3[] localPlayerPositions;

    public static Action AllPlayersSpawned;

    private void Start()
    {
        // Find the Canvas - PlayerSpawn object
        canvasTransform = GameObject.Find("Canvas - PlayerSpawn").transform;
        if (canvasTransform == null)
        {
            Debug.LogError("Canvas - PlayerSpawn object not found in Start.");
            return;
        }
        else
        {
            Debug.Log("Canvas - PlayerSpawn found in Start: " + canvasTransform.name);
        }

        // Define local player positions relative to the local player's view
        localPlayerPositions = new Vector3[]
        {
            new Vector3(-961, -536, 0),  // Local player (6 o'clock)
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
            Debug.LogError("PhotonNetwork is not connected and ready.");
        }
    }

    private void SpawnAllPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int playerIndex = player.ActorNumber - 1;
            Vector3 spawnPosition = localPlayerPositions[playerIndex % localPlayerPositions.Length];

            // Instantiate the player
            GameObject playerObject = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
            if (playerObject != null)
            {
                // Set a custom name to the player object
                playerObject.name = "Player_" + player.ActorNumber;

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
                Debug.Log("Player parent after setting: " + playerObject.transform.parent.name);
            }
            else
            {
                Debug.LogError("Player instantiation failed.");
            }
        }

        // Check if all players have spawned and invoke event
        CheckAllPlayersSpawned();
    }

    private void CheckAllPlayersSpawned()
    {
        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            AllPlayersSpawned?.Invoke();
        }
    }
}
