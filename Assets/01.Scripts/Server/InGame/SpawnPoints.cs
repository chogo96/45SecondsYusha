using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public Vector3[] spawnPoints;

    private void Awake()
    {
        // ���� ��ġ �迭 (�ʿ信 ���� ��ġ�� �����մϴ�)
        spawnPoints = new Vector3[]
        {
            new Vector3(-830,-359,0),    // �÷��̾�1 - ����
            new Vector3(-843,262,0),   // �÷��̾�2
            new Vector3(364,416,0),   // �÷��̾�3
            new Vector3(734,29,0)     // �÷��̾�4
        };
    }
}
