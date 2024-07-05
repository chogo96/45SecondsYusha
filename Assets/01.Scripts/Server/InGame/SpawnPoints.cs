using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public Vector3[] spawnPoints;

    private void Awake()
    {
        // ���� ��ġ �迭 (�ʿ信 ���� ��ġ�� �����մϴ�)
        spawnPoints = new Vector3[]
        {
            new Vector3(0, 1, -4),   // �÷��̾�1
            new Vector3(-4, 1, 0),    // �÷��̾�2
            new Vector3(0, 1, 4),   // �÷��̾�3
            new Vector3(4, 1, 0)     // �÷��̾�4
        };
    }
}
