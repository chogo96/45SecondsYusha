using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
public abstract class DraggingActions : MonoBehaviour
{
    public abstract void OnStartDrag();

    public abstract void OnEndDrag();

    public abstract void OnDraggingInUpdate();

    public abstract void OnCancelDrag();

    protected abstract bool DragSuccessful();
    public virtual bool CanDrag
    {
        get
        {
            return GlobalSettings.instance.CanControlThisPlayer(playerOwner);
        }
    }
    protected virtual Player playerOwner
    {
        get
        {
            if (tag.Contains("Low"))
            {
                return GlobalSettings.instance.LowPlayer;
            }
            else if (tag.Contains("Top"))
            {
                return GlobalSettings.instance.TopPlayer;
            }
            else
            {
                Debug.LogError("태그가 없는 카드 입니다." + transform.parent.name);
                return null;
            }
        }
    }
}
