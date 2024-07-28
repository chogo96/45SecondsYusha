using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    private SpawnPoints spawnPoints;
    private Transform canvasTransform;
    GameObject canvasObject;

    private Vector3[] localPlayerPositions;

    private void Start()
    {
        // Find the SpawnPoints object
        spawnPoints = FindObjectOfType<SpawnPoints>();
        if (spawnPoints == null)
        {
            Debug.LogError("SpawnPoints object not found in Start.");
            return;
        }

        // Find the Canvas - PlayerSpawn object
        canvasObject = GameObject.Find("Canvas - PlayerSpawn");
        if (canvasObject == null)
        {
            Debug.LogError("Canvas - PlayerSpawn object not found in Start.");
            return;
        }
        else
        {
            canvasTransform = canvasObject.transform;
            Debug.Log("Canvas - PlayerSpawn found in Start: " + canvasTransform.name);
        }

        // Define local player positions relative to the local player's view
        localPlayerPositions = new Vector3[]
        {
            new Vector3(-830, -359, 0),  // Local player (6 o'clock)
            new Vector3(-843, 262, 0),   // Player 2 (9 o'clock)
            new Vector3(364, 416, 0),    // Player 3 (12 o'clock)
            new Vector3(734, 29, 0)      // Player 4 (3 o'clock)
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
            SpawnPlayer(player);
        }
    }

    private void SpawnPlayer(Player player)
    {
        int localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        int playerIndex = player.ActorNumber - 1;

        Vector3 spawnPosition;
        if (player.IsLocal)
        {
            spawnPosition = localPlayerPositions[0]; // Local player always at 6 o'clock
        }
        else
        {
            int relativeIndex = (playerIndex - localPlayerIndex + 4) % 4;
            spawnPosition = localPlayerPositions[relativeIndex];
        }

        // Instantiate the player
        Debug.Log("Spawning player at position: " + spawnPosition);
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        if (playerObject != null)
        {
            // Set a custom name to the player object
            playerObject.name = "Player_" + player.ActorNumber;

            // Set the player as a child of the Canvas - PlayerSpawn object
            Debug.Log("Setting player as child of Canvas - PlayerSpawn");
            playerObject.transform.SetParent(canvasTransform, false);
            playerObject.transform.localPosition = spawnPosition;

            // Debugging information
            Debug.Log("Player parent after setting: " + playerObject.transform.parent.name);
        }
        else
        {
            Debug.LogError("Player instantiation failed.");
        }
    }
}
