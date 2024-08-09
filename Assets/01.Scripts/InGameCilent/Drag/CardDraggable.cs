using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static SoundManager;

public class CardDraggable : MonoBehaviour
{
    private bool _dragging = false;
    private Vector3 _privateDisplacement;
    private float _zDisplacement;
    private DraggingActions _draggingActions;
    private static CardDraggable _cardDraggingThis;

    public static CardDraggable DraggingThis
    {
        get { return _cardDraggingThis; }
    }

    private void Awake()
    {
        _draggingActions = GetComponent<DraggingActions>();
    }

    void OnMouseDown()
    {
        if (_draggingActions != null && _draggingActions.CanDrag && HowToStart == StartDragBehavior.OnMouseDown)
        {
            StartDragging();
        }

        if (_dragging && HowToEnd == EndDragBehavior.OnMouseDown)
        {
            EndDragging();
        }
    }

    void Update()
    {
        if (_dragging)
        {
            Vector3 mousePos = MouseInWorldCoords();
            transform.position = new Vector3(mousePos.x - _privateDisplacement.x, mousePos.y - _privateDisplacement.y, transform.position.z);
            _draggingActions.OnDraggingInUpdate();
        }
    }

    void OnMouseUp()
    {
        if (_dragging && HowToEnd == EndDragBehavior.OnMouseUp)
        {
            EndDragging();
        }
    }

    private void EndDragging()
    {
        _dragging = false;
        HoverPreview.PreviewsAllowed = true;
        _cardDraggingThis = null;
        _draggingActions.OnEndDrag();
        SoundManager.instance.PlaySfx(Sfx.CardUse);

        // 드래그가 끝나면 원 모양으로 변경 후 중앙으로 이동
        AnimateCardToCenter();
    }

    public void StartDragging()
    {
        _dragging = true;
        HoverPreview.PreviewsAllowed = false;
        _cardDraggingThis = this;
        _draggingActions.OnStartDrag();
        _zDisplacement = -Camera.main.transform.position.z + transform.position.z;
        _privateDisplacement = -transform.position + MouseInWorldCoords();
    }

    public void CancelDrag()
    {
        if (_dragging)
        {
            _dragging = false;
            HoverPreview.PreviewsAllowed = true;
            _cardDraggingThis = null;
            _draggingActions.OnCancelDrag();
        }
    }

    private Vector3 MouseInWorldCoords()
    {
        var screenMousePos = Input.mousePosition;
        screenMousePos.z = _zDisplacement;

        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(screenMousePos);
        return AdjustForCameraRotation(worldMousePos);
    }

    private Vector3 AdjustForCameraRotation(Vector3 worldPoint)
    {
        Vector3 cameraRight = Camera.main.transform.right;
        Vector3 cameraUp = Camera.main.transform.up;

        Vector3 adjustedPoint = Camera.main.transform.position +
                                (cameraRight * (worldPoint.x - Camera.main.transform.position.x)) +
                                (cameraUp * (worldPoint.y - Camera.main.transform.position.y)) +
                                (Vector3.forward * (worldPoint.z - Camera.main.transform.position.z));

        return adjustedPoint;
    }

    private void AnimateCardToCenter()
    {
        // 원 모양으로 변형 (간단하게 크기 조정으로 처리)
        //transform.DOScale(new Vector3(60, 60, 0), 0.5f).SetEase(Ease.InOutQuad);

        // 중앙으로 이동
        Vector3 centerPosition = new Vector3(500, 288, 381); // 화면 중앙의 월드 좌표
        transform.DOMove(centerPosition, 1f).SetEase(Ease.InOutQuad);
    }

    public enum StartDragBehavior
    {
        OnMouseDown, InAwake
    }

    public enum EndDragBehavior
    {
        OnMouseUp, OnMouseDown
    }

    public StartDragBehavior HowToStart = StartDragBehavior.OnMouseDown;
    public EndDragBehavior HowToEnd = EndDragBehavior.OnMouseUp;
}
