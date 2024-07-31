//using Newtonsoft.Json;
//using Photon.Pun;
//using Photon.Realtime;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class EnemySpawner : MonoBehaviourPunCallbacks
//{
//    public GameObject enemyPrefab; // �� ������
//    public GameObject shopPrefab; // ���� ������
//    public Transform canvasTransform; // ���� ������ ĵ���� Ʈ������
//    public List<EnemyData> normalEnemies; // �Ϲ� �� ������ ����Ʈ
//    public List<EnemyData> midBosses; // �߰� ���� ������ ����Ʈ
//    public List<EnemyData> finalBosses; // ���� ���� ������ ����Ʈ
//    public List<PlayerScripts> players; // �÷��̾� ����Ʈ

//    private List<EnemyData> spawnOrder; // ���� ������ ��� ����Ʈ
//    private int currentEnemyIndex = 0;
//    private string spawnOrderJson;


//    void Start()
//    {
//        StartCoroutine(WaitForPlayersAndSpawnEnemies());

//    }

//    private IEnumerator WaitForPlayersAndSpawnEnemies()
//    {
//        // PlayerScripts�� �ʱ�ȭ�� ������ ���
//        yield return new WaitUntil(() => players != null && players.Count > 0);

//        // PlayerScripts�� ��� �ν��Ͻ��� �ʱ�ȭ�� ������ ���
//        yield return new WaitUntil(() => PlayerScripts.Players != null && PlayerScripts.Players.Length > 0);

//        // �߰����� �ʱ�ȭ Ȯ���� ���� ��� ���
//        yield return new WaitForSeconds(1f);

//        // ������ Ŭ���̾�Ʈ�� ���� ���� ���� �� ��������Ʈ �� Json �������� �ٸ�Ŭ���̾�Ʈ ���� ����
//        GenerateSpawnOrder();

//        photonView.RPC("RPC_SpawnNextEnemy", RpcTarget.All, currentEnemyIndex);

//    }


//    void GenerateSpawnOrder()
//    {
//        spawnOrder = new List<EnemyData>();
//        for (int i = 0; i < 3; i++)
//        {
//            List<EnemyData> currentCycleEnemies = new List<EnemyData>();

//            // 10���� �� ��ȯ (�߰��� �������� �� �� ������ �߰�)
//            for (int j = 0; j < 10; j++)
//            {
//                currentCycleEnemies.Add(normalEnemies[Random.Range(0, normalEnemies.Count)]);
//            }

//            // �߰��� �������� �� �� ���� �߰�
//            int randomIndex = Random.Range(0, 10);
//            currentCycleEnemies[randomIndex] = null; // null�� ����Ͽ� ������ ǥ��

//            // ���� ����Ŭ�� ���� ���� ������ �߰�
//            spawnOrder.AddRange(currentCycleEnemies);

//            // ���� �߰� ���� �߰�
//            spawnOrder.Add(midBosses[Random.Range(0, midBosses.Count)]);
//        }

//        // ���� ���� ���� �߰�
//        spawnOrder.Add(finalBosses[Random.Range(0, finalBosses.Count)]);

//    }

//    [PunRPC]
//    void RPC_SpawnNextEnemy(int index)
//    {
//        if (index < spawnOrder.Count)
//        {
//            GameObject newEntity;

//            if (spawnOrder[index] == null)
//            {
//                // ���� ��ȯ
//                newEntity = PhotonNetwork.Instantiate(shopPrefab.name, Vector3.zero, Quaternion.identity);
//            }
//            else
//            {
//                EnemyData enemyData = spawnOrder[index];
//                newEntity = PhotonNetwork.Instantiate(enemyPrefab.name, Vector3.zero, Quaternion.identity);
//                Image enemyImage = newEntity.GetComponent<Image>();
//                if (enemyImage != null)
//                {
//                    enemyImage.sprite = enemyData.enemySprite;
//                }

//                Enemy enemy = newEntity.GetComponent<Enemy>();
//                if (enemy != null)
//                {
//                    bool isFinalBoss = (index == spawnOrder.Count - 1);
//                    enemy.Initialize(enemyData, players, isFinalBoss);

