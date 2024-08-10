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
        _initialScale = new Vector3(90.0f, 90.0f, 30f); // ���ϴ� �ʱ� ũ��� ����
        firebaseCardManager = FindObjectOfType<FirebaseCardManager>();
    }

    //public void SetCardAsset(CardAsset asset)
    //{
    //    cardAsset = asset;
    //}

    public void SetCardAsset(CardAsset asset)
    {
        cardAsset = asset;
        // ī�� ������ �����Ǹ� �ٷ� ���� ������Ʈ
        if (cardAsset != null)
        {
            int count = firebaseCardManager.GetCardCount(LoginManager.UserId, cardAsset);
            UpdateQuantity(count);
        }
        else
        {
            Debug.LogError("SetCardAsset: cardAsset�� null�Դϴ�.");
        }
    }

    void OnMouseDown()
    {
        CardAsset asset = GetComponent<OneCardManager>().cardAsset;
        if (asset == null)
            return;

        Debug.Log("if�� ������.");

        // ��ųʸ��� Ű�� �����ϴ��� Ȯ��
        if (firebaseCardManager.viewCardss.ContainsKey(cardAsset))
        {
            int currentDeckCount = DeckBuildingScreen.instance.BuilderScript.NumberOfThisCardInDeck(cardAsset);
            int availableCount = firebaseCardManager.viewCardss[cardAsset];

            if (availableCount - currentDeckCount > 0)
            {
                if (currentDeckCount < 3) // �ִ� 3������� ���� �߰�
                {
                    Debug.Log("Ŭ�� Ȱ��ȭ. �߰�");
                    DeckBuildingScreen.instance.BuilderScript.AddCard(asset);
                    UpdateQuantity(firebaseCardManager.viewCardss[cardAsset]);
                }
                else
                {
                    Debug.Log("ī�� ������ �̹� �ִ�ġ�Դϴ�.");
                    // �ִ� ī�� ���� �˸�
                }
            }
            else
            {
                Debug.Log("ī�尡 ������� �ʽ��ϴ�.");
                // ī�尡 ������� ������ �˷��ݽô�.
            }
        }
        else
        {
            Debug.LogError("viewCardss ��ųʸ��� �ش� ī�尡 �����ϴ�: " + cardAsset.CardScriptName);
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
    /// ��Ŭ�� �� ī�尡 ���� �� ���߿� ������ ����
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
    //        Debug.LogError("cardAsset�� �������� �ʾҽ��ϴ�: " + name);
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
            Debug.LogError("UpdateQuantity: cardAsset�� �������� �ʾҽ��ϴ�: " + name);
        }
    }

}
