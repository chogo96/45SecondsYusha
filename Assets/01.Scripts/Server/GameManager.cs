using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    private SpawnPoints spawnPoints;

    private void Start()
    {
        spawnPoints = FindObjectOfType<SpawnPoints>();

        if (PhotonNetwork.IsConnected)
        {
            // 배열 길이 검사 추가
            if (spawnPoints.spawnPoints.Length > 0)
            {
                int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // ActorNumber는 1부터 시작하므로 0부터 시작하는 인덱스를 위해 -1
                Vector3 spawnPosition = spawnPoints.spawnPoints[playerIndex % spawnPoints.spawnPoints.Length];

                // 플레이어 생성
                PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Spawn points array is empty.");
            }
        }
    }
}
