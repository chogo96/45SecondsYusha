using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 이 클래스는 게임 오브젝트를 드래그 드롭 할 수 있는 클래스임
/// 다른 
/// </summary>
public class Draggable : MonoBehaviour
{
    /// <summary>
    /// 지금 우리가 이 게임오브젝트를 드래그 하고 있는지 확인 하기 위한 변수
    /// </summary>
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
            _dragging = false;
            // turn all previews back on
            HoverPreview.PreviewsAllowed = true;
            _draggingThis = null;
            _draggingActions.OnEndDrag();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_dragging)
        {
            Vector3 mousePos = MouseInWorldCoords();
            //Debug.Log(mousePos);
            transform.position = new Vector3(mousePos.x - _privateDisplacement.x, mousePos.y - _privateDisplacement.y, transform.position.z);
            _draggingActions.OnDraggingInUpdate();
        }
    }

    void OnMouseUp()
    {
        if (_dragging && HowToEnd == EndDragBehavior.OnMouseUp)
        {
            _dragging = false;
            // turn all previews back on
            HoverPreview.PreviewsAllowed = true;
            _draggingThis = null;
            _draggingActions.OnEndDrag();
        }
    }

    public void StartDragging()
    {
        _dragging = true;
        // when we are dragging something, all previews should be off
        HoverPreview.PreviewsAllowed = false;
        _draggingThis = this;
        _draggingActions.OnStartDrag();
        _zDisplacement = -Camera.main.transform.position.z + transform.position.z;
        _privateDisplacement = -transform.position + MouseInWorldCoords();
    }

    public void CancelDrag()
    {
        if (_dragging)
        {
            _dragging = false;
            // turn all previews back on
            HoverPreview.PreviewsAllowed = true;
            _draggingThis = null;
            _draggingActions.OnCancelDrag();
        }
    }

    // returns mouse position in World coordinates for our GameObject to follow. 
    private Vector3 MouseInWorldCoords()
    {
        var screenMousePos = Input.mousePosition;
        //Debug.Log(screenMousePos);
        screenMousePos.z = _zDisplacement;
        return Camera.main.ScreenToWorldPoint(screenMousePos);
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
