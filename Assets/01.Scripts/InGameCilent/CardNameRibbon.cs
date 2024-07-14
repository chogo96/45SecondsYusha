using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardNameRibbon : MonoBehaviour
{

    public TMP_Text NameText;
    public TMP_Text QuantityText;
    public Image RibbonImage;

    public CardAsset Asset { get; set; }
    public int Quantity { get; set; }

    public void ApplyAsset(CardAsset cardAsset, int quantity)
    {
        if (cardAsset.CharacterAsset != null)
            RibbonImage.color = cardAsset.CharacterAsset.ClassCardTint;

        Asset = cardAsset;

        NameText.text = cardAsset.name;
        SetQuantity(quantity);
    }

    public void SetQuantity(int quantity)
    {
        if (quantity == 0)
            return;

        QuantityText.text = "X" + quantity.ToString();
        Quantity = quantity;
    }

    public void ReduceQuantity()
    {
        Debug.Log("In reduce Quantity");
        DeckBuildingScreen.instance.BuilderScript.RemoveCard(Asset);
    }
}
