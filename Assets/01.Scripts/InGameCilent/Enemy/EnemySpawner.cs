using Newtonsoft.Json;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public GameObject shopPrefab; // 상점 프리팹
    public Transform canvasTransform; // 적을 생성할 캔버스 트랜스폼
    public List<EnemyData> normalEnemies; // 일반 몹 데이터 리스트
    public List<EnemyData> midBosses; // 중간 보스 데이터 리스트
    public List<EnemyData> finalBosses; // 최종 보스 데이터 리스트
    public List<PlayerScripts> players; // 플레이어 리스트

    public List<string> spawnOrder; // 스폰 순서를 담는 리스트
    private int currentEnemyIndex = 0;
    private string spawnOrderJson;

    // 프리팹 로드용 딕셔너리
    private Dictionary<string, GameObject> enemyPrefabs;

    void Start()
    {
        LoadEnemyData();
        LoadEnemyPrefabs(); // 프리팹 로드
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
        GameObject[] prefabs = Resources.LoadAll<GameObject>("SPUM/SPUM_Units"); // 정확한 경로로 수정

        foreach (GameObject prefab in prefabs)
        {
            if (!enemyPrefabs.ContainsKey(prefab.name))
            {
                enemyPrefabs[prefab.name] = prefab;
                Debug.Log($"Loaded prefab: {prefab.name}"); // 로드된 프리팹의 이름을 출력
            }
        }
    }



    private IEnumerator WaitForPlayersAndSpawnEnemies()
    {
        // PlayerScripts가 초기화될 때까지 대기
        yield return new WaitUntil(() => players != null && players.Count > 0);

        // PlayerScripts의 모든 인스턴스가 초기화될 때까지 대기
        yield return new WaitUntil(() => PlayerScripts.Players != null && PlayerScripts.Players.Length > 0);

        // 추가적인 초기화 확인을 위해 잠시 대기
        yield return new WaitForSeconds(1f);

        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트가 스폰 순서 생성 및 JSON 형식으로 직렬화
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

            // 다른 클라이언트에게 스폰 순서 전송
            photonView.RPC("RPC_SetSpawnOrder", RpcTarget.Others, spawnOrderJson);

            // 스폰 순서가 설정될 때까지 대기
            yield return new WaitUntil(() => spawnOrder != null && spawnOrder.Count > 0);
            // 첫 번째 적 소환
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

            for (int j = 0; j < 9; j++) // 9명의 적 추가
            {
                if (normalEnemies.Count > 0)
                {
                    var enemyData = normalEnemies[Random.Range(0, normalEnemies.Count)];
                    currentCycleEnemies.Add(enemyData.EnemyName);
                }
                else
                {
                    Utils.LogError("normalEnemies 리스트가 비어있습니다.");
                }
            }

            spawnOrder.AddRange(currentCycleEnemies);
            spawnOrder.Add("Shop"); // 상점 추가

            if (midBosses.Count > 0)
            {
                var midBoss = midBosses[Random.Range(0, midBosses.Count)];
                spawnOrder.Add(midBoss.EnemyName); // 중간 보스 추가
            }
            else
            {
                Utils.LogError("midBosses 리스트가 비어있습니다.");
            }
        }

        if (finalBosses.Count > 0)
        {
            var finalBoss = finalBosses[Random.Range(0, finalBosses.Count)];
            spawnOrder.Add(finalBoss.EnemyName); // 최종 보스 추가
        }
        else
        {
            Utils.LogRed("finalBosses 리스트가 비어있습니다.");
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
                // 상점 소환
                newEntity = PhotonNetwork.Instantiate(shopPrefab.name, Vector3.zero, Quaternion.identity);
            }
            else
            {
                string enemyName = spawnOrder[index];

                // EnemyName에 맞는 프리팹 로드
                if (!enemyPrefabs.TryGetValue(enemyName, out GameObject prefab))
                {
                    Debug.LogError($"No prefab found for enemy with name {enemyName}");
                    return;
                }

                // Photon을 통해 프리팹 인스턴스화
                newEntity = PhotonNetwork.Instantiate($"SPUM/SPUM_Units/{prefab.name}", Vector3.zero, Quaternion.identity);

                // 프리팹에서 적 정보 초기화
                Enemy enemy = newEntity.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // EnemyData 로드
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

            // newEntity의 부모를 canvasTransform으로 설정
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
        // 상점 버튼이 눌렸을 때 적을 처치한 것처럼 다음 적을 소환
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
