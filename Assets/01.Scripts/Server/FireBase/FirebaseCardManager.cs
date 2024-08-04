using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseCardManager : MonoBehaviour
{
    public FirebaseInit firebaseInit;
    private List<CardAsset> _allCards; // ������Ʈ �� ��� ī�� �������� ����Ʈ

    public GameObject CardPrefab; // ī�� Prefab
    public Transform Content; // ScrollView�� Content GameObject

    public List<CardAsset> viewCards = new List<CardAsset>(); // ������Ʈ �� ��� ī�� �������� ����Ʈ
    public Dictionary<CardAsset, int> viewCardss = new Dictionary<CardAsset, int>();

    private CharacterAsset selectedCharacter; // ���õ� ĳ����

    private void Awake()
    {
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
        // �̺�Ʈ ���� ����
        FirebaseInit.OnFirebaseInitialized -= OnFirebaseInitialized;
    }

    private void OnFirebaseInitialized()
    {
        // Firebase �ʱ�ȭ�� �Ϸ�Ǿ��� �� ȣ��Ǵ� �޼���
        Utils.Log("Firebase �ʱ�ȭ�� �Ϸ�Ǿ����ϴ�.");
    }

    public void SaveCardData(Dictionary<CardAsset, int> cards, string userEmail)
    {
        if (firebaseInit == null || firebaseInit.firestore == null)
        {
            Utils.LogRed("firebaseInit �Ǵ� firestore�� null�Դϴ�. SaveCardData�� ȣ���� �� �����ϴ�.");
            return;
        }

        CollectionReference cardsRef = firebaseInit.firestore.Collection("users").Document(userEmail).Collection("cards");

        foreach (var cardEntry in cards)
        {
            CardAsset card = cardEntry.Key;
            int cardCount = cardEntry.Value;

            Dictionary<string, object> cardData = new Dictionary<string, object>
            {
                { "cardName", card.CardScriptName },
                { "cardCount", cardCount },
                { "rarity", card.Rarity }
            };
            cardsRef.Document(card.CardScriptName).SetAsync(cardData);
        }
    }

    public async void LoadCardNames(string userEmail)
    {
        if (firebaseInit == null || firebaseInit.firestore == null)
        {
            Utils.LogRed("firebaseInit �Ǵ� firestore�� null�Դϴ�. LoadCardNames�� ȣ���� �� �����ϴ�.");
            return;
        }

        if (string.IsNullOrEmpty(userEmail))
        {
            Utils.LogRed("userEmail�� null�̰ų� �� ���ڿ��Դϴ�.");
            return;
        }

        Utils.Log("LoadCardNames ȣ���");
        Utils.Log($"����� �̸���: {userEmail}");

        CollectionReference cardsRef = firebaseInit.firestore.Collection("users").Document(userEmail).Collection("cards");
        Utils.Log($"Firestore ���: users/{userEmail}/cards");

        try
        {
            QuerySnapshot snapshot = await cardsRef.GetSnapshotAsync();
            Utils.Log("ī�� ������ �ε� �Ϸ�");
            List<CardAsset> loadedCards = new List<CardAsset>();
            Dictionary<CardAsset, int> loadedCardss = new Dictionary<CardAsset, int>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Utils.Log($"���� ID: {document.Id}");
                if (document.TryGetValue("cardName", out string cardName) && document.TryGetValue("cardCount", out int cardCount))
                {
                    Utils.Log($"ī�� �̸�: {cardName}");
                    Utils.Log($"ī�� ����: {cardCount}");
                    CardAsset card = FindCardByName(cardName);
                    if (card != null)
                    {
                        // Ŭ���� ���͸�
                        if (selectedCharacter == null || card.CharacterAsset == selectedCharacter)
                        {
                            loadedCards.Add(card);
                            loadedCardss.Add(card, cardCount);
                        }
                    }
                    else
                    {
                        Utils.LogRed($"ī�� �̸��� ã�� �� �����ϴ�: {cardName}");
                    }
                }
                else
                {
                    Utils.LogRed("ī�� �̸��� �������� ���߽��ϴ�.");
                }
            }
            viewCardss = loadedCardss;

            Utils.Log($"�ε�� ī�� ��: {loadedCards.Count}");

            // �ߺ� �߰� ����: viewCards �ʱ�ȭ
            viewCards.Clear();

            foreach (CardAsset card in loadedCards)
            {
                if (!viewCards.Contains(card))
                {
                    viewCards.Add(card);
                }
            }

            // ������ ������ ī�� UI ����
            foreach (Transform child in Content)
            {
                Destroy(child.gameObject);
            }

            // ī�� ���� �� ��ġ
            foreach (CardAsset card in viewCards)
            {
                GameObject newCard = Instantiate(CardPrefab, Content);
                OneCardManager manager = newCard.GetComponent<OneCardManager>();
                manager.cardAsset = card;
                manager.ReadCardFromAsset();

                AddCardToDeck addCardComponent = newCard.GetComponent<AddCardToDeck>();
                addCardComponent.SetCardAsset(card);
                addCardComponent.UpdateQuantity();
            }
        }
        catch (System.Exception e)
        {
            Utils.LogRed("ī�� ������ �ε� ����: " + e.Message);
        }
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
        LoadCardNames(LoginManager.Email); // ĳ���Ͱ� �����Ǹ� ī�带 �ٽ� �ε��Ͽ� ���͸��� ī�带 ǥ���մϴ�.
    }
}
