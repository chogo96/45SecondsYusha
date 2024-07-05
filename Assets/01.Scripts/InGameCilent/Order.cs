using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] Renderer[] _backRenderers;
    [SerializeField] Renderer[] _middleRenderers;
    [SerializeField] string _sortingLayerName;
    int originOrder;

    public void SetOriginOrder(int originOrder)
    {
        this.originOrder = originOrder;
        SetOrder(originOrder);
    }

    public void SetMostFrontOrder(bool isMostFront)
    {
        SetOrder(isMostFront ? 100 : originOrder);
    }
    //private void Start()
    //{
    //    SetOrder(0);
    //}

    public void SetOrder(int order)
    {
        int mulOrder = order * 10;

        foreach(var renderer in _backRenderers)
        {
            renderer.sortingLayerName = _sortingLayerName;
            renderer.sortingOrder = mulOrder;
        }
        foreach(var renderer in _middleRenderers)
        {
            renderer.sortingLayerName = _sortingLayerName;
            renderer.sortingOrder = mulOrder -1;
        }
    }
}
