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

        // ���� ���� ���� �� �� ��ȯ
        GenerateSpawnOrder();


        // ������ Ŭ���̾�Ʈ�� ���� ����
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnNextEnemy();
        }
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


    [PunRPC]
    void RPC_SpawnNextEnemy(int index)
    {
        if (index < spawnOrder.Count)
        {
            if (spawnOrder[index] == null)
            {
                // ���� ��ȯ
                Instantiate(shopPrefab, canvasTransform);
            }
            else
            {
                EnemyData enemyData = spawnOrder[index];
                GameObject newEnemy = Instantiate(enemyPrefab, canvasTransform);
                Image enemyImage = newEnemy.GetComponent<Image>();
                enemyImage.sprite = enemyData.enemySprite;

                Enemy enemy = newEnemy.GetComponent<Enemy>();
                bool isFinalBoss = (index == spawnOrder.Count - 1);
                enemy.Initialize(enemyData, players, isFinalBoss);

                // �� ������ ���� (�� �� ��ȯ�� ���� �ٽ� ������ ����)
                normalEnemies.Remove(enemyData);
                midBosses.Remove(enemyData);
                finalBosses.Remove(enemyData);
            }
            currentEnemyIndex = index + 1;
        }
    }

    void SpawnNextEnemy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_SpawnNextEnemy", RpcTarget.All, currentEnemyIndex);
        }
    }
    #region �������� ������ ���� �ּ�ó��(����)
    //void SpawnNextEnemy()
    //{
    //    if (currentEnemyIndex < spawnOrder.Count)
    //    {
    //        if (spawnOrder[currentEnemyIndex] == null)
    //        {
    //            // ���� ��ȯ
    //            Instantiate(shopPrefab, canvasTransform);
    //        }
    //        else
    //        {
    //            EnemyData enemyData = spawnOrder[currentEnemyIndex];
    //            GameObject newEnemy = Instantiate(enemyPrefab, canvasTransform);
    //            Image enemyImage = newEnemy.GetComponent<Image>();
    //            enemyImage.sprite = enemyData.enemySprite;

    //            Enemy enemy = newEnemy.GetComponent<Enemy>();
    //            bool isFinalBoss = (currentEnemyIndex == spawnOrder.Count - 1);
    //            enemy.Initialize(enemyData, players, isFinalBoss);

    //            // �� ������ ���� (�� �� ��ȯ�� ���� �ٽ� ������ ����)
    //            normalEnemies.Remove(enemyData);
    //            midBosses.Remove(enemyData);
    //            finalBosses.Remove(enemyData);
    //        }
    //        currentEnemyIndex++;
    //    }
    //}
    #endregion

    public void OnShopButtonPressed()
    {
        // ���� ��ư�� ������ �� ���� óġ�� ��ó�� ���� ���� ��ȯ
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnNextEnemy();
        }
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
        SpawnNextEnemy();
    }

    internal void RegisterPlayer(PlayerScripts player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }
}
