using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseCardManager : MonoBehaviour
{
    public FirebaseInit firebaseInit;
    public List<CardAsset> allCards; // 프로젝트 내 모든 카드 데이터의 리스트


    // 카드 이름만 Firestore에 저장
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
    /// Firestore에서 카드 이름을 불러와 리스트로 변환
    /// </summary>
    /// <param name="userEmail">유저 이메일</param>
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

                // 카드 리스트가 준비완료 => 여기는 이제 덱리스트 보는곳에서 보이게 수정하면 될듯
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
