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
            return true;
        }
    }
    protected virtual PlayerScripts playerOwner
    {
        get
        {
            if (tag.Contains("1Card"))
            {
                return GlobalSettings.instance.LowPlayer;
            }
            else if (tag.Contains("2Card"))
            {
                return GlobalSettings.instance.TopPlayer;
            }
            else
            {
                return null;
            }
        }
    }
}
