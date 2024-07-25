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
        _initialScale = new Vector3(60.0f, 60.0f, 60.0f); // 원하는 초기 크기로 설정
        firebaseCardManager = FindObjectOfType<FirebaseCardManager>();
    }

    public void SetCardAsset(CardAsset asset) { cardAsset = asset; }

    void OnMouseDown()
    {
        CardAsset asset = GetComponent<OneCardManager>().cardAsset;
        if (asset == null)
            return;

        Debug.Log("if문 실행전.");

        if (/*CardCollection.instance.QuantityOfEachCard[cardAsset]*/firebaseCardManager.viewCardss[cardAsset] - DeckBuildingScreen.instance.BuilderScript.NumberOfThisCardInDeck(cardAsset) >= 0)
        {
            Debug.Log("클릭 활성화. 추가");
            DeckBuildingScreen.instance.BuilderScript.AddCard(asset);
            UpdateQuantity();
        }
        else
        {
            Debug.Log("카드없음 ㅅㄱ.");

            // 카드가 충분하지 않음을 알려줍시다.
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
    /// 우클릭 시 카드가 제작 됨 나중에 구현할 예정
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
