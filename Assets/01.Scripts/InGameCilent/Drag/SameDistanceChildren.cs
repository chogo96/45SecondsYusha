using UnityEngine;
using System.Collections;

// 첫 번째와 마지막 요소는 수동으로 자식 배열에 배치합니다.
// 나머지 요소들은 첫 번째와 마지막 요소 사이에 동일한 거리로 자동 배치됩니다.
public class SameDistanceChildren : MonoBehaviour
{
    public Transform[] Children; // 자식 오브젝트 배열

    // 초기화 시 실행되는 함수
    void Awake()
    {
        // 첫 번째 요소의 위치를 가져옴
        Vector3 firstElementPos = Children[0].transform.position;
        // 마지막 요소의 위치를 가져옴
        Vector3 lastElementPos = Children[Children.Length - 1].transform.position;

        // Children.Length - 1로 나누는 이유는, 예를 들어 10개의 점 사이에는 9개의 구간이 있기 때문임
        float XDist = (lastElementPos.x - firstElementPos.x) / (float)(Children.Length - 1); // X축 거리 계산
        float YDist = (lastElementPos.y - firstElementPos.y) / (float)(Children.Length - 1); // Y축 거리 계산
        float ZDist = (lastElementPos.z - firstElementPos.z) / (float)(Children.Length - 1); // Z축 거리 계산

        // X, Y, Z축 거리 값을 포함하는 벡터 생성
        Vector3 Dist = new Vector3(XDist, YDist, ZDist);

        // 첫 번째 요소와 마지막 요소 사이에 동일한 거리를 가진 자식 오브젝트들을 배치
        for (int i = 1; i < Children.Length; i++)
        {
            // 이전 요소의 위치에 동일한 거리를 더하여 현재 요소의 위치를 설정
            Children[i].transform.position = Children[i - 1].transform.position + Dist;
        }
    }
}
