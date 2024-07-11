using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DragPackOpening : DraggingActions
{
    private bool canceling = false;
    private bool movingToOpeningSpot = false;
    private Vector3 savedPosition;

    public override bool CanDrag
    {
        get
        {
            return ShopManager.instance.OpeningArea.AllowedToDragAPack && !canceling && !movingToOpeningSpot;
        }
    }
    public override void OnStartDrag()
    {
        savedPosition = transform.localPosition;
        ShopManager.instance.OpeningArea.AllowedToDragAPack = false;
    }
    public override void OnDraggingInUpdate()
    {

    }

    public override void OnEndDrag()
    {
        if (DragSuccessful())
        {
            transform.DOMove(ShopManager.instance.OpeningArea.transform.position, 0.5f).OnComplete(() =>
            {
                GetComponent<ScriptToOpenOnePack>().AllowToOpenThisPack();
            });
        }
        else
        {
            OnCancelDrag();
        }
    }


    public override void OnCancelDrag()
    {
        canceling = true;
        transform.DOLocalMove(savedPosition, 1f).OnComplete(() =>
        {
            canceling = false;
            ShopManager.instance.OpeningArea.AllowedToDragAPack = true;
        });
    }
    protected override bool DragSuccessful()
    {
        return ShopManager.instance.OpeningArea.CursorOverArea();
    }
}
