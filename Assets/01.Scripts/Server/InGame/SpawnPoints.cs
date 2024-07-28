using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public Vector3[] spawnPoints;

    private void Awake()
    {
        // 예제 위치 배열 (필요에 따라 위치를 정의합니다)
        spawnPoints = new Vector3[]
        {
            new Vector3(-830,-359,0),    // 플레이어1 - 본인
            new Vector3(-843,262,0),   // 플레이어2
            new Vector3(364,416,0),   // 플레이어3
            new Vector3(734,29,0)     // 플레이어4
        };
    }
}
