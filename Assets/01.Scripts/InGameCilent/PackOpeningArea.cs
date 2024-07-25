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
    // ī�� ���鼭 ������ glow ����
    // ���� ������ �����ְ� ī���� ������ ���� ������ �� ����
    [Header("Colors")]
    public Color32 MythicsColor;
    public Color32 EpicColor;
    public Color32 RareColor;
    public Color32 NormalColor;
    public Color32 BasicColor;

    public Dictionary<RarityOptions, Color32> GlowColorsByRarity = new Dictionary<RarityOptions, Color32>();

    //��í �� ��� �Ѱ��� �� �����ϴ��� ����
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
                // 5�� �� �������� done��ư Ȱ��ȭ
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
        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));

        bool passedThroughTableCollider = false;
        foreach (RaycastHit hit in hits)
        {
            // �ݶ��̴��� ������
            if (hit.collider == _boxCollider)
            {
                passedThroughTableCollider = true;
            }
        }
        return passedThroughTableCollider;
    }

    public void ShowPackOpening(Vector3 cardsInitialPosition)
    {
        // done��ư�� ������ ���� ī�带 �� �� �ְ� �ϴ� �Լ�
        RarityOptions[] rarity = new RarityOptions[SlotsForCards.Length];
        bool AtLeastOneRareGiven = false;
        for (int i = 0; i < rarity.Length; i++)
        {
            // �� ī���� ��� Ȯ��
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

        FirebaseCardManager firebaseCardManager = FindObjectOfType<FirebaseCardManager>();
        Dictionary<CardAsset,int> openCardPackData = new Dictionary<CardAsset,int>();

        for (int i = 0; i < rarity.Length; i++)
        {
            GameObject card = CardFromPack(rarity[i], openCardPackData);
            CardsFromPackCreated.Add(card);
            card.transform.position = cardsInitialPosition;
            card.transform.DOMove(SlotsForCards[i].position, 0.5f);
        }
        firebaseCardManager.SaveCardData(openCardPackData, LoginManager.Email);
    }

    private GameObject CardFromPack(RarityOptions rarity, Dictionary<CardAsset, int> openpack)
    {
        List<CardAsset> CardsOfThisRarity = CardCollection.instance.GetCardsWithRarity(rarity);
        CardAsset cardAsset = CardsOfThisRarity[Random.Range(0, CardsOfThisRarity.Count)];

        // ī�� �ݷ��ǿ� ī�忡�� �߰�
        CardCollection.instance.QuantityOfEachCard[cardAsset]++;

        // ���� ī�带 openpack ��ųʸ��� �߰�
        if (openpack.ContainsKey(cardAsset))
        {
            openpack[cardAsset] = CardCollection.instance.QuantityOfEachCard[cardAsset];
        }
        else
        {
            openpack.Add(cardAsset, CardCollection.instance.QuantityOfEachCard[cardAsset]);
        }

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
            GameObject gameObject = CardsFromPackCreated[0];
            CardsFromPackCreated.RemoveAt(0);
            Destroy(gameObject);
        }
        BackButton.interactable = true;
    }
}
