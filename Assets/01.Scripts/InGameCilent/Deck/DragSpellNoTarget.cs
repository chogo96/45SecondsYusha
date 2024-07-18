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
        // 여기에 드래그 중일 때의 로직을 추가할 수 있습니다.
    }

    public override void OnEndDrag()
    {
        // 드래그가 성공적으로 끝났을 때 카드를 사용하고 소멸 또는 폐기 덱으로 이동
        if (DragSuccessful())
        {
            // play this card
            playerOwner.PlayACardFromHand(GetComponent<IDHolder>().UniqueID, -1);
            playerOwner.PArea.handVisual.PlayASpellFromHand(GetComponent<IDHolder>().UniqueID);

            // 사용된 카드 처리
            CardLogic cardLogic;
            if (CardLogic.CardsCreatedThisGame.TryGetValue(GetComponent<IDHolder>().UniqueID, out cardLogic))
            {
                if (cardLogic.cardAsset.IsVanishCard)
                {
                    // 카드가 소멸하는 경우
                    VanishCard(cardLogic.cardAsset);
                }
                else
                {
                    // 카드가 소멸하지 않는 경우
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
        // TableVisual을 사용하지 않으므로 항상 드래그가 성공한 것으로 간주
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
