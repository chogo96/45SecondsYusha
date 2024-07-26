using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class AddCardToDeck : MonoBehaviour
{

    public TMP_Text QuantityText;
    private Vector3 _initialScale;
    private float scaleFactor = 1.5f;
    private CardAsset cardAsset;
    FirebaseCardManager firebaseCardManager;

    void Awake()
    {
        _initialScale = new Vector3(60.0f, 60.0f, 60.0f); // ���ϴ� �ʱ� ũ��� ����
        firebaseCardManager = FindObjectOfType<FirebaseCardManager>();
    }

    public void SetCardAsset(CardAsset asset) { cardAsset = asset; }

    void OnMouseDown()
    {
        CardAsset asset = GetComponent<OneCardManager>().cardAsset;
        if (asset == null)
            return;

        Debug.Log("if�� ������.");

        if (/*CardCollection.instance.QuantityOfEachCard[cardAsset]*/firebaseCardManager.viewCardss[cardAsset] - DeckBuildingScreen.instance.BuilderScript.NumberOfThisCardInDeck(cardAsset) >= 0)
        {
            Debug.Log("Ŭ�� Ȱ��ȭ. �߰�");
            DeckBuildingScreen.instance.BuilderScript.AddCard(asset);
            UpdateQuantity();
        }
        else
        {
            Debug.Log("ī����� ����.");

            // ī�尡 ������� ������ �˷��ݽô�.
        }
    }

    void OnMouseEnter()
    {
        //if (CraftingScreen.instance.Visible)
        //    return;

        transform.DOScale(_initialScale * scaleFactor, 0.5f);
    }

    void OnMouseExit()
    {
        transform.DOScale(_initialScale, 0.5f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            OnRightClick();
    }

    /// <summary>
    /// ��Ŭ�� �� ī�尡 ���� �� ���߿� ������ ����
    /// </summary>
    void OnRightClick()
    {
        //if (CraftingScreen.instance.Visible)
        //{
        //    return;
        //}


        Ray clickPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitPoint;

        if (Physics.Raycast(clickPoint, out hitPoint))
        {
            if (hitPoint.collider == this.GetComponent<Collider>())
            {
                Debug.Log("Right Clicked on " + this.name);
                //CraftingScreen.instance.ShowCraftingScreen(GetComponent<OneCardManager>().cardAsset);
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