//                    // �� ������ ���� (�� �� ��ȯ�� ���� �ٽ� ������ ����)
//                    normalEnemies.Remove(enemyData);
//                    midBosses.Remove(enemyData);
//                    finalBosses.Remove(enemyData);
//                }
//            }

//            // newEntity�� �θ� canvasTransform���� ����
//            newEntity.transform.SetParent(canvasTransform, false);

//            currentEnemyIndex = index + 1;
//        }
//    }

//    /*
//    [PunRPC]
//    void RPC_SpawnNextEnemy(int index)
//    {
//        if (index < spawnOrder.Count)
//        {
//            if (spawnOrder[index] == null)
//            {
//                // ���� ��ȯ
//                Instantiate(shopPrefab, canvasTransform);
//            }
//            else
//            {
//                EnemyData enemyData = spawnOrder[index];
//                GameObject newEnemy = Instantiate(enemyPrefab, canvasTransform);
//                Image enemyImage = newEnemy.GetComponent<Image>();
//                if (enemyImage != null)
//                {
//                    enemyImage.sprite = enemyData.enemySprite;
//                }

//                Enemy enemy = newEnemy.GetComponent<Enemy>();
//                if (enemy != null)
//                {
//                    bool isFinalBoss = (index == spawnOrder.Count - 1);
//                    enemy.Initialize(enemyData, players, isFinalBoss);

//                    // �� ������ ���� (�� �� ��ȯ�� ���� �ٽ� ������ ����)
//                    normalEnemies.Remove(enemyData);
//                    midBosses.Remove(enemyData);
//                    finalBosses.Remove(enemyData);
//                }
//            }
//            currentEnemyIndex = index + 1;
//        }
//    }
//    */

//    public void OnShopButtonPressed()
//    {
//        // ���� ��ư�� ������ �� ���� óġ�� ��ó�� ���� ���� ��ȯ
//        photonView.RPC("RPC_SpawnNextEnemy", RpcTarget.All, currentEnemyIndex);
//    }

//    void OnEnable()
//    {
//        Enemy.OnEnemyDeath += HandleEnemyDeath;
//    }

//    void OnDisable()
//    {
//        Enemy.OnEnemyDeath -= HandleEnemyDeath;
//    }

//    private void HandleEnemyDeath()
//    {
//        if (PhotonNetwork.IsMasterClient)
//        {
//            StartCoroutine(SpawnNextEnemyAfterDelay(1f));
//        }
//    }

//    private IEnumerator SpawnNextEnemyAfterDelay(float delay)
//    {
//        yield return new WaitForSeconds(delay);
//        photonView.RPC("RPC_SpawnNextEnemy", RpcTarget.All, currentEnemyIndex);
//    }

