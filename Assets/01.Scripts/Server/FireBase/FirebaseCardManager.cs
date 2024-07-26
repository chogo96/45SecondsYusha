using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseCardManager : MonoBehaviour
{
    public FirebaseInit firebaseInit;
    private List<CardAsset> _allCards; // ������Ʈ �� ��� ī�� �������� ����Ʈ

    public GameObject CreatureMenuPrefab;
    public List<CardAsset> viewCards = new List<CardAsset>(); // ������Ʈ �� ��� ī�� �������� ����Ʈ

    public Dictionary<CardAsset, int> viewCardss = new Dictionary<CardAsset, int>();


    public Transform[] Slots;
    CollectionBrowser collectionBrowser = new CollectionBrowser();




    private void Awake()
    {
        if (firebaseInit == null)
        {
            Debug.LogError("FirebaseInit �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
        else
        {
            FirebaseInit.OnFirebaseInitialized += OnFirebaseInitialized;
            Debug.Log("FirebaseInit �ν��Ͻ��� ã�ҽ��ϴ�.");
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
        Debug.Log("Firebase �ʱ�ȭ�� �Ϸ�Ǿ����ϴ�.");
    }

    /// <summary>
    /// ī�� �̸��� Firestore�� ����
    /// </summary>
    /// <param name="cards">���� ī���</param>
    /// <param name="userEmail">���� �̸���</param>
    public void SaveCardData(Dictionary<CardAsset, int> cards, string userEmail)
    {

        if (firebaseInit == null || firebaseInit.firestore == null)
        {
            Debug.LogError("firebaseInit �Ǵ� firestore�� null�Դϴ�. SaveCardData�� ȣ���� �� �����ϴ�.");
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


    /// <summary>
    /// Firestore���� ī�� �̸��� �ҷ��� ����Ʈ�� ��ȯ
    /// </summary>
    /// <param name="userEmail">���� �̸���</param>
    public async void LoadCardNames(string userEmail)
    {
        if (firebaseInit == null || firebaseInit.firestore == null)
        {
            Debug.LogError("firebaseInit �Ǵ� firestore�� null�Դϴ�. LoadCardNames�� ȣ���� �� �����ϴ�.");
            return;
        }

        if (string.IsNullOrEmpty(userEmail))
        {
            Debug.LogError("userEmail�� null�̰ų� �� ���ڿ��Դϴ�.");
            return;
        }

        Debug.Log("LoadCardNames ȣ���");
        Debug.Log($"����� �̸���: {userEmail}");

        CollectionReference cardsRef = firebaseInit.firestore.Collection("users").Document(userEmail).Collection("cards");
        Debug.Log($"Firestore ���: users/{userEmail}/cards");

        try
        {
            QuerySnapshot snapshot = await cardsRef.GetSnapshotAsync();
            Debug.Log("ī�� ������ �ε� �Ϸ�");
            List<CardAsset> loadedCards = new List<CardAsset>();
            Dictionary<CardAsset,int> loadedCardss = new Dictionary<CardAsset, int>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Debug.Log($"���� ID: {document.Id}");
                if (document.TryGetValue("cardName", out string cardName) && document.TryGetValue("cardCount", out int cardCount))
                {
                    Debug.Log($"ī�� �̸�: {cardName}");
                    Debug.Log($"ī�� ����: {cardCount}");
                    CardAsset card = FindCardByName(cardName);
                    if (card != null)
                    {
                        loadedCards.Add(card);
                        loadedCardss.Add(card, cardCount);
                        
                    }
                    else
                    {
                        Debug.LogError($"ī�� �̸��� ã�� �� �����ϴ�: {cardName}");
                    }
                }
                else
                {
                    Debug.LogError("ī�� �̸��� �������� ���߽��ϴ�.");
                }
            }
            viewCardss = loadedCardss;

            Debug.Log($"�ε�� ī�� ��: {loadedCards.Count}");

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
            foreach (GameObject cardObject in collectionBrowser.CreatedCards)
            {
                Destroy(cardObject);
            }
            collectionBrowser.CreatedCards.Clear();


            


            for (int i = 0; i < viewCards.Count; i++)
            {
                GameObject newMenuCard;

                if (viewCards[i].TypeOfCard == TypesOfCards.Attacks)
                {
                    newMenuCard = Instantiate(CreatureMenuPrefab, Slots[i].position, Quaternion.identity);
                }
                else
                {
                    newMenuCard = Instantiate(CreatureMenuPrefab, Slots[i].position, Quaternion.identity);
                }

                newMenuCard.transform.SetParent(this.transform);

                collectionBrowser.CreatedCards.Add(newMenuCard);

                OneCardManager manager = newMenuCard.GetComponent<OneCardManager>();
                manager.cardAsset = viewCards[i];
                manager.ReadCardFromAsset();

                AddCardToDeck addCardComponent = newMenuCard.GetComponent<AddCardToDeck>();
                addCardComponent.SetCardAsset(viewCards[i]);
                addCardComponent.UpdateQuantity();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("ī�� ������ �ε� ����: " + e.Message);
        }
    }

    private CardAsset FindCardByName(string cardName)
    {
        string path = $"GameAssets/Cards/{cardName}";
        CardAsset card = Resources.Load<CardAsset>(path);

        if (card == null)
        {
            Debug.LogError($"ī�� ������ ã�� �� �����ϴ�: {path}");
        }
        else
        {
            Debug.Log($"ī�� ���� �ε� ����: {path}");
        }

        return card;
    }
}
