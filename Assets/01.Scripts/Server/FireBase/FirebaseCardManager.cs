using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseCardManager : MonoBehaviour
{
    public FirebaseInit firebaseInit;
    public GameObject CardPrefab; // ī�� Prefab
    public Transform Content; // ScrollView�� Content GameObject

    private List<CardAsset> basicCards; // �⺻ ī�� ����Ʈ
    public Dictionary<CardAsset, int> viewCardss = new Dictionary<CardAsset, int>(); // ī�� �� ����
    public List<CardAsset> viewCards = new List<CardAsset>(); // ���͸��� ī�� ����Ʈ

    public GameObject OneCharacterTabs; // Ư�� ĳ������ ī�常 �����ִ� ��
    public GameObject AllCharactersTabs; // ��ü ĳ������ ī�带 �����ִ� ��

    private CharacterAsset selectedCharacter; // ���õ� ĳ����

    private void Awake()
    {
        LoadBasicCards(); // �⺻ ī�� �ε�

        if (firebaseInit == null)
        {
            Utils.LogRed("FirebaseInit �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
        else
        {
            FirebaseInit.OnFirebaseInitialized += OnFirebaseInitialized;
            Utils.Log("FirebaseInit �ν��Ͻ��� ã�ҽ��ϴ�.");
        }
    }

    private void OnDestroy()
    {
        FirebaseInit.OnFirebaseInitialized -= OnFirebaseInitialized;
    }

    private void OnFirebaseInitialized()
    {
        Utils.Log("Firebase �ʱ�ȭ�� �Ϸ�Ǿ����ϴ�.");
    }

    private void LoadBasicCards()
    {
        basicCards = new List<CardAsset>
        {
            // �⺻ ī�带 �������� �߰��ϰų� ���ҽ����� �ε��մϴ�.
            Resources.Load<CardAsset>("GameAssets/Cards/�ݷ�"),
            Resources.Load<CardAsset>("GameAssets/Cards/�� ��� �Ƕ�!"),
            Resources.Load<CardAsset>("GameAssets/Cards/�Ӹ�!"),
            Resources.Load<CardAsset>("GameAssets/Cards/��� [��������]"),
            Resources.Load<CardAsset>("GameAssets/Cards/��������"),
            Resources.Load<CardAsset>("GameAssets/Cards/������ �ϰ�"),
            Resources.Load<CardAsset>("GameAssets/Cards/���� ����"),
            Resources.Load<CardAsset>("GameAssets/Cards/��� �ڼ�"),
            Resources.Load<CardAsset>("GameAssets/Cards/��ȣ"),
            Resources.Load<CardAsset>("GameAssets/Cards/�ش�"),
            Resources.Load<CardAsset>("GameAssets/Cards/�� ������"),
            Resources.Load<CardAsset>("GameAssets/Cards/�簢 ����"),
            Resources.Load<CardAsset>("GameAssets/Cards/������"),
            Resources.Load<CardAsset>("GameAssets/Cards/��ũ�� ����!"),
            Resources.Load<CardAsset>("GameAssets/Cards/�Ⱦ�"),
            Resources.Load<CardAsset>("GameAssets/Cards/�ձ� ����"),
            Resources.Load<CardAsset>("GameAssets/Cards/����"),
            Resources.Load<CardAsset>("GameAssets/Cards/�Ƿ�� ����"),
            Resources.Load<CardAsset>("GameAssets/Cards/������ ������!"),
            Resources.Load<CardAsset>("GameAssets/Cards/���̾ �ֹ���"),
            Resources.Load<CardAsset>("GameAssets/Cards/�ϱ� ��ȭ"),
            Resources.Load<CardAsset>("GameAssets/Cards/�ϱ� ġ��"),
        };

        // �⺻ ī�带 viewCardss�� 3�徿 �߰�
        foreach (var basicCard in basicCards)
        {
            viewCardss[basicCard] = 3;
        }
    }

    public void SaveCardData(Dictionary<CardAsset, int> cards, string userId)
    {
        if (firebaseInit == null || firebaseInit.firestore == null)
        {
            Utils.LogRed("firebaseInit �Ǵ� firestore�� null�Դϴ�. SaveCardData�� ȣ���� �� �����ϴ�.");
            return;
        }

        CollectionReference cardsRef = firebaseInit.firestore.Collection("users").Document(userId).Collection("cards");

        foreach (var cardEntry in cards)
        {
            var cardData = new Dictionary<string, object>
            {
                { "cardName", cardEntry.Key.CardScriptName },
                { "cardCount", cardEntry.Value },
                { "rarity", cardEntry.Key.Rarity }
            };
            cardsRef.Document(cardEntry.Key.CardScriptName).SetAsync(cardData);
        }
    }

    public async void LoadCardNames(string userId)
    {
        if (firebaseInit == null || firebaseInit.firestore == null)
        {
            Utils.LogRed("firebaseInit �Ǵ� firestore�� null�Դϴ�. LoadCardNames�� ȣ���� �� �����ϴ�.");
            return;
        }

        if (string.IsNullOrEmpty(userId))
        {
            Utils.LogRed("userId�� null�̰ų� �� ���ڿ��Դϴ�.");
            return;
        }

        try
        {
            CollectionReference cardsRef = firebaseInit.firestore.Collection("users").Document(userId).Collection("cards");
            QuerySnapshot snapshot = await cardsRef.GetSnapshotAsync();

            List<CardAsset> loadedCards = new List<CardAsset>();
            Dictionary<CardAsset, int> loadedCardss = new Dictionary<CardAsset, int>();

            foreach (var document in snapshot.Documents)
            {
                if (document.TryGetValue("cardName", out string cardName) && document.TryGetValue("cardCount", out int cardCount))
                {
                    CardAsset card = FindCardByName(cardName);
                    if (card != null && (selectedCharacter == null || card.CharacterAsset == selectedCharacter || card.CharacterAsset == null))
                    {
                        loadedCards.Add(card);
                        loadedCardss[card] = cardCount;
                    }
                }
            }

            // �⺻ ī�嵵 ���͸��Ͽ� �߰�
            foreach (var basicCard in basicCards)
            {
                if (selectedCharacter == null || basicCard.CharacterAsset == selectedCharacter || basicCard.CharacterAsset == null)
                {
                    loadedCards.Add(basicCard);
                    if (!loadedCardss.ContainsKey(basicCard))
                    {
                        loadedCardss[basicCard] = 3;
                    }
                }
            }

            viewCardss = loadedCardss;
            viewCards.Clear();
            viewCards.AddRange(loadedCards);

            UpdateCardUI();
        }
        catch (System.Exception e)
        {
            Utils.LogRed("ī�� ������ �ε� ����: " + e.Message);
        }
    }

    // ������ ī�� UI�� ��� �����ϴ� �޼���
    public void ClearCreatedCards()
    {
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }
    }

    // Ư�� ī���� ������ ��ȯ�ϴ� �޼���
    public int GetCardCount(string userId, CardAsset card)
    {
        if (viewCardss.TryGetValue(card, out int count))
        {
            return count;
        }
        return 0; // ī�尡 ������ 0 ��ȯ
    }

    private CardAsset FindCardByName(string cardName)
    {
        string path = $"GameAssets/Cards/{cardName}";
        CardAsset card = Resources.Load<CardAsset>(path);

        if (card == null)
        {
            Utils.LogRed($"ī�� ������ ã�� �� �����ϴ�: {path}");
        }
        else
        {
            Utils.Log($"ī�� ���� �ε� ����: {path}");
        }

        return card;
    }

    public void SetSelectedCharacter(CharacterAsset character)
    {
        selectedCharacter = character;
        LoadCardNames(LoginManager.UserId);
    }

    private void UpdateCardUI()
    {
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }

        foreach (var card in viewCards)
        {
            GameObject newCard = Instantiate(CardPrefab, Content);
            OneCardManager manager = newCard.GetComponent<OneCardManager>();
            manager.cardAsset = card;
            manager.ReadCardFromAsset();

            AddCardToDeck addCardComponent = newCard.GetComponent<AddCardToDeck>();
            addCardComponent.SetCardAsset(card);

            if (viewCardss.TryGetValue(card, out int cardCount))
            {
                addCardComponent.UpdateQuantity(cardCount);
            }
            else
            {
                addCardComponent.UpdateQuantity(0);
            }
        }
    }
}
