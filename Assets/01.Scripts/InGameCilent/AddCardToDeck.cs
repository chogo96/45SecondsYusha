using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AddCardToDeck : MonoBehaviour
{

    public Text QuantityText;
    private float InitialScale;
    private float scaleFactor = 1.1f;
    private CardAsset cardAsset;

    void Awake()
    {
        InitialScale = transform.localScale.x;
    }

    public void SetCardAsset(CardAsset asset) { cardAsset = asset; }

    void OnMouseDown()
    {
        CardAsset asset = GetComponent<OneCardManager>().cardAsset;
        if (asset == null)
            return;

        if (CardCollection.instance.QuantityOfEachCard[cardAsset] - DeckBuildingScreen.instance.BuilderScript.NumberOfThisCardInDeck(cardAsset) > 0)
        {
            DeckBuildingScreen.instance.BuilderScript.AddCard(asset);
            UpdateQuantity();
        }
        else
        {
            // 카드가 충분하지 않음을 알려줍시다.
        }
    }

    void OnMouseEnter()
    {
        if (CraftingScreen.instance.Visible)
            return;

        transform.DOScale(InitialScale * scaleFactor, 0.5f);
    }

    void OnMouseExit()
    {
        transform.DOScale(InitialScale, 0.5f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            OnRightClick();
    }

    /// <summary>
    /// 우클릭 시 카드가 제작 됨
    /// </summary>
    void OnRightClick()
    {
        if (CraftingScreen.instance.Visible)
        {
            return;
        }


        Ray clickPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitPoint;

        if (Physics.Raycast(clickPoint, out hitPoint))
        {
            if (hitPoint.collider == this.GetComponent<Collider>())
            {
                Debug.Log("Right Clicked on " + this.name);
                CraftingScreen.instance.ShowCraftingScreen(GetComponent<OneCardManager>().cardAsset);
            }
        }
    }

    public void UpdateQuantity()
    {
        int quantity = CardCollection.instance.QuantityOfEachCard[cardAsset];

        if (DeckBuildingScreen.instance.BuilderScript.InDeckBuildingMode && DeckBuildingScreen.instance.ShowReducedQuantities)
            quantity -= DeckBuildingScreen.instance.BuilderScript.NumberOfThisCardInDeck(cardAsset);

        QuantityText.text = "X" + quantity.ToString();

    }
}
