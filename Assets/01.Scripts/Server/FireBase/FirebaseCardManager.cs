using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseCardManager : MonoBehaviour
{
    private FirebaseInit firebaseInit;
    public List<CardAsset> allCards; // ������Ʈ �� ��� ī�� �������� ����Ʈ

    private void Awake()
    {
        FirebaseInit.OnFirebaseInitialized += FirebaseGetcomponent;
    }


    private void FirebaseGetcomponent()
    {
        firebaseInit = FindObjectOfType<FirebaseInit>();
        if (firebaseInit == null)
        {
            Debug.LogError("FirebaseInit �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
        else
        {
            Debug.Log("FirebaseInit �ν��Ͻ��� ã�ҽ��ϴ�.");
        }

    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        FirebaseInit.OnFirebaseInitialized -= FirebaseGetcomponent;
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
                { "cardCount", cardCount }
            };
            cardsRef.Document(card.CardScriptName).SetAsync(cardData);
        }
    }


    /// <summary>
    /// Firestore���� ī�� �̸��� �ҷ��� ����Ʈ�� ��ȯ
    /// </summary>
    /// <param name="userEmail">���� �̸���</param>
    public void LoadCardNames(string userEmail)
    {
        CollectionReference cardsRef = firebaseInit.firestore.Collection("users").Document(userEmail).Collection("cards");
        cardsRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                QuerySnapshot snapshot = task.Result;
                List<CardAsset> loadedCards = new List<CardAsset>();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.TryGetValue("cardName", out string cardName))
                    {
                        CardAsset card = FindCardByName(cardName);
                        if (card != null)
                        {
                            loadedCards.Add(card);
                        }
                    }
                }

                // ī�� ����Ʈ�� �غ�Ϸ� => ����� ���� ������Ʈ ���°����� ���̰� �����ϸ� �ɵ�
                foreach (CardAsset card in loadedCards)
                {
                    Debug.Log($"Card: {card.CardScriptName}, SwordAttack: {card.SwordAttack}, MagicAttack: {card.MagicAttack}, ShieldAttack: {card.ShieldAttack}");
                }
            }
        });
    }

    private CardAsset FindCardByName(string cardName)
    {
        return allCards.Find(card => card.CardScriptName == cardName);
    }
}