//    internal void RegisterPlayer(PlayerScripts player)
//    {
//        if (!players.Contains(player))
//        {
//            players.Add(player);
//        }
//    }
//}
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public GameObject enemyPrefab; // �� ������
    public GameObject shopPrefab; // ���� ������
    public Transform canvasTransform; // ���� ������ ĵ���� Ʈ������
    public List<EnemyData> normalEnemies; // �Ϲ� �� ������ ����Ʈ
    public List<EnemyData> midBosses; // �߰� ���� ������ ����Ʈ
    public List<EnemyData> finalBosses; // ���� ���� ������ ����Ʈ
    public List<PlayerScripts> players; // �÷��̾� ����Ʈ

    private List<EnemyData> spawnOrder; // ���� ������ ��� ����Ʈ
    private int currentEnemyIndex = 0;
    private string spawnOrderJson;

    void Start()
    {
        StartCoroutine(WaitForPlayersAndSpawnEnemies());
    }

    private IEnumerator WaitForPlayersAndSpawnEnemies()
    {
        // PlayerScripts�� �ʱ�ȭ�� ������ ���
        yield return new WaitUntil(() => players != null && players.Count > 0);

        // PlayerScripts�� ��� �ν��Ͻ��� �ʱ�ȭ�� ������ ���
        yield return new WaitUntil(() => PlayerScripts.Players != null && PlayerScripts.Players.Length > 0);

        // �߰����� �ʱ�ȭ Ȯ���� ���� ��� ���
        yield return new WaitForSeconds(1f);

        if (PhotonNetwork.IsMasterClient)
        {
            // ������ Ŭ���̾�Ʈ�� ���� ���� ���� �� JSON �������� ����ȭ
            GenerateSpawnOrder();
            spawnOrderJson = JsonConvert.SerializeObject(spawnOrder);
            Debug.Log("Spawn order generated and serialized: " + spawnOrderJson);

            // �ٸ� Ŭ���̾�Ʈ���� ���� ���� ����
            photonView.RPC("RPC_SetSpawnOrder", RpcTarget.Others, spawnOrderJson);
        }

        // ���� ������ ������ ������ ���
        yield return new WaitUntil(() => spawnOrder != null);

        // ù ��° �� ��ȯ
        photonView.RPC("RPC_SpawnNextEnemy", RpcTarget.All, currentEnemyIndex);
    }

    [PunRPC]
    void RPC_SetSpawnOrder(string json)
    {
        Debug.Log("Received spawn order JSON: " + json);
        spawnOrder = JsonConvert.DeserializeObject<List<EnemyData>>(json);
        Debug.Log("Spawn order deserialized, count: " + spawnOrder.Count);
    }

    void GenerateSpawnOrder()
    {
        spawnOrder = new List<EnemyData>();
        for (int i = 0; i < 3; i++)
        {
            List<EnemyData> currentCycleEnemies = new List<EnemyData>();

            for (int j = 0; j < 9; j++) // 9���� �� �߰�
            {
                currentCycleEnemies.Add(normalEnemies[Random.Range(0, normalEnemies.Count)]);
            }

            spawnOrder.AddRange(currentCycleEnemies);
            spawnOrder.Add(null); // ���� �߰�
            spawnOrder.Add(midBosses[Random.Range(0, midBosses.Count)]); // �߰� ���� �߰�
        }

        spawnOrder.Add(finalBosses[Random.Range(0, finalBosses.Count)]); // ���� ���� �߰�
    }

    [PunRPC]
    void RPC_SpawnNextEnemy(int index)
    {
        if (spawnOrder == null)
        {
            Debug.LogError("Spawn order is null!");
            return;
        }

        if (index < spawnOrder.Count)
        {
            GameObject newEntity;

            if (spawnOrder[index] == null)
            {
                // ���� ��ȯ
                newEntity = PhotonNetwork.Instantiate(shopPrefab.name, Vector3.zero, Quaternion.identity);
            }
            else
            {
                EnemyData enemyData = spawnOrder[index];
                newEntity = PhotonNetwork.Instantiate(enemyPrefab.name, Vector3.zero, Quaternion.identity);
                Image enemyImage = newEntity.GetComponent<Image>();
                if (enemyImage != null)
                {
                    enemyImage.sprite = enemyData.enemySprite;
                }

                Enemy enemy = newEntity.GetComponent<Enemy>();
                if (enemy != null)
                {
                    bool isFinalBoss = (index == spawnOrder.Count - 1);
                    enemy.Initialize(enemyData, players, isFinalBoss);
                }
            }

            // newEntity�� �θ� canvasTransform���� ����
            newEntity.transform.SetParent(canvasTransform, false);

            currentEnemyIndex = index + 1;
        }
        else
        {
            Debug.LogError("Index out of range: " + index);
        }
    }

    public void OnShopButtonPressed()
    {
        // ���� ��ư�� ������ �� ���� óġ�� ��ó�� ���� ���� ��ȯ
        photonView.RPC("RPC_SpawnNextEnemy", RpcTarget.All, currentEnemyIndex);
    }

    void OnEnable()
    {
        Enemy.OnEnemyDeath += HandleEnemyDeath;
    }

    void OnDisable()
    {
        Enemy.OnEnemyDeath -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnNextEnemyAfterDelay(1f));
        }
    }

    private IEnumerator SpawnNextEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        photonView.RPC("RPC_SpawnNextEnemy", RpcTarget.All, currentEnemyIndex);
    }

    internal void RegisterPlayer(PlayerScripts player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }
}
