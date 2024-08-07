using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static SoundManager;

/// <summary>
/// 이 클래스는 게임 오브젝트를 드래그 드롭 할 수 있는 클래스임
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

        // 카메라 회전을 고려하여 월드 좌표를 계산
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(screenMousePos);

        // 카메라의 회전 값을 고려하여 변환된 월드 좌표를 반환
        return AdjustForCameraRotation(worldMousePos);
    }

    private Vector3 AdjustForCameraRotation(Vector3 worldPoint)
    {
        // 카메라의 로컬 좌표계를 사용하여 회전값을 보정
        Vector3 cameraRight = Camera.main.transform.right;
        Vector3 cameraUp = Camera.main.transform.up;

        // 카메라 회전 값을 고려하여 보정된 좌표 계산
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
