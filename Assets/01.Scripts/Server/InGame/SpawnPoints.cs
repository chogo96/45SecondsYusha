using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public Vector3[] spawnPoints;

    private void Awake()
    {
        // 예제 위치 배열 (필요에 따라 위치를 정의합니다)
        spawnPoints = new Vector3[]
        {
            new Vector3(0, 1, -4),   // 플레이어1
            new Vector3(-4, 1, 0),    // 플레이어2
            new Vector3(0, 1, 4),   // 플레이어3
            new Vector3(4, 1, 0)     // 플레이어4
        };
    }
}
