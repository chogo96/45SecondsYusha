using UnityEngine;
using System.Collections;
using DG.Tweening;
public class DragSpellNoTarget : DraggingActions
{
    private int savedHandSlot;
    private WhereIsTheCardOrCreature whereIsCard;
    private OneCardManager manager;
    private bool hasEndedDrag = false;  // �ߺ� ȣ�� ���� �÷���
    private Coroutine endDragCoroutine; // �巡�� �� �ڷ�ƾ ����

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
            OnCancelDrag(); // ���� �巡�� ���
            return;
        }

        if (endDragCoroutine != null)
        {
            StopCoroutine(endDragCoroutine); // �巡�� �� �ٽ� �����ϸ� ���� �巡�� ���
            OnCancelDrag();
        }

        savedHandSlot = whereIsCard.Slot;
        whereIsCard.VisualState = VisualStates.Dragging;
        whereIsCard.BringToFront();
        hasEndedDrag = false;  // �巡�� ���� �� �÷��� �ʱ�ȭ
        DragManager.instance.isDraggingCard = true; // �巡�� ���� �÷��� ����
    }

    public override void OnDraggingInUpdate()
    {
        // ���⿡ �巡�� ���� ���� ������ �߰��� �� �ֽ��ϴ�.
    }

    public override void OnEndDrag()
    {
        if (hasEndedDrag) return;  // �̹� �巡�װ� ���� ��� ����
        hasEndedDrag = true;  // �巡�� �� �÷��� ����

        // �巡�װ� ���������� ������ �� ī�带 ���
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
        yield return new WaitForSeconds(0.5f); // 0.5�� ���
        DragManager.instance.isDraggingCard = false; // �巡�� ���� �÷��� ����
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

        DragManager.instance.isDraggingCard = false; // �巡�� ��� �� �÷��� ����
    }

    protected override bool DragSuccessful()
    {
        // TableVisual�� ������� �����Ƿ� �׻� �巡�װ� ������ ������ ����
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