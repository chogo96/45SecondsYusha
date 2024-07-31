//using Newtonsoft.Json;
//using Photon.Pun;
//using Photon.Realtime;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class EnemySpawner : MonoBehaviourPunCallbacks
//{
//    public GameObject enemyPrefab; // 적 프리팹
//    public GameObject shopPrefab; // 상점 프리팹
//    public Transform canvasTransform; // 적을 생성할 캔버스 트랜스폼
//    public List<EnemyData> normalEnemies; // 일반 몹 데이터 리스트
//    public List<EnemyData> midBosses; // 중간 보스 데이터 리스트
//    public List<EnemyData> finalBosses; // 최종 보스 데이터 리스트
//    public List<PlayerScripts> players; // 플레이어 리스트

//    private List<EnemyData> spawnOrder; // 스폰 순서를 담는 리스트
//    private int currentEnemyIndex = 0;
//    private string spawnOrderJson;


//    void Start()
//    {
//        StartCoroutine(WaitForPlayersAndSpawnEnemies());

//    }

//    private IEnumerator WaitForPlayersAndSpawnEnemies()
//    {
//        // PlayerScripts가 초기화될 때까지 대기
//        yield return new WaitUntil(() => players != null && players.Count > 0);

//        // PlayerScripts의 모든 인스턴스가 초기화될 때까지 대기
//        yield return new WaitUntil(() => PlayerScripts.Players != null && PlayerScripts.Players.Length > 0);

//        // 추가적인 초기화 확인을 위해 잠시 대기
//        yield return new WaitForSeconds(1f);

//        // 마스터 클라이언트가 스폰 순서 생성 및 순서리스트 를 Json 형식으로 다른클라이언트 에게 전송
//        GenerateSpawnOrder();

//        photonView.RPC("RPC_SpawnNextEnemy", RpcTarget.All, currentEnemyIndex);

//    }


//    void GenerateSpawnOrder()
//    {
//        spawnOrder = new List<EnemyData>();
//        for (int i = 0; i < 3; i++)
//        {
//            List<EnemyData> currentCycleEnemies = new List<EnemyData>();

//            // 10개의 적 소환 (중간에 무작위로 한 번 상점을 추가)
//            for (int j = 0; j < 10; j++)
//            {
//                currentCycleEnemies.Add(normalEnemies[Random.Range(0, normalEnemies.Count)]);
//            }

//            // 중간에 무작위로 한 번 상점 추가
//            int randomIndex = Random.Range(0, 10);
//            currentCycleEnemies[randomIndex] = null; // null을 사용하여 상점을 표시

//            // 현재 사이클의 적을 스폰 순서에 추가
//            spawnOrder.AddRange(currentCycleEnemies);

//            // 랜덤 중간 보스 추가
//            spawnOrder.Add(midBosses[Random.Range(0, midBosses.Count)]);
//        }

//        // 랜덤 최종 보스 추가
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
//                // 상점 소환
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

//                    // 적 데이터 삭제 (한 번 소환된 적은 다시 나오지 않음)
//                    normalEnemies.Remove(enemyData);
//                    midBosses.Remove(enemyData);
//                    finalBosses.Remove(enemyData);
//                }
//            }

//            // newEntity의 부모를 canvasTransform으로 설정
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
//                // 상점 소환
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

//                    // 적 데이터 삭제 (한 번 소환된 적은 다시 나오지 않음)
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
//        // 상점 버튼이 눌렸을 때 적을 처치한 것처럼 다음 적을 소환
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
    public GameObject enemyPrefab; // 적 프리팹
    public GameObject shopPrefab; // 상점 프리팹
    public Transform canvasTransform; // 적을 생성할 캔버스 트랜스폼
    public List<EnemyData> normalEnemies; // 일반 몹 데이터 리스트
    public List<EnemyData> midBosses; // 중간 보스 데이터 리스트
    public List<EnemyData> finalBosses; // 최종 보스 데이터 리스트
    public List<PlayerScripts> players; // 플레이어 리스트

    private List<EnemyData> spawnOrder; // 스폰 순서를 담는 리스트
    private int currentEnemyIndex = 0;
    private string spawnOrderJson;

    void Start()
    {
        StartCoroutine(WaitForPlayersAndSpawnEnemies());
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
            spawnOrderJson = JsonConvert.SerializeObject(spawnOrder);
            Debug.Log("Spawn order generated and serialized: " + spawnOrderJson);

            // 다른 클라이언트에게 스폰 순서 전송
            photonView.RPC("RPC_SetSpawnOrder", RpcTarget.Others, spawnOrderJson);
        }

        // 스폰 순서가 설정될 때까지 대기
        yield return new WaitUntil(() => spawnOrder != null);

        // 첫 번째 적 소환
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

            for (int j = 0; j < 9; j++) // 9명의 적 추가
            {
                currentCycleEnemies.Add(normalEnemies[Random.Range(0, normalEnemies.Count)]);
            }

            spawnOrder.AddRange(currentCycleEnemies);
            spawnOrder.Add(null); // 상점 추가
            spawnOrder.Add(midBosses[Random.Range(0, midBosses.Count)]); // 중간 보스 추가
        }

        spawnOrder.Add(finalBosses[Random.Range(0, finalBosses.Count)]); // 최종 보스 추가
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
                // 상점 소환
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
