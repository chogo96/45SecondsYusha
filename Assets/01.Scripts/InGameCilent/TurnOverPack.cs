using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class TurnOverPack : MonoBehaviour
{
    public Image Glow;

    private float _initialScale;
    private float _scaleFactor = 1.1f;
    private OneCardManager _oneCardManager;

    private void Awake()
    {
        _initialScale = transform.localScale.x;
        _oneCardManager = new OneCardManager();
    }
    private void OnMouseDown()
    {
        transform.DORotate(Vector3.zero, 0.5f);
        ShopManager.instance.OpeningArea.NumberOfCardsOpenedFromPack++;
    }
    private void OnMouseEnter()
    {
        transform.DOScale(_initialScale*_scaleFactor,0.5f);
        Glow.DOColor(ShopManager.instance.OpeningArea.GlowColorsByRarity[_oneCardManager.cardAsset.Rarity], 0.5f);
    }
    private void OnMouseExit()
    {
        transform.DOScale(_initialScale, 0.5f);
        Glow.DOColor(Color.clear, 0.5f);
    }
}
