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
            // �迭 ���� �˻� �߰�
            if (spawnPoints.spawnPoints.Length > 0)
            {
                int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // ActorNumber�� 1���� �����ϹǷ� 0���� �����ϴ� �ε����� ���� -1
                Vector3 spawnPosition = spawnPoints.spawnPoints[playerIndex % spawnPoints.spawnPoints.Length];

                // �÷��̾� ����
                PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Spawn points array is empty.");
            }
        }
    }
}
