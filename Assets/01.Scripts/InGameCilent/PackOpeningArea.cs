using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class PackOpeningArea : MonoBehaviour
{
    public bool AllowedToDragAPack { get; set; }

    public GameObject AttacksCardPackPrefab;
    public GameObject MagicCardPackPrefab;
    public GameObject TechniqueCardPackPrefab;
    public Button DoneButton;
    public Button BackButton;
    [Header("Probabilities")]
    [Range(0, 1)]
    public float MythicProbability;
    [Range(0, 1)]
    public float EpicProbability;
    [Range(0, 1)]
    public float RareProbability;
    // these are the glow colors that will show while opening cards
    // or you can use colors from  RarityColors
    [Header("Colors")]
    public Color32 MythicsColor;
    public Color32 EpicColor;
    public Color32 RareColor;
    public Color32 NormalColor;
    public Color32 BasicColor;

    public Dictionary<RarityOptions, Color32> GlowColorsByRarity = new Dictionary<RarityOptions, Color32>();

    //가챠 시 희귀 한개는 꼭 떠야하는지 여부
    public bool giveAtLeastOneRare = false;

    public Transform[] SlotsForCards;

    private BoxCollider _boxCollider;
    private List<GameObject> CardsFromPackCreated = new List<GameObject>();
    private int numOfCardsOpened = 0;
    public int NumberOfCardsOpenedFromPack
    {
        get { return numOfCardsOpened; }
        set
        {
            numOfCardsOpened = value;
            if (value == SlotsForCards.Length)
            {
                // activate the Done button
                DoneButton.gameObject.SetActive(true);
            }
        }
    }

    void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        AllowedToDragAPack = true;

        GlowColorsByRarity.Add(RarityOptions.Basic, BasicColor);
        GlowColorsByRarity.Add(RarityOptions.Normal, NormalColor);
        GlowColorsByRarity.Add(RarityOptions.Rare, RareColor);
        GlowColorsByRarity.Add(RarityOptions.Epic, EpicColor);
        GlowColorsByRarity.Add(RarityOptions.Mythic, MythicsColor);
    }

    public bool CursorOverArea()
    {
        RaycastHit[] hits;
        // raycst to mousePosition and store all the hits in the array
        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));

        bool passedThroughTableCollider = false;
        foreach (RaycastHit h in hits)
        {
            // check if the collider that we hit is the collider on this GameObject
            if (h.collider == _boxCollider)
            {
                Debug.Log("커서가 위에 닿았아요");
                passedThroughTableCollider = true;
            }
        }
        return passedThroughTableCollider;
    }

    public void ShowPackOpening(Vector3 cardsInitialPosition)
    {
        // ShopManager.Instance.PacksCreated--;
        // Allow To Drag Another Pack Only After DoneButton Is pressed
        // 1) Determine rarity of all cards
        RarityOptions[] rarity = new RarityOptions[SlotsForCards.Length];
        bool AtLeastOneRareGiven = false;
        for (int i = 0; i < rarity.Length; i++)
        {
            // determine rarity of this card
            float prob = Random.Range(0f, 1f);
            if (prob < MythicProbability)
            {
                rarity[i] = RarityOptions.Mythic;
                AtLeastOneRareGiven = true;
            }
            else if (prob < EpicProbability)
            {
                rarity[i] = RarityOptions.Epic;
                AtLeastOneRareGiven = true;
            }
            else if (prob < RareProbability)
            {
                rarity[i] = RarityOptions.Rare;
                AtLeastOneRareGiven = true;
            }
            else
                rarity[i] = RarityOptions.Normal;
        }

        if (AtLeastOneRareGiven == false && giveAtLeastOneRare)
        {
            rarity[Random.Range(0, rarity.Length)] = RarityOptions.Rare;
        }

        for (int i = 0; i < rarity.Length; i++)
        {
            GameObject card = CardFromPack(rarity[i]);
            CardsFromPackCreated.Add(card);
            card.transform.position = cardsInitialPosition;
            card.transform.DOMove(SlotsForCards[i].position, 0.5f);
        }
    }

    private GameObject CardFromPack(RarityOptions rarity)
    {
        List<CardAsset> CardsOfThisRarity = CardCollection.instance.GetCardsWithRarity(rarity);
        CardAsset cardAsset = CardsOfThisRarity[Random.Range(0, CardsOfThisRarity.Count)];

        // add this card to your collection. 
        CardCollection.instance.QuantityOfEachCard[cardAsset]++;

        GameObject card;
        if (cardAsset.TypeOfCard == TypesOfCards.Attacks)
        {
            card = Instantiate(AttacksCardPackPrefab) as GameObject;
        }
        else if (cardAsset.TypeOfCard == TypesOfCards.Magics)
        {
            card = Instantiate(MagicCardPackPrefab) as GameObject;
        }
        else
        {
            card = Instantiate(TechniqueCardPackPrefab) as GameObject;
        }

        card.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        OneCardManager manager = card.GetComponent<OneCardManager>();
        manager.cardAsset = cardAsset;
        manager.ReadCardFromAsset();
        return card;
    }

    public void Done()
    {
        AllowedToDragAPack = true;
        NumberOfCardsOpenedFromPack = 0;
        while (CardsFromPackCreated.Count > 0)
        {
            GameObject g = CardsFromPackCreated[0];
            CardsFromPackCreated.RemoveAt(0);
            Destroy(g);
        }
        BackButton.interactable = true;
    }
}
