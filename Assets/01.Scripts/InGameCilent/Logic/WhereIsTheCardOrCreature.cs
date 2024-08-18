using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum VisualStates
{
    Transition,  // 전환 상태
    LowHand,     // 손에 낮은 위치
    TopHand,     // 손에 높은 위치
    LeftHand,    // 손에 왼쪽 위치
    RightHand,   // 손에 오른쪽 위치
    Dragging     // 드래그 중
}

public class WhereIsTheCardOrCreature : MonoBehaviour
{

    // HoverPreview 컴포넌트에 대한 참조
    private HoverPreview hover;

    // 이 오브젝트에 있는 Canvas에 대한 참조 (정렬 순서를 설정하기 위해 사용)
    private Canvas canvas;

    // 이 오브젝트를 다른 모든 것 위에 보여주고 싶을 때 사용되는 canvas 정렬 순서 값
    private int TopSortingOrder = 500;

    // 프로퍼티
    private int slot = -1;
    public int Slot
    {
        get { return slot; }

        set
        {
            slot = value;
        }
    }

    private VisualStates state;
    public VisualStates VisualState
    {
        get { return state; }

        set
        {
            state = value;
            switch (state)
            {
                case VisualStates.LowHand:
                    hover.ThisPreviewEnabled = true;
                    break;
                case VisualStates.Transition:
                    hover.ThisPreviewEnabled = false;
                    break;
                case VisualStates.Dragging:
                    hover.ThisPreviewEnabled = false;
                    break;
                case VisualStates.TopHand:
                    hover.ThisPreviewEnabled = false;
                    break;
            }
        }
    }

    void Awake()
    {
        // HoverPreview 컴포넌트 가져오기
        hover = GetComponent<HoverPreview>();
        // 캐릭터의 경우 HoverPreview가 자식 게임 오브젝트에 붙어있음
        if (hover == null)
            hover = GetComponentInChildren<HoverPreview>();

        // 자식 오브젝트에서 Canvas 컴포넌트를 가져오기
        canvas = GetComponentInChildren<Canvas>();
    }

    // 이 오브젝트를 모든 것 위로 보내기 위한 함수
    public void BringToFront()
    {
        canvas.sortingOrder = TopSortingOrder;
        canvas.sortingLayerName = "AboveEverything";
        //canvas.transform.localPosition = new Vector3(0f, 0f, -1f);
    }

    // 시각적 상태(VisualState) 프로퍼티 내부에서 정렬 순서를 설정하지 않는 이유는
    // 카드를 뽑을 때 인덱스를 먼저 설정한 후, 카드를 손에 도착했을 때만 정렬 순서를 설정하기 위함임
    public void SetHandSortingOrder()
    {
        if (slot != -1)
            canvas.sortingOrder = HandSortingOrder(slot);
        canvas.sortingLayerName = "Cards";
    }

    // 테이블에 있을 때 정렬 순서를 설정하는 함수
    public void SetTableSortingOrder()
    {
        canvas.sortingOrder = 0;
        canvas.sortingLayerName = "Creatures";
    }

    // 손에서의 정렬 순서를 계산하는 함수
    private int HandSortingOrder(int placeInHand)
    {
        return (-(placeInHand + 1) * 10);
    }
}
