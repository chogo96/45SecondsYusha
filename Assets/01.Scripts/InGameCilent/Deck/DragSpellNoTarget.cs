using UnityEngine;
using System.Collections;
using DG.Tweening;
public class DragSpellNoTarget : DraggingActions
{
    private int savedHandSlot;
    private WhereIsTheCardOrCreature whereIsCard;
    private OneCardManager manager;
    private bool hasEndedDrag = false;  // �ߺ� ȣ�� ���� �÷���

    public override bool CanDrag
    {
        get
        {

            // TODO : include full field check
            return base.CanDrag;
        }
    }

    void Awake()
    {
        whereIsCard = GetComponent<WhereIsTheCardOrCreature>();
        manager = GetComponent<OneCardManager>();
    }

    public override void OnStartDrag()
    {
        savedHandSlot = whereIsCard.Slot;
        whereIsCard.VisualState = VisualStates.Dragging;
        whereIsCard.BringToFront();
        hasEndedDrag = false;  // �巡�� ���� �� �÷��� �ʱ�ȭ
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
        HandVisual PlayerHand = playerOwner.PArea.handVisual;
        Vector3 oldCardPos = PlayerHand.slots.Children[savedHandSlot].transform.localPosition;
        transform.DOLocalMove(oldCardPos, 1f);
    }

    protected override bool DragSuccessful()
    {
        // TableVisual�� ������� �����Ƿ� �׻� �巡�װ� ������ ������ ����
        return true;
    }
}
