using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemySpawner : MonoBehaviour
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

            // 10���� �� ��ȯ (�߰��� �������� �� �� ������ �߰�)
            for (int j = 0; j < 10; j++)
            {
                currentCycleEnemies.Add(normalEnemies[Random.Range(0, normalEnemies.Count)]);
            }

            // �߰��� �������� �� �� ���� �߰�
            int randomIndex = Random.Range(0, 10);
            currentCycleEnemies[randomIndex] = null; // null�� ����Ͽ� ������ ǥ��

            // ���� ����Ŭ�� ���� ���� ������ �߰�
            spawnOrder.AddRange(currentCycleEnemies);

            // ���� �߰� ���� �߰�
            spawnOrder.Add(midBosses[Random.Range(0, midBosses.Count)]);
        }

        // ���� ���� ���� �߰�
        spawnOrder.Add(finalBosses[Random.Range(0, finalBosses.Count)]);
    }

    void SpawnNextEnemy()
    {
        if (currentEnemyIndex < spawnOrder.Count)
        {
            if (spawnOrder[currentEnemyIndex] == null)
            {
                // ���� ��ȯ
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

                // �� ������ ���� (�� �� ��ȯ�� ���� �ٽ� ������ ����)
                normalEnemies.Remove(enemyData);
                midBosses.Remove(enemyData);
                finalBosses.Remove(enemyData);
            }
            currentEnemyIndex++;
        }
    }

    public void OnShopButtonPressed()
    {
        // ���� ��ư�� ������ �� ���� óġ�� ��ó�� ���� ���� ��ȯ
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