using Newtonsoft.Json;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public GameObject enemyPrefab; // 적 프리팹
    public GameObject shopPrefab; // 상점 프리팹
    public Transform canvasTransform; // 적을 생성할 캔버스 트랜스폼
    public List<EnemyData> normalEnemies; // 일반 몹 데이터 리스트
    public List<EnemyData> midBosses; // 중간 보스 데이터 리스트
    public List<EnemyData> finalBosses; // 최종 보스 데이터 리스트
    public List<PlayerScripts> players; // 플레이어 리스트

    public List<string> spawnOrder; // 스폰 순서를 담는 리스트
    private int currentEnemyIndex = 0;
    private string spawnOrderJson;

    void Start()
    {
        LoadEnemyData();
        StartCoroutine(WaitForPlayersAndSpawnEnemies());
    }

    private void LoadEnemyData()
    {
        normalEnemies = new List<EnemyData>(Resources.LoadAll<EnemyData>("GameAssets/Enemies/NormalEnemy"));
        midBosses = new List<EnemyData>(Resources.LoadAll<EnemyData>("GameAssets/Enemies/MinibossEnemy"));
        finalBosses = new List<EnemyData>(Resources.LoadAll<EnemyData>("GameAssets/Enemies/FinalBossEnemy"));
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
                    args.ErrorContext.Handled = true;
                }
            };

            spawnOrderJson = JsonConvert.SerializeObject(spawnOrder, settings);

            // 다른 클라이언트에게 스폰 순서 전송
            photonView.RPC("RPC_SetSpawnOrder", RpcTarget.Others, spawnOrderJson);
        }

        // 스폰 순서가 설정될 때까지 대기
        yield return new WaitUntil(() => spawnOrder != null && spawnOrder.Count > 0);


        // 첫 번째 적 소환
        photonView.RPC("RPC_SpawnNextEnemy", RpcTarget.All, currentEnemyIndex);
    }

    [PunRPC]
    void RPC_SetSpawnOrder(string json)
    {
        spawnOrder = JsonConvert.DeserializeObject<List<string>>(json);

        if (spawnOrder == null || spawnOrder.Count == 0)
        {
        }
        else
        {
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
            }

            spawnOrder.AddRange(currentCycleEnemies);
            spawnOrder.Add("Shop"); // 상점 추가

            if (midBosses.Count > 0)
            {
                var midBoss = midBosses[Random.Range(0, midBosses.Count)];
                spawnOrder.Add(midBoss.EnemyName); // 중간 보스 추가
            }

        }

        if (finalBosses.Count > 0)
        {
            var finalBoss = finalBosses[Random.Range(0, finalBosses.Count)];
            spawnOrder.Add(finalBoss.EnemyName); // 최종 보스 추가
        }
    }

    [PunRPC]
    void RPC_SpawnNextEnemy(int index)
    {
        if (spawnOrder == null || spawnOrder.Count == 0)
        {
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
                var enemyData = Resources.Load<EnemyData>($"GameAssets/Enemies/NormalEnemy/{spawnOrder[index]}");
                if (enemyData == null)
                {
                    enemyData = Resources.Load<EnemyData>($"GameAssets/Enemies/MinibossEnemy/{spawnOrder[index]}");
                }
                if (enemyData == null)
                {
                    enemyData = Resources.Load<EnemyData>($"GameAssets/Enemies/FinalBossEnemy/{spawnOrder[index]}");
                }
                if (enemyData == null)
                {
                    return;
                }

                newEntity = PhotonNetwork.Instantiate(enemyPrefab.name, Vector3.zero, Quaternion.identity);
                Image enemyImage = newEntity.GetComponent<Image>();
                if (enemyImage != null && enemyData.enemySprite != null)
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
