using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseCardManager : MonoBehaviour
{
    public FirebaseInit firebaseInit;
    public List<CardAsset> allCards; // ������Ʈ �� ��� ī�� �������� ����Ʈ


    // ī�� �̸��� Firestore�� ����
    public void SaveCardData(List<CardAsset> cards, string userEmail)
    {
        CollectionReference cardsRef = firebaseInit.firestore.Collection("users").Document(userEmail).Collection("cards");

        foreach (CardAsset card in cards)
        {
            Dictionary<string, object> cardData = new Dictionary<string, object>
            {
                { "cardName", card.CardScriptName },
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
