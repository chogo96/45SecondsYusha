using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RarityTradingCost
{
    public RarityOptions Rarity;
    public int CraftCost;
    public int DisenchantOutcome;
}

public class CraftingScreen : MonoBehaviour
{

    public static CraftingScreen instance;

    public GameObject Content;

    public GameObject AttackCard;
    public GameObject SpellCard;

    public Text CraftText;
    public Text DisenchantText;
    public Text QuantityText;

    public RarityTradingCost[] TradingCostsArray;

    public bool Visible { get { return Content.activeInHierarchy; } }

    private CardAsset currentCard;
    private Dictionary<RarityOptions, RarityTradingCost> TradingCosts = new Dictionary<RarityOptions, RarityTradingCost>();

    void Awake()
    {
        instance = this;
        foreach (RarityTradingCost cost in TradingCostsArray)
            TradingCosts.Add(cost.Rarity, cost);
    }

    public void ShowCraftingScreen(CardAsset cardToShow)
    {
        currentCard = cardToShow;

        GameObject cardObject;
        if (currentCard.TypeOfCard == TypesOfCards.Attacks)
        {
            cardObject = AttackCard;
            AttackCard.SetActive(true);
            SpellCard.SetActive(false);
        }
        else
        {
            cardObject = SpellCard;
            AttackCard.SetActive(false);
            SpellCard.SetActive(true);
        }
        OneCardManager manager = cardObject.GetComponent<OneCardManager>();
        manager.cardAsset = cardToShow;
        manager.ReadCardFromAsset();

        CraftText.text = "Craft this card for " + TradingCosts[cardToShow.Rarity].CraftCost.ToString() + " dust";
        DisenchantText.text = "Disenchant to get " + TradingCosts[cardToShow.Rarity].DisenchantOutcome.ToString() + " dust";

        ShopManager.instance.DustHUD.SetActive(true);
        UpdateQuantityOfCurrentCard();
        Content.SetActive(true);
    }

    public void UpdateQuantityOfCurrentCard()
    {
        int AmountOfThisCardInYourCollection = CardCollection.instance.QuantityOfEachCard[currentCard];
        QuantityText.text = "You have " + AmountOfThisCardInYourCollection.ToString() + " of these";
        DeckBuildingScreen.instance.CollectionBrowser.UpdatePage();
    }

    public void HideCraftingScreen()
    {
        ShopManager.instance.DustHUD.SetActive(false);

        Content.SetActive(false);
    }

    public void CraftCurrentCard()
    {
        if (currentCard.Rarity != RarityOptions.Basic)
        {
            if (ShopManager.instance.Dust >= TradingCosts[currentCard.Rarity].CraftCost)
            {
                ShopManager.instance.Dust -= TradingCosts[currentCard.Rarity].CraftCost;
                CardCollection.instance.QuantityOfEachCard[currentCard]++;
                UpdateQuantityOfCurrentCard();
            }
        }
        else
        {
            // TODO: 기본 카드는 제작할 수 없음을 표시하거나 미리 제작 버튼을 비활성화합니다.
        }
    }

    public void DisenchantCurrentCard()
    {
        if (currentCard.Rarity != RarityOptions.Basic)
        {
            if (CardCollection.instance.QuantityOfEachCard[currentCard] > 0)
            {
                CardCollection.instance.QuantityOfEachCard[currentCard]--;
                ShopManager.instance.Dust += TradingCosts[currentCard.Rarity].DisenchantOutcome;
                UpdateQuantityOfCurrentCard();

                foreach (DeckInfo info in DecksStorage.instance.AllDecks)
                {
                    while (info.NumberOfThisCardInDeck(currentCard) > CardCollection.instance.QuantityOfEachCard[currentCard])
                    {
                        info.Cards.Remove(currentCard);
                    }
                }

                while (DeckBuildingScreen.instance.BuilderScript.InDeckBuildingMode &&
                       DeckBuildingScreen.instance.BuilderScript.NumberOfThisCardInDeck(currentCard) > CardCollection.instance.QuantityOfEachCard[currentCard])
                {
                    DeckBuildingScreen.instance.BuilderScript.RemoveCard(currentCard);
                }
            }
        }
        else
        {
            // TODO: 기본 카드는 분해할 수 없음을 표시하거나 미리 제작 버튼을 비활성화합니다.
        }
    }
}
