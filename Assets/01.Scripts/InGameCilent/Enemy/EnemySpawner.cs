using Newtonsoft.Json;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public GameObject shopPrefab; // ���� ������
    public Transform canvasTransform; // ���� ������ ĵ���� Ʈ������
    public List<EnemyData> normalEnemies; // �Ϲ� �� ������ ����Ʈ
    public List<EnemyData> midBosses; // �߰� ���� ������ ����Ʈ
    public List<EnemyData> finalBosses; // ���� ���� ������ ����Ʈ
    public List<PlayerScripts> players; // �÷��̾� ����Ʈ

    public List<string> spawnOrder; // ���� ������ ��� ����Ʈ
    private int currentEnemyIndex = 0;
    private string spawnOrderJson;

    // ������ �ε�� ��ųʸ�
    private Dictionary<string, GameObject> enemyPrefabs;

    void Start()
    {
        LoadEnemyData();
        LoadEnemyPrefabs(); // ������ �ε�
        StartCoroutine(WaitForPlayersAndSpawnEnemies());
    }

    private void LoadEnemyData()
    {
        normalEnemies = new List<EnemyData>(Resources.LoadAll<EnemyData>("GameAssets/Enemies/NormalEnemy"));
        midBosses = new List<EnemyData>(Resources.LoadAll<EnemyData>("GameAssets/Enemies/MinibossEnemy"));
        finalBosses = new List<EnemyData>(Resources.LoadAll<EnemyData>("GameAssets/Enemies/FinalBossEnemy"));

        Debug.Log("Normal Enemies Count: " + normalEnemies.Count);
        Debug.Log("Mid Bosses Count: " + midBosses.Count);
        Debug.Log("Final Bosses Count: " + finalBosses.Count);

        if (normalEnemies.Count == 0) Debug.LogError("Normal enemies list is empty.");
        if (midBosses.Count == 0) Debug.LogError("Mid bosses list is empty.");
        if (finalBosses.Count == 0) Debug.LogError("Final bosses list is empty.");
    }

    private void LoadEnemyPrefabs()
    {
        enemyPrefabs = new Dictionary<string, GameObject>();
        GameObject[] prefabs = Resources.LoadAll<GameObject>("SPUM/SPUM_Units"); // ��Ȯ�� ��η� ����

        foreach (GameObject prefab in prefabs)
        {
            if (!enemyPrefabs.ContainsKey(prefab.name))
            {
                enemyPrefabs[prefab.name] = prefab;
                Debug.Log($"Loaded prefab: {prefab.name}"); // �ε�� �������� �̸��� ���
            }
        }
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

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Error = (sender, args) =>
                {
                    Debug.LogError($"Serialization error: {args.ErrorContext.Error.Message}");
                    args.ErrorContext.Handled = true;
                }
            };

            spawnOrderJson = JsonConvert.SerializeObject(spawnOrder, settings);
            Utils.Log("Spawn order generated and serialized: " + spawnOrderJson);

            // �ٸ� Ŭ���̾�Ʈ���� ���� ���� ����
            photonView.RPC("RPC_SetSpawnOrder", RpcTarget.Others, spawnOrderJson);

            // ���� ������ ������ ������ ���
            yield return new WaitUntil(() => spawnOrder != null && spawnOrder.Count > 0);
            // ù ��° �� ��ȯ
            photonView.RPC("RPC_SpawnNextEnemy", RpcTarget.All, currentEnemyIndex);
        }
    }

    [PunRPC]
    void RPC_SetSpawnOrder(string json)
    {
        Utils.Log("Received spawn order JSON: " + json);
        spawnOrder = JsonConvert.DeserializeObject<List<string>>(json);

        if (spawnOrder == null || spawnOrder.Count == 0)
        {
            Utils.LogRed("Deserialized spawn order is null or empty!");
        }
        else
        {
            Utils.Log("Spawn order deserialized, count: " + spawnOrder.Count);
        }
    }

    void GenerateSpawnOrder()
    {
        spawnOrder = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            List<string> currentCycleEnemies = new List<string>();

            for (int j = 0; j < 9; j++) // 9���� �� �߰�
            {
                if (normalEnemies.Count > 0)
                {
                    var enemyData = normalEnemies[Random.Range(0, normalEnemies.Count)];
                    currentCycleEnemies.Add(enemyData.EnemyName);
                }
                else
                {
                    Utils.LogError("normalEnemies ����Ʈ�� ����ֽ��ϴ�.");
                }
            }

            spawnOrder.AddRange(currentCycleEnemies);
            spawnOrder.Add("Shop"); // ���� �߰�

            if (midBosses.Count > 0)
            {
                var midBoss = midBosses[Random.Range(0, midBosses.Count)];
                spawnOrder.Add(midBoss.EnemyName); // �߰� ���� �߰�
            }
            else
            {
                Utils.LogError("midBosses ����Ʈ�� ����ֽ��ϴ�.");
            }
        }

        if (finalBosses.Count > 0)
        {
            var finalBoss = finalBosses[Random.Range(0, finalBosses.Count)];
            spawnOrder.Add(finalBoss.EnemyName); // ���� ���� �߰�
        }
        else
        {
            Utils.LogRed("finalBosses ����Ʈ�� ����ֽ��ϴ�.");
        }
    }

    [PunRPC]
    void RPC_SpawnNextEnemy(int index)
    {
        if (spawnOrder == null || spawnOrder.Count == 0)
        {
            Debug.LogError("Spawn order is null or empty!");
            return;
        }

        if (index < spawnOrder.Count)
        {
            GameObject newEntity;

            if (spawnOrder[index] == "Shop")
            {
                // ���� ��ȯ
                newEntity = PhotonNetwork.Instantiate(shopPrefab.name, Vector3.zero, Quaternion.identity);
            }
            else
            {
                string enemyName = spawnOrder[index];

                // EnemyName�� �´� ������ �ε�
                if (!enemyPrefabs.TryGetValue(enemyName, out GameObject prefab))
                {
                    Debug.LogError($"No prefab found for enemy with name {enemyName}");
                    return;
                }

                // Photon�� ���� ������ �ν��Ͻ�ȭ
                newEntity = PhotonNetwork.Instantiate($"SPUM/SPUM_Units/{prefab.name}", Vector3.zero, Quaternion.identity);

                // �����տ��� �� ���� �ʱ�ȭ
                Enemy enemy = newEntity.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // EnemyData �ε�
                    var enemyData = Resources.Load<EnemyData>($"GameAssets/Enemies/NormalEnemy/{enemyName}");
                    if (enemyData == null)
                    {
                        enemyData = Resources.Load<EnemyData>($"GameAssets/Enemies/MinibossEnemy/{enemyName}");
                    }
                    if (enemyData == null)
                    {
                        enemyData = Resources.Load<EnemyData>($"GameAssets/Enemies/FinalBossEnemy/{enemyName}");
                    }

                    if (enemyData != null)
                    {
                        bool isFinalBoss = (index == spawnOrder.Count - 1);
                        enemy.Initialize(enemyData, players, isFinalBoss);
                    }
                    else
                    {
                        Debug.LogError($"EnemyData with name {enemyName} could not be found in Resources.");
                    }
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
