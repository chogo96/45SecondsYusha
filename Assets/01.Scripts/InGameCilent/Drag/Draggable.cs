using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static SoundManager;

/// <summary>
/// �� Ŭ������ ���� ������Ʈ�� �巡�� ��� �� �� �ִ� Ŭ������
/// </summary>
public class Draggable : MonoBehaviour
{
    private bool _dragging = false;
    private Vector3 _privateDisplacement;
    private float _zDisplacement;
    private DraggingActions _draggingActions;
    private static Draggable _draggingThis;

    public static Draggable DraggingThis
    {
        get { return _draggingThis; }
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
        _draggingThis = null;
        _draggingActions.OnEndDrag();
    }

    public void StartDragging()
    {
        _dragging = true;
        HoverPreview.PreviewsAllowed = false;
        _draggingThis = this;
        _draggingActions.OnStartDrag();
        _zDisplacement = -Camera.main.transform.position.z + transform.position.z;
        _privateDisplacement = -transform.position + MouseInWorldCoords();
        SoundManager.instance.PlaySfx(Sfx.CardUse);
    }

    public void CancelDrag()
    {
        if (_dragging)
        {
            _dragging = false;
            HoverPreview.PreviewsAllowed = true;
            _draggingThis = null;
            _draggingActions.OnCancelDrag();
        }
    }

    private Vector3 MouseInWorldCoords()
    {
        var screenMousePos = Input.mousePosition;
        screenMousePos.z = _zDisplacement;

        // ī�޶� ȸ���� ����Ͽ� ���� ��ǥ�� ���
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(screenMousePos);

        // ī�޶��� ȸ�� ���� ����Ͽ� ��ȯ�� ���� ��ǥ�� ��ȯ
        return AdjustForCameraRotation(worldMousePos);
    }

    private Vector3 AdjustForCameraRotation(Vector3 worldPoint)
    {
        // ī�޶��� ���� ��ǥ�踦 ����Ͽ� ȸ������ ����
        Vector3 cameraRight = Camera.main.transform.right;
        Vector3 cameraUp = Camera.main.transform.up;

        // ī�޶� ȸ�� ���� ����Ͽ� ������ ��ǥ ���
        Vector3 adjustedPoint = Camera.main.transform.position +
                                (cameraRight * (worldPoint.x - Camera.main.transform.position.x)) +
                                (cameraUp * (worldPoint.y - Camera.main.transform.position.y)) +
                                (Vector3.forward * (worldPoint.z - Camera.main.transform.position.z));

        return adjustedPoint;
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
