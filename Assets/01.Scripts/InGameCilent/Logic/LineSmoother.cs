using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineSmoother : MonoBehaviour
{
    // 주어진 포인트 배열(inputPoints)을 받아서, 부드러운 선을 만들어 반환하는 함수
    // segmentSize는 각 선분의 크기를 지정합니다.
    public static Vector3[] SmoothLine(Vector3[] inputPoints, float segmentSize)
    {
        // 각 좌표(X, Y, Z)에 대한 곡선을 생성
        AnimationCurve curveX = new AnimationCurve();
        AnimationCurve curveY = new AnimationCurve();
        AnimationCurve curveZ = new AnimationCurve();

        // 각 좌표에 대한 키프레임 배열을 생성
        Keyframe[] keysX = new Keyframe[inputPoints.Length];
        Keyframe[] keysY = new Keyframe[inputPoints.Length];
        Keyframe[] keysZ = new Keyframe[inputPoints.Length];

        // 키프레임 설정
        for (int i = 0; i < inputPoints.Length; i++)
        {
            keysX[i] = new Keyframe(i, inputPoints[i].x);
            keysY[i] = new Keyframe(i, inputPoints[i].y);
            keysZ[i] = new Keyframe(i, inputPoints[i].z);
        }

        // 키프레임을 곡선에 적용
        curveX.keys = keysX;
        curveY.keys = keysY;
        curveZ.keys = keysZ;

        // 곡선의 탄젠트를 부드럽게 설정
        for (int i = 0; i < inputPoints.Length; i++)
        {
            curveX.SmoothTangents(i, 0);
            curveY.SmoothTangents(i, 0);
            curveZ.SmoothTangents(i, 0);
        }

        // 부드러운 값을 기록할 리스트 생성
        List<Vector3> lineSegments = new List<Vector3>();

        // 각 섹션에서 선분을 찾음
        for (int i = 0; i < inputPoints.Length; i++)
        {
            // 첫 번째 포인트를 리스트에 추가
            lineSegments.Add(inputPoints[i]);

            // 배열의 범위를 벗어나지 않도록 체크
            if (i + 1 < inputPoints.Length)
            {
                // 다음 포인트까지의 거리 계산
                float distanceToNext = Vector3.Distance(inputPoints[i], inputPoints[i + 1]);

                // 해당 구간의 선분 개수 계산
                int segments = (int)(distanceToNext / segmentSize);

                // 각 선분을 리스트에 추가
                for (int s = 1; s < segments; s++)
                {
                    // 곡선에서 보간된 시간을 계산
                    float time = ((float)s / (float)segments) + (float)i;

                    // 곡선을 샘플링하여 부드러운 위치 계산
                    Vector3 newSegment = new Vector3(curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time));

                    // 새 선분을 리스트에 추가
                    lineSegments.Add(newSegment);
                }
            }
        }

        // 부드러운 선분 리스트를 배열로 변환하여 반환
        return lineSegments.ToArray();
    }
}
