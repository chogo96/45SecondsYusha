using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragSpellNoTarget : DraggingActions
{
    private int savedHandSlot;
    private WhereIsTheCardOrCreature whereIsCard;
    private OneCardManager manager;

    public override bool CanDrag
    {
        get
        {
            // TEST LINE: this is just to test playing creatures before the game is complete 
            // return true;

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
    }

    public override void OnDraggingInUpdate()
    {
        // ���⿡ �巡�� ���� ���� ������ �߰��� �� �ֽ��ϴ�.
    }

    public override void OnEndDrag()
    {
        // �巡�װ� ���������� ������ �� ī�带 ����ϰ� �Ҹ� �Ǵ� ��� ������ �̵�
        if (DragSuccessful())
        {
            // play this card
            playerOwner.PlayACardFromHand(GetComponent<IDHolder>().UniqueID, -1);
            playerOwner.PArea.handVisual.PlayASpellFromHand(GetComponent<IDHolder>().UniqueID);

            // ���� ī�� ó��
            CardLogic cardLogic;
            if (CardLogic.CardsCreatedThisGame.TryGetValue(GetComponent<IDHolder>().UniqueID, out cardLogic))
            {
                if (cardLogic.cardAsset.IsVanishCard)
                {
                    // ī�尡 �Ҹ��ϴ� ���
                    VanishCard(cardLogic.cardAsset);
                }
                else
                {
                    // ī�尡 �Ҹ����� �ʴ� ���
                    DiscardCard(cardLogic.cardAsset);
                }
            }
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

    private void VanishCard(CardAsset cardAsset)
    {
        playerOwner.deck.VanishDeck.Add(cardAsset);
        Debug.Log($"Card {cardAsset.name} vanished.");
    }

    private void DiscardCard(CardAsset cardAsset)
    {
        playerOwner.deck.DiscardDeck.Add(cardAsset);
        Debug.Log($"Card {cardAsset.name} discarded.");
    }
}
