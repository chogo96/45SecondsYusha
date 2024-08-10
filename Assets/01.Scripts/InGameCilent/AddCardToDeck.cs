using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AddCardToDeck : MonoBehaviour
{
    public Text QuantityText;
    private Vector3 _initialScale;
    private float scaleFactor = 1.1f;
    private CardAsset cardAsset;
    FirebaseCardManager firebaseCardManager;

    public CardAsset CardAsset
    {
        get { return cardAsset; }
        set { cardAsset = value; }
    }

    void Awake()
    {
        _initialScale = new Vector3(90.0f, 90.0f, 30f); // 원하는 초기 크기로 설정
        firebaseCardManager = FindObjectOfType<FirebaseCardManager>();
    }

    //public void SetCardAsset(CardAsset asset)
    //{
    //    cardAsset = asset;
    //}

    public void SetCardAsset(CardAsset asset)
    {
        cardAsset = asset;
        // 카드 에셋이 설정되면 바로 수량 업데이트
        if (cardAsset != null)
        {
            int count = firebaseCardManager.GetCardCount(LoginManager.UserId, cardAsset);
            UpdateQuantity(count);
        }
        else
        {
            Debug.LogError("SetCardAsset: cardAsset이 null입니다.");
        }
    }

    void OnMouseDown()
    {
        CardAsset asset = GetComponent<OneCardManager>().cardAsset;
        if (asset == null)
            return;

        Debug.Log("if문 실행전.");

        // 딕셔너리에 키가 존재하는지 확인
        if (firebaseCardManager.viewCardss.ContainsKey(cardAsset))
        {
            int currentDeckCount = DeckBuildingScreen.instance.BuilderScript.NumberOfThisCardInDeck(cardAsset);
            int availableCount = firebaseCardManager.viewCardss[cardAsset];

            if (availableCount - currentDeckCount > 0)
            {
                if (currentDeckCount < 3) // 최대 3장까지만 덱에 추가
                {
                    Debug.Log("클릭 활성화. 추가");
                    DeckBuildingScreen.instance.BuilderScript.AddCard(asset);
                    UpdateQuantity(firebaseCardManager.viewCardss[cardAsset]);
                }
                else
                {
                    Debug.Log("카드 수량이 이미 최대치입니다.");
                    // 최대 카드 수량 알림
                }
            }
            else
            {
                Debug.Log("카드가 충분하지 않습니다.");
                // 카드가 충분하지 않음을 알려줍시다.
            }
        }
        else
        {
            Debug.LogError("viewCardss 딕셔너리에 해당 카드가 없습니다: " + cardAsset.CardScriptName);
        }
    }

    void OnMouseEnter()
    {
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

    //public void UpdateQuantity(int count)
    //{
    //    if (cardAsset != null)
    //    {
    //        int quantity = cardAsset.Rarity == RarityOptions.Basic ? 3 : count;

    //        if (DeckBuildingScreen.instance.BuilderScript.InDeckBuildingMode && DeckBuildingScreen.instance.ShowReducedQuantities)
    //            quantity -= DeckBuildingScreen.instance.BuilderScript.NumberOfThisCardInDeck(cardAsset);

    //        QuantityText.text = "X" + quantity.ToString();
    //    }
    //    else
    //    {
    //        Debug.LogError("cardAsset이 설정되지 않았습니다: " + name);
    //    }
    //}
    public void UpdateQuantity(int count)
    {
        if (cardAsset != null)
        {
            int quantity = cardAsset.Rarity == RarityOptions.Basic ? 3 : count;

            if (DeckBuildingScreen.instance.BuilderScript.InDeckBuildingMode && DeckBuildingScreen.instance.ShowReducedQuantities)
                quantity -= DeckBuildingScreen.instance.BuilderScript.NumberOfThisCardInDeck(cardAsset);

            QuantityText.text = "X" + quantity.ToString();
        }
        else
        {
            Debug.LogError("UpdateQuantity: cardAsset이 설정되지 않았습니다: " + name);
        }
    }

}
