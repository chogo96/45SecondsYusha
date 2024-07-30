//using Photon.Pun;
//using Photon.Realtime;
//using UnityEngine;
//using System;

//public class GameManager : MonoBehaviourPunCallbacks
//{
//    public GameObject playerPrefab;
//    private SpawnPoints spawnPoints;
//    private Transform canvasTransform;
//    GameObject canvasObject;

//    private Vector3[] localPlayerPositions;

//    public static Action AllPlayersSpawned;

//    private void Start()
//    {
//        // Find the SpawnPoints object
//        spawnPoints = FindObjectOfType<SpawnPoints>();
//        if (spawnPoints == null)
//        {
//            Debug.LogError("SpawnPoints object not found in Start.");
//            return;
//        }

//        // Find the Canvas - PlayerSpawn object
//        canvasObject = GameObject.Find("Canvas - PlayerSpawn");
//        if (canvasObject == null)
//        {
//            Debug.LogError("Canvas - PlayerSpawn object not found in Start.");
//            return;
//        }
//        else
//        {
//            canvasTransform = canvasObject.transform;
//            Debug.Log("Canvas - PlayerSpawn found in Start: " + canvasTransform.name);
//        }

//        // Define local player positions relative to the local player's view
//        localPlayerPositions = new Vector3[]
//        {
//            new Vector3(-961, -536, 0),  // Local player (6 o'clock)
//            new Vector3(-961,16,0),   // Player 2 (9 o'clock)
//            new Vector3(91,218,0),    // Player 3 (12 o'clock)
//            new Vector3(597,-167,0)      // Player 4 (3 o'clock)
//        };

//        if (PhotonNetwork.IsConnectedAndReady)
//        {
//            SpawnPlayer(PhotonNetwork.LocalPlayer);
//        }
//        else
//        {
//            Debug.LogError("PhotonNetwork is not connected and ready.");
//        }
//    }

//    private void SpawnPlayer(Player player)
//    {
//        int localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
//        int playerIndex = player.ActorNumber - 1;

//        Vector3 spawnPosition;
//        if (player.IsLocal)
//        {
//            spawnPosition = localPlayerPositions[0]; // Local player always at 6 o'clock
//        }
//        else
//        {
//            int relativeIndex = (playerIndex - localPlayerIndex + 4) % 4;
//            spawnPosition = localPlayerPositions[relativeIndex];
//        }

//        // Instantiate the player
//        Debug.Log("Spawning player at position: " + spawnPosition);
//        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

//        if (playerObject != null)
//        {
//            // Set a custom name to the player object
//            playerObject.name = "Player_" + player.ActorNumber;

//            // Set the player as a child of the Canvas - PlayerSpawn object
//            Debug.Log("Setting player as child of Canvas - PlayerSpawn");
//            playerObject.transform.SetParent(canvasTransform, false);
//            playerObject.transform.localPosition = spawnPosition;

//            // Debugging information
//            Debug.Log("Player parent after setting: " + playerObject.transform.parent.name);

//            // 모든 플레이어가 스폰되었는지 확인하고 이벤트 호출
//            CheckAllPlayersSpawned();
//        }
//        else
//        {
//            Debug.LogError("Player instantiation failed.");
//        }
//    }

//    private void CheckAllPlayersSpawned()
//    {
//        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.PlayerCount)
//        {
//            AllPlayersSpawned?.Invoke();
//        }
//    }
//}
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    private SpawnPoints spawnPoints;
    private Transform canvasTransform;
    GameObject canvasObject;

    private Vector3[] localPlayerPositions;

    public static Action AllPlayersSpawned;

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
            new Vector3(-961, -536, 0),  // Local player (6 o'clock)
            new Vector3(-961,16,0),   // Player 2 (9 o'clock)
            new Vector3(91,218,0),    // Player 3 (12 o'clock)
            new Vector3(597,-167,0)      // Player 4 (3 o'clock)
        };

        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer(PhotonNetwork.LocalPlayer);
        }
        else
        {
            Debug.LogError("PhotonNetwork is not connected and ready.");
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

            // Assign PlayerScripts to GlobalSettings
            PlayerScripts playerScripts = playerObject.GetComponent<PlayerScripts>();
            if (player.IsLocal)
            {
                GlobalSettings.instance.AssignLowPlayer(playerScripts);
            }

            // Debugging information
            Debug.Log("Player parent after setting: " + playerObject.transform.parent.name);

            // Check if all players have spawned and invoke event
            CheckAllPlayersSpawned();
        }
        else
        {
            Debug.LogError("Player instantiation failed.");
        }
    }

    private void CheckAllPlayersSpawned()
    {
        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            AllPlayersSpawned?.Invoke();
        }
    }
}
