using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class DragSpellOnTarget : DraggingActions
{
    // 타겟팅 옵션 (모든 것, 적, 자신 등)
    public TargetingOptions Targets = TargetingOptions.All;
    private SpriteRenderer sr; // 카드의 스프라이트 렌더러
    private LineRenderer lr; // 드래그 중 보여줄 선 렌더러 (화살표)
    private WhereIsTheCardOrCreature whereIsThisCard; // 카드의 현재 위치 상태를 관리하는 컴포넌트
    private VisualStates tempVisualState; // 드래그 시작 전의 시각적 상태를 저장하기 위한 변수
    private Transform triangle; // 화살표 끝 부분의 삼각형
    private SpriteRenderer triangleSR; // 삼각형의 스프라이트 렌더러
    private GameObject Target; // 드래그한 마법의 타겟이 될 오브젝트
    private OneCardManager manager; // 카드 관리 매니저

    private CurvedLinePoint[] linePoints = new CurvedLinePoint[0]; // 곡선 라인의 포인트들
    private Vector3[] linePositions = new Vector3[0]; // 라인의 위치들
    private Vector3[] linePositionsOld = new Vector3[0]; // 이전 라인의 위치들

    // 드래그 가능한지 여부를 설정하는 속성 (항상 true로 설정)
    public override bool CanDrag
    {
        get
        {
            return true;
        }
    }

    void Awake()
    {
        // 컴포넌트들을 초기화
        sr = GetComponent<SpriteRenderer>();
        lr = GetComponentInChildren<LineRenderer>();
        lr.sortingLayerName = "AboveEverything"; // 선을 모든 것 위에 표시
        triangle = transform.Find("Triangle");
        triangleSR = triangle.GetComponent<SpriteRenderer>();

        manager = GetComponentInParent<OneCardManager>();
        whereIsThisCard = GetComponentInParent<WhereIsTheCardOrCreature>();
    }

    // 드래그 시작 시 호출되는 함수
    public override void OnStartDrag()
    {
        tempVisualState = whereIsThisCard.VisualState; // 이전 상태 저장
        whereIsThisCard.VisualState = VisualStates.Dragging; // 상태를 드래그 중으로 변경

        sr.enabled = true; // 카드 스프라이트를 활성화
        lr.enabled = true; // 선 렌더러를 활성화

        whereIsThisCard.SetHandSortingOrder(); // 카드의 정렬 순서를 설정
    }

    // 드래그 중일 때 매 프레임 호출되는 함수
    public override void OnDraggingInUpdate()
    {
        Vector3 notNormalized = transform.position - transform.parent.position; // 카드와 부모 오브젝트 간의 벡터 계산
        Vector3 direction = notNormalized.normalized; // 방향 벡터 계산
        float distanceToTarget = (direction * 2.3f).magnitude; // 타겟까지의 거리 계산

        if (notNormalized.magnitude > distanceToTarget)
        {
            linePoints = lr.gameObject.GetComponentsInChildren<CurvedLinePoint>(); // 곡선 라인의 포인트들 가져오기

            // 위치 설정
            Vector3 midPoint = Vector3.Lerp(transform.parent.position, transform.position - direction * 2.3f, 0.5f); // 중간 지점 계산
            linePositions = new Vector3[linePoints.Length]; // 라인 포지션 배열 생성

            midPoint += new Vector3(0, 0, -lr.positionCount * 0.1f); // 중간 지점의 Z 값 보정
            if (midPoint.z > 0)
                midPoint.z = 0;
            if (midPoint.z < -10f)
                midPoint.z = -10f;

            if (lr.positionCount < 2)
                midPoint.z = 0;

            // 라인 포인트들의 위치 설정
            linePoints[0].transform.position = transform.parent.position;
            linePoints[1].transform.position = midPoint;
            linePoints[2].transform.position = transform.position - direction * 1.5f;

            linePositions[0] = linePoints[0].transform.position;
            linePositions[1] = linePoints[1].transform.position;
            linePositions[2] = linePoints[2].transform.position;

            // 이전 라인 포지션과 크기가 맞지 않으면 새로 설정
            if (linePositionsOld.Length != linePositions.Length)
            {
                linePositionsOld = new Vector3[linePositions.Length];
            }

            // 부드러운 라인 계산
            Vector3[] smoothedPoints = LineSmoother.SmoothLine(linePositions, 2);

            // 라인 렌더러 설정
            lr.positionCount = smoothedPoints.Length;
            lr.SetPositions(smoothedPoints);

            lr.enabled = true; // 라인 렌더러 활성화

            // 화살표 끝 부분의 삼각형 위치 및 회전 설정
            triangleSR.enabled = true;
            triangleSR.transform.position = transform.position - 1.35f * direction;

            float rot_z = Mathf.Atan2(notNormalized.y, notNormalized.x) * Mathf.Rad2Deg;
            triangleSR.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }
        else
        {
            // 타겟이 너무 가깝다면 화살표를 비활성화
            lr.enabled = false;
            triangleSR.enabled = false;
        }

    }

    // 드래그 종료 시 호출되는 함수
    public override void OnEndDrag()
    {
        HandVisual PlayerHand = playerOwner.PArea.handVisual; // 플레이어의 손 비주얼 관리
        Target = null;
        RaycastHit[] hits;

        // 레이캐스트 실행, 타겟을 검색
        hits = Physics.RaycastAll(origin: Camera.main.transform.position,
            direction: (-Camera.main.transform.position + this.transform.position).normalized,
            maxDistance: 30f);

        foreach (RaycastHit h in hits)
        {
            if (h.transform.tag.Contains("Player"))
            {
                // 플레이어를 선택함
                Target = h.transform.gameObject;
            }
            else if (h.transform.tag.Contains("Creature"))
            {
                // 생물을 선택함
                Target = h.transform.parent.gameObject;
            }
        }

        bool targetValid = false;

        if (Target != null)
        {
            // 타겟팅 옵션에 따라 마법을 사용할지 확인
            int targetID = Target.GetComponent<IDHolder>().UniqueID;
            switch (Targets)
            {
                case TargetingOptions.All:
                    targetValid = true;
                    playerOwner.PArea.handVisual.PlayASpellFromHand(GetComponentInParent<IDHolder>().UniqueID);
                    break;
                case TargetingOptions.Enemy:
                    if (Target.tag.Contains("적"))
                    {
                        targetValid = true;
                        playerOwner.PArea.handVisual.PlayASpellFromHand(GetComponentInParent<IDHolder>().UniqueID);
                    }
                    break;
                case TargetingOptions.Myself:
                    if (Target.tag.Contains("Creature") || Target.tag.Contains("Player"))
                    {
                        if ((tag.Contains("Low") && Target.tag.Contains("Top"))
                            || (tag.Contains("Top") && Target.tag.Contains("Low")))
                        {
                            playerOwner.PlayACardFromHand(GetComponentInParent<IDHolder>().UniqueID, targetID);
                            targetValid = true;
                            playerOwner.PArea.handVisual.PlayASpellFromHand(GetComponentInParent<IDHolder>().UniqueID);
                        }
                    }
                    break;
                case TargetingOptions.Other:
                    if (Target.tag.Contains("Creature"))
                    {
                        if ((tag.Contains("Low") && Target.tag.Contains("Top"))
                            || (tag.Contains("Top") && Target.tag.Contains("Low")))
                        {
                            playerOwner.PlayACardFromHand(GetComponentInParent<IDHolder>().UniqueID, targetID);
                            targetValid = true;
                            playerOwner.PArea.handVisual.PlayASpellFromHand(GetComponentInParent<IDHolder>().UniqueID);
                        }
                    }
                    break;
                default:
                    Utils.LogRed("DragSpellOnTarget에서 기본 케이스에 도달했습니다! 의심스러운 동작!!");
                    break;
            }
        }

        if (!targetValid)
        {
            // 타겟이 유효하지 않다면 카드 상태를 복원
            whereIsThisCard.VisualState = tempVisualState;
            whereIsThisCard.SetHandSortingOrder();

            // 카드를 원래 슬롯으로 이동
            PlayerHand.PlaceCardsOnNewSlots();
        }

        // 화살표와 타겟을 원래 위치로 복원
        transform.localPosition = new Vector3(0f, 0f, -1f);
        sr.enabled = false;
        lr.enabled = false;
        triangleSR.enabled = false;
    }

    // 사용되지 않는 함수
    protected override bool DragSuccessful()
    {
        return true;
    }

    public override void OnCancelDrag()
    {

    }
}
