using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemySpawner : MonoBehaviour
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

    void Start()
    {
        GenerateSpawnOrder();
        SpawnNextEnemy();
    }

    void GenerateSpawnOrder()
    {
        spawnOrder = new List<EnemyData>();

        for (int i = 0; i < 3; i++)
        {
            List<EnemyData> currentCycleEnemies = new List<EnemyData>();

            // 10개의 적 소환 (중간에 무작위로 한 번 상점을 추가)
            for (int j = 0; j < 10; j++)
            {
                currentCycleEnemies.Add(normalEnemies[Random.Range(0, normalEnemies.Count)]);
            }

            // 중간에 무작위로 한 번 상점 추가
            int randomIndex = Random.Range(0, 10);
            currentCycleEnemies[randomIndex] = null; // null을 사용하여 상점을 표시

            // 현재 사이클의 적을 스폰 순서에 추가
            spawnOrder.AddRange(currentCycleEnemies);

            // 랜덤 중간 보스 추가
            spawnOrder.Add(midBosses[Random.Range(0, midBosses.Count)]);
        }

        // 랜덤 최종 보스 추가
        spawnOrder.Add(finalBosses[Random.Range(0, finalBosses.Count)]);
    }

    void SpawnNextEnemy()
    {
        if (currentEnemyIndex < spawnOrder.Count)
        {
            if (spawnOrder[currentEnemyIndex] == null)
            {
                // 상점 소환
                Instantiate(shopPrefab, canvasTransform);
            }
            else
            {
                EnemyData enemyData = spawnOrder[currentEnemyIndex];
                GameObject newEnemy = Instantiate(enemyPrefab, canvasTransform);
                Image enemyImage = newEnemy.GetComponent<Image>();
                enemyImage.sprite = enemyData.enemySprite;

                Enemy enemy = newEnemy.GetComponent<Enemy>();
                bool isFinalBoss = (currentEnemyIndex == spawnOrder.Count - 1);
                enemy.Initialize(enemyData, players, isFinalBoss);

                // 적 데이터 삭제 (한 번 소환된 적은 다시 나오지 않음)
                normalEnemies.Remove(enemyData);
                midBosses.Remove(enemyData);
                finalBosses.Remove(enemyData);
            }
            currentEnemyIndex++;
        }
    }

    public void OnShopButtonPressed()
    {
        // 상점 버튼이 눌렸을 때 적을 처치한 것처럼 다음 적을 소환
        SpawnNextEnemy();
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
        StartCoroutine(SpawnNextEnemyAfterDelay(1f));
    }

    private IEnumerator SpawnNextEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNextEnemy();
    }
}