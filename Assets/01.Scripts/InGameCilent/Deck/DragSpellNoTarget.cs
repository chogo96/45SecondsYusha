using UnityEngine;
using System.Collections;
using DG.Tweening;
public class DragSpellNoTarget : DraggingActions
{
    private int savedHandSlot;
    private WhereIsTheCardOrCreature whereIsCard;
    private OneCardManager manager;
    private bool hasEndedDrag = false;  // 중복 호출 방지 플래그
    private Coroutine endDragCoroutine; // 드래그 끝 코루틴 저장

    public override bool CanDrag
    {
        get
        {
            // TODO : include full field check
            return base.CanDrag && !DragManager.instance.isDraggingCard;
        }
    }

    void Awake()
    {
        whereIsCard = GetComponent<WhereIsTheCardOrCreature>();
        manager = GetComponent<OneCardManager>();
    }

    public override void OnStartDrag()
    {
        if (DragManager.instance.isDraggingCard)
        {
            OnCancelDrag(); // 기존 드래그 취소
            return;
        }

        if (endDragCoroutine != null)
        {
            StopCoroutine(endDragCoroutine); // 드래그 중 다시 시작하면 이전 드래그 취소
            OnCancelDrag();
        }

        savedHandSlot = whereIsCard.Slot;
        whereIsCard.VisualState = VisualStates.Dragging;
        whereIsCard.BringToFront();
        hasEndedDrag = false;  // 드래그 시작 시 플래그 초기화
        DragManager.instance.isDraggingCard = true; // 드래그 시작 플래그 설정
    }

    public override void OnDraggingInUpdate()
    {
        // 여기에 드래그 중일 때의 로직을 추가할 수 있습니다.
    }

    public override void OnEndDrag()
    {
        if (hasEndedDrag) return;  // 이미 드래그가 끝난 경우 리턴
        hasEndedDrag = true;  // 드래그 끝 플래그 설정

        // 드래그가 성공적으로 끝났을 때 카드를 사용
        if (DragSuccessful())
        {
            // play this card
            playerOwner.PlayACardFromHand(GetComponent<IDHolder>().UniqueID, -1);
            playerOwner.PArea.handVisual.PlayASpellFromHand(GetComponent<IDHolder>().UniqueID);
        }
        else
        {
            OnCancelDrag();
        }

        endDragCoroutine = StartCoroutine(WaitAndEndDrag());
    }

    private IEnumerator WaitAndEndDrag()
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 대기
        DragManager.instance.isDraggingCard = false; // 드래그 종료 플래그 해제
    }

    public override void OnCancelDrag()
    {
        // Set old sorting order 
        whereIsCard.Slot = savedHandSlot;
        if (tag.Contains("Low"))
            whereIsCard.VisualState = VisualStates.LowHand;
        else
            whereIsCard.VisualState = VisualStates.TopHand;

        // Move this card back to its slot position
        HandVisual playerHand = playerOwner.PArea.handVisual;
        Vector3 oldCardPos = playerHand.GetCardPositionAtIndex(savedHandSlot);
        transform.DOLocalMove(oldCardPos, 1f);

        DragManager.instance.isDraggingCard = false; // 드래그 취소 시 플래그 해제
    }

    protected override bool DragSuccessful()
    {
        // TableVisual을 사용하지 않으므로 항상 드래그가 성공한 것으로 간주
        return true;
    }

    protected override PlayerScripts playerOwner
    {
        get
        {
            if (tag.Contains("1Card"))
            {
                return GlobalSettings.instance?.LowPlayer;
            }
            else if (tag.Contains("2Card"))
            {
                return GlobalSettings.instance?.TopPlayer;
            }
            else
            {
                return null;
            }
        }
    }
}